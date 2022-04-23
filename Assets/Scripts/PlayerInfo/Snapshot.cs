using Assets;
using ScriptableObjects;
using System;
using System.Collections.Generic;
using UnityEngine;

using AssetTypeEntity = System.Tuple<
    string, System.Collections.Generic.List<Assets.AbstractAsset>>;
using IncomeEntry = System.Tuple<Assets.AbstractAsset, int>;
using ItemValueEntry = System.Tuple<
    int, string, UnityEngine.Vector2Int, System.Action>;

namespace PlayerInfo
{
    public class Snapshot
    {
        public Vector2Int totalCashflowRange { get; private set; }
        public int netWorth { get; private set; }
        public int financialIndependenceProgress { get; private set; }
        public int totalActiveIncome { get; private set; }
        public int totalFixedExpenses { get; private set; }
        public Vector2Int passiveIncomeRange { get; private set; }
        public List<ItemValueEntry> itemsActiveIncome { get; private set; }
        public List<ItemValueEntry> itemsPassiveIncome { get; private set; }
        public List<ItemValueEntry> itemsFixedExpenses { get; private set; }
        public List<AbstractAsset> assets { get; private set; }

        public int availablePersonalLoanAmount { get; private set; }

        private void AddActiveIncome(Player player)
        {
            Localization local = Localization.Instance;
            int tabCount = 0;

            foreach (ScriptableObjects.Profession income in player.jobs)
            {
                totalActiveIncome += income.salary;
                itemsActiveIncome.Add(
                    new ItemValueEntry(
                        tabCount,
                        income.professionName,
                        new Vector2Int(income.salary, income.salary),
                        null));
            }

            if (player.spouse != null && player.spouse.additionalIncome > 0)
            {
                int income = player.spouse.additionalIncome;
                totalActiveIncome += income;
                itemsActiveIncome.Add(
                    new ItemValueEntry(
                        tabCount,
                        "Spouse",
                        new Vector2Int(income, income),
                        null));
            }
        }

        private void addInvestmentsByType(
            string investmentType,
            List<AbstractAsset> assets,
            int tabCount,
            Func<AbstractAsset, Action> getClickAction)
        {
            Localization local = Localization.Instance;
            List<ItemValueEntry> entries = new List<ItemValueEntry>();
            foreach (AbstractAsset asset in assets)
            {
                netWorth += asset.value;
                netWorth -= asset.combinedLiability.amount;

                Vector2Int incomeRange = asset.netIncomeRange;
                if (incomeRange != Vector2Int.zero)
                {
                    passiveIncomeRange += incomeRange;
                    entries.Add(new ItemValueEntry(
                        tabCount + 1,
                        asset.name,
                        incomeRange,
                        getClickAction?.Invoke(asset)));
                }
            }

            if (entries.Count > 0)
            {
                itemsPassiveIncome.Add(
                    new ItemValueEntry(tabCount, investmentType, Vector2Int.zero, null));
                itemsPassiveIncome.AddRange(entries);
            }
        }

        private void AddPassiveIncome(
            Player player,
            Func<AbstractAsset, Action> getClickAction)
        {
            Localization local = Localization.Instance;
            int tabCount = 0;

            foreach (AssetTypeEntity assetType in player.portfolio.assetsByType)
            {
                assets.AddRange(assetType.Item2);
                addInvestmentsByType(
                    assetType.Item1,
                    assetType.Item2,
                    tabCount,
                    getClickAction);
            }
        }

        private void AddFixedExpenses(
            Player player,
            Func<AbstractLiability, Action> getLiabilityClickAction,
            Action healthInsuranceAction)
        {
            Localization local = Localization.Instance;
            int tabCount = 0;

            // Personal Expenses
            totalFixedExpenses += player.personalExpenses;
            itemsFixedExpenses.Add(
                new ItemValueEntry(
                    tabCount,
                    "Personal Expenses",
                    new Vector2Int(player.personalExpenses, player.personalExpenses),
                    null));

            if (player.spouse != null && player.spouse.additionalExpense > 0)
            {
                int expense = player.spouse.additionalExpense;
                totalFixedExpenses += expense;
                itemsFixedExpenses.Add(
                    new ItemValueEntry(
                        tabCount,
                        "Spouse's Expenses",
                        new Vector2Int(expense, expense),
                        null));
            }

            // Child Expenses
            if (player.numChild > 0)
            {
                int childCost = player.numChild * player.costPerChild;
                totalFixedExpenses += childCost;
                itemsFixedExpenses.Add(
                    new ItemValueEntry(
                        tabCount,
                        "Child Expenses",
                        new Vector2Int(childCost, childCost),
                        null));
            }

            if (player.portfolio.hasHealthInsurance)
            {
                int cost = PersonalEventManager.Instance.healthInsuranceCost;
                totalFixedExpenses += cost;
                itemsFixedExpenses.Add(
                    new ItemValueEntry(
                        tabCount,
                        "Health Insurance",
                        new Vector2Int(cost, cost),
                        healthInsuranceAction));
            }

            // Liabilities
            List<AbstractLiability> liabilities = player.portfolio.liabilities.FindAll(
                l => l.expense > 0);
            if (liabilities.Count > 0)
            {
                itemsFixedExpenses.Add(
                    new ItemValueEntry(
                        tabCount, "Other Liabilities", Vector2Int.zero, null));
                foreach (AbstractLiability liability in liabilities)
                {
                    netWorth -= liability.amount;

                    int expense = liability.expense;
                    totalFixedExpenses += expense;
                    itemsFixedExpenses.Add(
                        new ItemValueEntry(
                            tabCount + 1,
                            liability.shortName,
                            new Vector2Int(expense, expense),
                            getLiabilityClickAction?.Invoke(liability)));
                }
            }
        }

        public int GetActualIncome(System.Random random)
        {
            int income = totalActiveIncome - totalFixedExpenses;
            assets.ForEach(a => income += a.GetActualIncome(random));
            return income;
        }

        public Snapshot(
            Player player,
            Func<AbstractAsset, Action> getAssetClickAction = null,
            Func<AbstractLiability, Action> getLiabilityClickAction = null,
            Action healthInsuranceAction = null)
        {
            itemsActiveIncome = new List<ItemValueEntry>();
            itemsPassiveIncome = new List<ItemValueEntry>();
            itemsFixedExpenses = new List<ItemValueEntry>();
            assets = new List<AbstractAsset>();
            totalActiveIncome = 0;
            totalFixedExpenses = 0;
            passiveIncomeRange = Vector2Int.zero;
            totalCashflowRange = Vector2Int.zero;
            netWorth = player.cash;

            AddActiveIncome(player);
            AddFixedExpenses(player, getLiabilityClickAction, healthInsuranceAction);
            AddPassiveIncome(player, getAssetClickAction);

            itemsActiveIncome.ForEach(e => totalCashflowRange += e.Item3);
            itemsPassiveIncome.ForEach(e => totalCashflowRange += e.Item3);
            itemsFixedExpenses.ForEach(e => totalCashflowRange -= e.Item3);
            financialIndependenceProgress = Mathf.FloorToInt(
                100 * passiveIncomeRange.x / totalFixedExpenses);

            availablePersonalLoanAmount =
                totalCashflowRange.x / InterestRateManager.Instance.personalLoanRate * 100;
            if (player.portfolio.personalLoan != null)
            {
                availablePersonalLoanAmount -= player.portfolio.personalLoan.amount;
            }
            availablePersonalLoanAmount = Mathf.Max(availablePersonalLoanAmount, 0);
        }
    }
}
