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
        /*
        private void showAssetLiabilityDetails(
            AbstractAsset asset, AbstractLiability loan, int fontSizeMax = 32)
        {
            List<string> details =
                asset == null ? loan.GetDetails() : asset.GetDetails();
            SimpleTextMessageBox panel = UIManager.Instance.ShowSimpleMessageBox(
                string.Join("\n", details), ButtonChoiceType.OK_ONLY, null);
            panel.text.fontSizeMax = fontSizeMax;
        }

        private Action getClickAction(
            AbstractAsset asset, AbstractLiability loan)
        {
            if (loan != null && loan.payable && loan.amount > 0)
            {
                return () => LoanPayoffActions.PayAssetLoanPrincipal(
                    player, asset, loan, reloadWindow);
            }
            else
            {
                return () => showAssetLiabilityDetails(asset, loan);
            }
        }*/

/*
        private int AddItemValueAsCurrency(
            ItemValueListPanel parentPanel,
            string label,
            int value,
            bool flip,
            Action clickAction = null)
        {
            ItemValuePanel panel = Instantiate(_prefabItemValuePanel, parentTranform);
            panel.setLabel(label);
            if (value != 0)
            {
                panel.setValueAsCurrency(value, flip);
            }
            else
            {
                panel.removeValue();
            }
            panel.setTabCount(tab);
            panel.transform.SetSiblingIndex(index);
            panel.clickAction = clickAction;
            return index + 1;
        }
*/
        private void AddActiveIncome()
        {
            int activeIncome = 0;
            int tabCount = _panelActiveIncome.firstItemValuePanel.tabCount + 1;

            foreach (ScriptableObjects.Profession income in player.jobs)
            {
                activeIncome += income.salary;
                _panelActiveIncome.AddItemValueAsCurrency(
                    income.professionName, tabCount, income.salary);
            }

            if (player.spouse != null && player.spouse.additionalIncome > 0)
            {
                activeIncome += player.spouse.additionalIncome;
                _panelActiveIncome.AddItemValueAsCurrency(
                    "Spouse", tabCount, player.spouse.additionalIncome);
            }

            bool enable = _panelActiveIncome.itemCount > 0;
            _panelActiveIncome.gameObject.SetActive(enable);
            if (enable)
            {
                if (_showTotalValues)
                {
                    _panelActiveIncome.firstItemValuePanel.SetValueAsCurrency(activeIncome);
                }
                else
                {
                    _panelActiveIncome.firstItemValuePanel.RemoveValue();
                }
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

            panelParent.AddItemValue(investmentType, tabCount);

            foreach (AbstractAsset asset in assets)
            {
                int value = (positive ? 1 : -1) * asset.expectedIncome;
                total += value;
                ItemValuePanel panel = panelParent.AddItemValueAsCurrency(
                    asset.name, tabCount + 1, value, !positive);
                panel.clickAction = () => asset.OnDetail(player, reloadWindow);
            }
        }

        private void AddPassiveIncome()
        {
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
                _panelPassiveIncome.AddItemValue("Liquid Assets", baseTabCount);
                foreach (PurchasedStock stock in stocks)
                {
                    int income = stock.expectedIncome;
                    passiveIncome += income;
                    ItemValuePanel panel = _panelPassiveIncome.AddItemValueAsCurrency(
                        stock.stock.longName, baseTabCount + 1, income);
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
            /*
            foreach (AbstractAsset asset in player.portfolio.assets)
            {
                int income = asset.expectedIncome;
                if (income > 0)
                {
                    passiveIncome += income;

                    index = AddItemValueAsCurrency(
                        _panelPassiveIncome.transform.parent,
                        index,
                        _panelPassiveIncome.tabCount + 1,
                        asset.name,
                        income,
                        false,
                        getClickAction(asset, asset.combinedLiability));
                }
            }
            */

            bool enable = _panelPassiveIncome.itemCount > 0;
            _panelPassiveIncome.gameObject.SetActive(enable);
            if (enable)
            {
                if (_showTotalValues)
                {
                    _panelPassiveIncome.firstItemValuePanel.SetValueAsCurrency(passiveIncome);
                }
                else
                {
                    _panelPassiveIncome.firstItemValuePanel.RemoveValue();
                }
            }
        }

        private void AddExpenses()
        {
            int expenses = 0;
            int baseTabCount = _panelExpenses.firstItemValuePanel.tabCount + 1;

            // Personal Expenses
            expenses += player.personalExpenses;
            _panelExpenses.AddItemValueAsCurrency(
                "Personal Expenses", baseTabCount, player.personalExpenses, true);

            if (player.spouse != null && player.spouse.additionalExpense > 0)
            {
                expenses += player.spouse.additionalExpense;
                _panelExpenses.AddItemValueAsCurrency(
                    "Spouse's Expenses", baseTabCount, player.spouse.additionalExpense, true);
            }

            // Child Expenses
            if (player.numChild > 0)
            {
                int childCost = player.numChild * player.costPerChild;
                expenses += childCost;
                _panelExpenses.AddItemValueAsCurrency(
                    "Child Expenses", baseTabCount, childCost, true);
            }

            if (player.portfolio.hasHealthInsurance)
            {
                expenses += PersonalEventManager.Instance.healthInsuranceCost;
                ItemValuePanel panel = _panelExpenses.AddItemValueAsCurrency(
                    "Health Insurance",
                    baseTabCount,
                    PersonalEventManager.Instance.healthInsuranceCost,
                    true);
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
                _panelExpenses.AddItemValue("Other Liabilities", baseTabCount);
                foreach (AbstractLiability liability in liabilities)
                {
                    int expense = liability.expense;
                    if (expense > 0)
                    {
                        expenses += expense;
                        ItemValuePanel panel = _panelExpenses.AddItemValueAsCurrency(
                            liability.shortName, baseTabCount + 1, expense, true);
                        panel.clickAction = () => liability.OnDetail(player, reloadWindow);
                    }
                }
            }

            bool enable = _panelExpenses.itemCount > 0;
            _panelExpenses.gameObject.SetActive(enable);
            if (enable)
            {
                if (_showTotalValues)
                {
                    _panelExpenses.firstItemValuePanel.SetValueAsCurrency(expenses, true);
                }
                else
                {
                    _panelExpenses.firstItemValuePanel.RemoveValue();
                }
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
            _panelTotalCashflow.SetValueAsCurrency(snapshot.expectedCashflow);

            int fi = (100 * snapshot.expectedPassiveIncome) / snapshot.expectedExpenses;
            _panelFinancialIndependence.SetValue(string.Format("{0}%", fi));
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
