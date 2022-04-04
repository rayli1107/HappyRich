using Actions;
using Assets;
using PlayerInfo;
using System;
using System.Collections.Generic;
using UI.Panels.Templates;
using UnityEngine;

using ItemValueEntry = System.Tuple<int, string, string, System.Action>;

namespace UI.Panels.PlayerDetails
{
    public class IncomeExpenseSnapshot
    {
        public int totalCashflow;
        public int financialIndependenceProgress;
        public int totalActiveIncome;
        public int totalPassiveIncome;
        public int totalExpenses;
        public List<ItemValueEntry> itemsActiveIncome;
        public List<ItemValueEntry> itemsPassiveIncome;
        public List<ItemValueEntry> itemsExpenses;

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
                        local.GetCurrency(income.salary),
                        null));
            }

            if (player.spouse != null && player.spouse.additionalIncome > 0)
            {
                totalActiveIncome += player.spouse.additionalIncome;
                itemsActiveIncome.Add(
                    new ItemValueEntry(
                        tabCount,
                        "Spouse",
                        local.GetCurrency(player.spouse.additionalIncome),
                        null));
            }
        }

        private void addInvestmentsByType(
            List<ItemValueEntry> entries,
            string investmentType,
            List<AbstractAsset> assets,
            int tabCount,
            ref int total,
            bool positive,
            Func<AbstractAsset, Action> getClickAction)
        {
            Localization local = Localization.Instance;
            if (positive)
            {
                assets = assets.FindAll(a => a.expectedIncome > 0);
            }
            else
            {
                assets = assets.FindAll(a => a.expectedIncome < 0);
            }

            if (assets.Count == 0)
            {
                return;
            }

            entries.Add(new ItemValueEntry(tabCount, investmentType, null, null));

            foreach (AbstractAsset asset in assets)
            {
                int value = (positive ? 1 : -1) * asset.expectedIncome;
                total += value;
                entries.Add(
                    new ItemValueEntry(
                        tabCount + 1,
                        asset.name,
                        local.GetCurrency(value, !positive),
                        getClickAction?.Invoke(asset)));
            }
        }

        private void AddPassiveIncome(
            Player player,
            Func<AbstractAsset, Action> getClickAction)
        {
            Localization local = Localization.Instance;
            int tabCount = 0;

            // Stocks
            List<PurchasedStock> stocks = new List<PurchasedStock>();
            foreach (KeyValuePair<string, PurchasedStock> entry in player.portfolio.stocks)
            {
                if (entry.Value.expectedIncome > 0)
                {
                    stocks.Add(entry.Value);
                }
            }
            if (stocks.Count > 0)
            {
                itemsPassiveIncome.Add(
                    new ItemValueEntry(tabCount, "Liquid Assets", null, null));
                foreach (PurchasedStock stock in stocks)
                {
                    int income = stock.expectedIncome;
                    totalPassiveIncome += income;

                    itemsPassiveIncome.Add(
                        new ItemValueEntry(
                            tabCount + 1,
                            stock.stock.longName,
                            local.GetCurrency(income),
                            getClickAction?.Invoke(stock)));
                }
            }

            addInvestmentsByType(
                itemsPassiveIncome,
                "Real Estate",
                player.portfolio.properties.ConvertAll(p => (AbstractAsset)p),
                tabCount,
                ref totalPassiveIncome,
                true,
                getClickAction);
            addInvestmentsByType(
                itemsPassiveIncome,
                "Businesses",
                player.portfolio.businesses.ConvertAll(p => (AbstractAsset)p),
                tabCount,
                ref totalPassiveIncome,
                true,
                getClickAction);
            addInvestmentsByType(
                itemsPassiveIncome,
                "Other Assets",
                player.portfolio.otherAssets,
                tabCount,
                ref totalPassiveIncome,
                true,
                getClickAction);
        }

        private void AddExpenses(
            Player player,
            Func<AbstractAsset, Action> getAssetClickAction,
            Func<AbstractLiability, Action> getLiabilityClickAction,
            Action healthInsuranceAction)
        {
            Localization local = Localization.Instance;
            int tabCount = 0;

            // Personal Expenses
            totalExpenses += player.personalExpenses;
            itemsExpenses.Add(
                new ItemValueEntry(
                    tabCount,
                    "Personal Expenses",
                    local.GetCurrency(player.personalExpenses, true),
                    null));

            if (player.spouse != null && player.spouse.additionalExpense > 0)
            {
                totalExpenses += player.spouse.additionalExpense;
                itemsExpenses.Add(
                    new ItemValueEntry(
                        tabCount,
                        "Spouse's Expenses",
                        local.GetCurrency(player.spouse.additionalExpense, true),
                        null));
            }

            // Child Expenses
            if (player.numChild > 0)
            {
                int childCost = player.numChild * player.costPerChild;
                totalExpenses += childCost;
                itemsExpenses.Add(
                    new ItemValueEntry(
                        tabCount,
                        "Child Expenses",
                        local.GetCurrency(childCost, true),
                        null));
            }

            if (player.portfolio.hasHealthInsurance)
            {
                totalExpenses += PersonalEventManager.Instance.healthInsuranceCost;
                itemsExpenses.Add(
                    new ItemValueEntry(
                        tabCount,
                        "Health Insurance",
                        local.GetCurrency(PersonalEventManager.Instance.healthInsuranceCost, true),
                        healthInsuranceAction));
            }

            // Assets with negative cashflow
            addInvestmentsByType(
                itemsExpenses,
                "Real Estate",
                player.portfolio.properties.ConvertAll(p => (AbstractAsset)p),
                tabCount,
                ref totalExpenses,
                false,
                getAssetClickAction);
            addInvestmentsByType(
                itemsExpenses,
                "Businesses",
                player.portfolio.businesses.ConvertAll(p => (AbstractAsset)p),
                tabCount,
                ref totalExpenses,
                false,
                getAssetClickAction);
            addInvestmentsByType(
                itemsExpenses,
                "Other Assets",
                player.portfolio.otherAssets,
                tabCount,
                ref totalExpenses,
                false,
                getAssetClickAction);

            // Liabilities
            List <AbstractLiability> liabilities = player.portfolio.liabilities.FindAll(
                l => l.expense > 0);
            if (liabilities.Count > 0)
            {
                itemsExpenses.Add(
                    new ItemValueEntry(
                        tabCount, "Other Liabilities", null, null));
                foreach (AbstractLiability liability in liabilities)
                {
                    int expense = liability.expense;
                    if (expense > 0)
                    {
                        totalExpenses += expense;
                        itemsExpenses.Add(
                            new ItemValueEntry(
                                tabCount + 1,
                                liability.shortName,
                                local.GetCurrency(expense, true),
                                getLiabilityClickAction?.Invoke(liability)));
                    }
                }
            }
        }

        public IncomeExpenseSnapshot(
            Player player,
            Func<AbstractAsset, Action> getAssetClickAction,
            Func<AbstractLiability, Action> getLiabilityClickAction,
            Action healthInsuranceAction)
        {
            itemsActiveIncome = new List<ItemValueEntry>();
            itemsPassiveIncome = new List<ItemValueEntry>();
            itemsExpenses = new List<ItemValueEntry>();

            AddActiveIncome(player);
            AddPassiveIncome(player, getAssetClickAction);
            AddExpenses(
                player, getAssetClickAction, getLiabilityClickAction, healthInsuranceAction);

            Snapshot snapshot = new Snapshot(player);
            totalCashflow = snapshot.expectedCashflow;
            financialIndependenceProgress = snapshot.financialIndependenceProgress;
        }

    }

    public class IncomeExpenseListPanel : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField]
        private ItemValuePanel _panelTotalCashflow;
        [SerializeField]
        private ItemValuePanel _panelFinancialIndependence;
        [SerializeField]
        private ItemValueListPanel _panelActiveIncome;
        [SerializeField]
        private ItemValueListPanel _panelPassiveIncome;
        [SerializeField]
        private ItemValueListPanel _panelExpenses;
        [SerializeField]
        private bool _showTotalValues = false;
#pragma warning restore 0649

        public Player player;
        public IncomeExpenseSnapshot incomeExpenseSnapshot;

        private void setupItemValueList(
            ItemValueListPanel panel,
            List<ItemValueEntry> entries,
            int totalValue,
            bool positive)
        {
            Localization local = Localization.Instance;
            int tabCount = panel.firstItemValuePanel.tabCount + 1;

            panel.Clear();

            if (_showTotalValues)
            {
                panel.firstItemValuePanel.SetValue(
                    local.GetCurrency(totalValue, !positive));
            }
            else
            {
                panel.firstItemValuePanel.RemoveValue();
            }

            foreach (ItemValueEntry entry in entries)
            {
                ItemValuePanel childPanel = panel.AddItemValue(
                    entry.Item2,
                    tabCount + entry.Item1,
                    entry.Item3);
                childPanel.clickAction = entry.Item4;
            }
            panel.ActivateIfNonEmpty();
        }

        private void refreshFromSnapshot(IncomeExpenseSnapshot snapshot)
        {
            Localization local = Localization.Instance;
            _panelTotalCashflow.SetValue(
                Localization.Instance.GetCurrency(snapshot.totalCashflow));

            _panelFinancialIndependence.SetValue(
                string.Format("{0}%", snapshot.financialIndependenceProgress));

            setupItemValueList(
                _panelActiveIncome,
                snapshot.itemsActiveIncome,
                snapshot.totalActiveIncome,
                true);
            setupItemValueList(
                _panelPassiveIncome,
                snapshot.itemsPassiveIncome,
                snapshot.totalPassiveIncome,
                true);
            setupItemValueList(
                _panelExpenses,
                snapshot.itemsExpenses,
                snapshot.totalExpenses,
                false);
        }

        public void RefreshContent()
        {
            if (incomeExpenseSnapshot != null)
            {
                refreshFromSnapshot(incomeExpenseSnapshot);
                return;
            }

            if (player == null)
            {
                return;
            }

            refreshFromSnapshot(
                new IncomeExpenseSnapshot(
                    player,
                    asset => () => asset.OnDetail(player, reloadWindow),
                    liability => () => liability.OnDetail(player, reloadWindow),
                    () => CancelHealthInsurance.Run(player, reloadWindow)));
        }

        private void OnEnable()
        {
            RefreshContent();
        }

        private void reloadWindow()
        {
            GetComponent<MessageBox>().Destroy();
            UIManager.Instance.ShowIncomeExpenseStatusPanel(incomeExpenseSnapshot);
        }
    }
}
