using Actions;
using Assets;
using PlayerInfo;
using System;
using System.Collections.Generic;
using UI.Panels.Templates;
using UnityEngine;

namespace UI.Panels.PlayerDetails
{
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

        private void AddActiveIncome()
        {
            Localization local = Localization.Instance;
            int activeIncome = 0;
            int tabCount = _panelActiveIncome.firstItemValuePanel.tabCount + 1;

            foreach (ScriptableObjects.Profession income in player.jobs)
            {
                activeIncome += income.salary;
                _panelActiveIncome.AddItemValue(
                    income.professionName,
                    tabCount,
                    local.GetCurrency(income.salary));
            }

            if (player.spouse != null && player.spouse.additionalIncome > 0)
            {
                activeIncome += player.spouse.additionalIncome;
                _panelActiveIncome.AddItemValue(
                    "Spouse",
                    tabCount,
                    local.GetCurrency(player.spouse.additionalIncome));
            }

            _panelActiveIncome.ActivateIfNonEmpty();
            if (_showTotalValues)
            {
                _panelActiveIncome.firstItemValuePanel.SetValue(
                    local.GetCurrency(activeIncome));
            }
            else
            {
                _panelActiveIncome.firstItemValuePanel.RemoveValue();
            }
        }

        private void addInvestmentsByType(
            ItemValueListPanel panelParent,
            Player player,
            string investmentType,
            List<AbstractAsset> assets,
            int tabCount,
            ref int total,
            bool positive)
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

            panelParent.AddItem(investmentType, tabCount);

            foreach (AbstractAsset asset in assets)
            {
                int value = (positive ? 1 : -1) * asset.expectedIncome;
                total += value;
                ItemValuePanel panel = panelParent.AddItemValue(
                    asset.name,
                    tabCount + 1,
                    local.GetCurrency(value, !positive));
                panel.clickAction = () => asset.OnDetail(player, reloadWindow);
            }
        }

        private void AddPassiveIncome()
        {
            Localization local = Localization.Instance;
            int passiveIncome = 0;
            int baseTabCount = _panelPassiveIncome.firstItemValuePanel.tabCount + 1;

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
                _panelPassiveIncome.AddItem("Liquid Assets", baseTabCount);
                foreach (PurchasedStock stock in stocks)
                {
                    int income = stock.expectedIncome;
                    passiveIncome += income;
                    ItemValuePanel panel = _panelPassiveIncome.AddItemValue(
                        stock.stock.longName,
                        baseTabCount + 1,
                        local.GetCurrency(income));
                    panel.clickAction = () => stock.OnDetail(player, reloadWindow);
                }
            }

            addInvestmentsByType(
                _panelPassiveIncome,
                player,
                "Real Estate",
                player.portfolio.properties.ConvertAll(p => (AbstractAsset)p),
                baseTabCount,
                ref passiveIncome,
                true);
            addInvestmentsByType(
                _panelPassiveIncome,
                player,
                "Businesses",
                player.portfolio.businesses.ConvertAll(p => (AbstractAsset)p),
                baseTabCount,
                ref passiveIncome,
                true);
            addInvestmentsByType(
                _panelPassiveIncome,
                player,
                "Other Assets",
                player.portfolio.otherAssets,
                baseTabCount,
                ref passiveIncome,
                true);

            _panelPassiveIncome.ActivateIfNonEmpty();
            if (_showTotalValues)
            {
                _panelPassiveIncome.firstItemValuePanel.SetValue(
                    local.GetCurrency(passiveIncome));
            }
            else
            {
                _panelPassiveIncome.firstItemValuePanel.RemoveValue();
            }
        }

        private void AddExpenses()
        {
            Localization local = Localization.Instance;
            int expenses = 0;
            int baseTabCount = _panelExpenses.firstItemValuePanel.tabCount + 1;

            // Personal Expenses
            expenses += player.personalExpenses;
            _panelExpenses.AddItemValue(
                "Personal Expenses",
                baseTabCount,
                local.GetCurrency(player.personalExpenses, true));

            if (player.spouse != null && player.spouse.additionalExpense > 0)
            {
                expenses += player.spouse.additionalExpense;
                _panelExpenses.AddItemValue(
                    "Spouse's Expenses",
                    baseTabCount,
                    local.GetCurrency(player.spouse.additionalExpense, true));
            }

            // Child Expenses
            if (player.numChild > 0)
            {
                int childCost = player.numChild * player.costPerChild;
                expenses += childCost;
                _panelExpenses.AddItemValue(
                    "Child Expenses",
                    baseTabCount,
                    local.GetCurrency(childCost, true));
            }

            if (player.portfolio.hasHealthInsurance)
            {
                expenses += PersonalEventManager.Instance.healthInsuranceCost;
                ItemValuePanel panel = _panelExpenses.AddItemValue(
                    "Health Insurance",
                    baseTabCount,
                    local.GetCurrency(PersonalEventManager.Instance.healthInsuranceCost, true));
                panel.clickAction = () => CancelHealthInsurance.Run(player, reloadWindow);
            }

            // Assets with negative cashflow
            addInvestmentsByType(
                _panelExpenses,
                player,
                "Real Estate",
                player.portfolio.properties.ConvertAll(p => (AbstractAsset)p),
                baseTabCount,
                ref expenses,
                false);
            addInvestmentsByType(
                _panelExpenses,
                player,
                "Businesses",
                player.portfolio.businesses.ConvertAll(p => (AbstractAsset)p),
                baseTabCount,
                ref expenses,
                false);
            addInvestmentsByType(
                _panelExpenses,
                player,
                "Other Assets",
                player.portfolio.otherAssets,
                baseTabCount,
                ref expenses,
                false);

            // Liabilities
            List<AbstractLiability> liabilities = player.portfolio.liabilities.FindAll(
                l => l.expense > 0);
            if (liabilities.Count > 0)
            {
                _panelExpenses.AddItem("Other Liabilities", baseTabCount);
                foreach (AbstractLiability liability in liabilities)
                {
                    int expense = liability.expense;
                    if (expense > 0)
                    {
                        expenses += expense;
                        ItemValuePanel panel = _panelExpenses.AddItemValue(
                            liability.shortName,
                            baseTabCount + 1,
                            local.GetCurrency(expense, true));
                        panel.clickAction = () => liability.OnDetail(player, reloadWindow);
                    }
                }
            }

            _panelExpenses.ActivateIfNonEmpty();
            if (_showTotalValues)
            {
                _panelExpenses.firstItemValuePanel.SetValue(
                    local.GetCurrency(expenses, true));
            }
            else
            {
                _panelExpenses.firstItemValuePanel.RemoveValue();
            }
        }

        public void RefreshContent()
        {
            if (player == null)
            {
                return;
            }

            AddActiveIncome();
            AddPassiveIncome();
            AddExpenses();

            Snapshot snapshot = new Snapshot(player);
            _panelTotalCashflow.SetValue(
                Localization.Instance.GetCurrency(snapshot.expectedCashflow));

            _panelFinancialIndependence.SetValue(
                string.Format("{0}%", snapshot.financialIndependenceProgress));
        }

        private void OnEnable()
        {
            RefreshContent();
        }

        private void reloadWindow()
        {
            GetComponent<MessageBox>().Destroy();
            UIManager.Instance.ShowIncomeExpenseStatusPanel();
        }
    }
}
