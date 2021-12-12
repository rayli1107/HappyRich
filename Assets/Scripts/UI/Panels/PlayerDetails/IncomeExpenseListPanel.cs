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
        private ItemValuePanel _panelActiveIncome;
        [SerializeField]
        private ItemValuePanel _panelPassiveIncome;
        [SerializeField]
        private ItemValuePanel _panelExpenses;
        [SerializeField]
        private ItemValuePanel _prefabItemValuePanel;
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

        private int AddItemValueAsCurrency(
            Transform parentTranform,
            int index,
            int tab,
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

        private void AddActiveIncome()
        {
            int activeIncome = 0;
            int index = _panelActiveIncome.transform.GetSiblingIndex() + 1;

            foreach (ScriptableObjects.Profession income in player.jobs)
            {
                activeIncome += income.salary;

                index = AddItemValueAsCurrency(
                    _panelActiveIncome.transform.parent,
                    index,
                    _panelActiveIncome.tabCount + 1,
                    income.professionName,
                    income.salary,
                    false);
            }

            if (player.spouse != null && player.spouse.additionalIncome > 0)
            {
                activeIncome += player.spouse.additionalIncome;
                index = AddItemValueAsCurrency(
                    _panelActiveIncome.transform.parent,
                    index,
                    _panelActiveIncome.tabCount + 1,
                    "Spouse",
                    player.spouse.additionalIncome,
                    false);
            }

            if (_showTotalValues)
            {
                _panelActiveIncome.setValueAsCurrency(activeIncome);
            }
            else
            {
                _panelActiveIncome.removeValue();
            }
        }

        private void addInvestmentsByType(
            ItemValuePanel panelParent,
            Player player,
            string investmentType,
            List<AbstractAsset> assets,
            int tabCount,
            ref int index,
            ref int total,
            bool positive)
        {
/*            Localization local = Localization.Instance;
            Converter<AbstractAsset, string> convertFn =
                a => string.Format(
                    "{0} - {1}", a.name, local.GetCurrency(a.expectedIncome));
            Debug.LogFormat(
                "{0} Before {1}",
                investmentType,
                string.Join("\n", assets.ConvertAll(convertFn)));*/
            if (positive)
            {
                assets = assets.FindAll(a => a.expectedIncome > 0);
            }
            else
            {
                assets = assets.FindAll(a => a.expectedIncome < 0);
            }
/*
 *Debug.LogFormat(
                "{0} After {1}",
                investmentType,
                string.Join("\n", assets.ConvertAll(convertFn)));*/
            if (assets.Count == 0)
            {
                return;
            }

            index = AddItemValueAsCurrency(
                panelParent.transform.parent,
                index,
                tabCount,
                investmentType,
                0,
                false);

            foreach (AbstractAsset asset in assets)
            {
                int value = (positive ? 1 : -1) * asset.expectedIncome;
                total += value;
                index = AddItemValueAsCurrency(
                    panelParent.transform.parent,
                    index,
                    tabCount + 1,
                    asset.name,
                    value,
                    !positive,
                    () => asset.OnDetail(player, reloadWindow));
            }
        }

        private void AddPassiveIncome()
        {
            int passiveIncome = 0;
            int index = _panelPassiveIncome.transform.GetSiblingIndex() + 1;

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
                index = AddItemValueAsCurrency(
                    _panelPassiveIncome.transform.parent,
                    index,
                    _panelPassiveIncome.tabCount + 1,
                    "Liquid Assets",
                    0,
                    false);
                foreach (PurchasedStock stock in stocks)
                {
                    int income = stock.expectedIncome;
                    passiveIncome += income;
                    index = AddItemValueAsCurrency(
                        _panelPassiveIncome.transform.parent,
                        index,
                        _panelPassiveIncome.tabCount + 2,
                        stock.stock.longName,
                        income,
                        false,
                        () => stock.OnDetail(player, reloadWindow));
                }
            }

            addInvestmentsByType(
                _panelPassiveIncome,
                player,
                "Real Estate",
                player.portfolio.properties.ConvertAll(p => (AbstractAsset)p),
                _panelPassiveIncome.tabCount + 1,
                ref index,
                ref passiveIncome,
                true);
            addInvestmentsByType(
                _panelPassiveIncome,
                player,
                "Businesses",
                player.portfolio.businesses.ConvertAll(p => (AbstractAsset)p),
                _panelPassiveIncome.tabCount + 1,
                ref index,
                ref passiveIncome,
                true);
            addInvestmentsByType(
                _panelPassiveIncome,
                player,
                "Other Assets",
                player.portfolio.otherAssets,
                _panelPassiveIncome.tabCount + 1,
                ref index,
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

            if (_showTotalValues)
            {
                _panelPassiveIncome.setValueAsCurrency(passiveIncome);
            }
            else
            {
                _panelPassiveIncome.removeValue();
            }
        }

        private void AddExpenses()
        {
            int expenses = 0;
            int index = _panelExpenses.transform.GetSiblingIndex() + 1;

            // Personal Expenses
            expenses += player.personalExpenses;
            index = AddItemValueAsCurrency(
                _panelExpenses.transform.parent,
                index,
                _panelExpenses.tabCount + 1,
                "Personal Expenses",
                player.personalExpenses,
                true);

            if (player.spouse != null && player.spouse.additionalExpense > 0)
            {
                expenses += player.spouse.additionalExpense;
                index = AddItemValueAsCurrency(
                    _panelExpenses.transform.parent,
                    index,
                    _panelExpenses.tabCount + 1,
                    "Spouse's Expenses",
                    player.spouse.additionalExpense,
                    true);
            }

            // Child Expenses
            if (player.numChild > 0)
            {
                int childCost = player.numChild * player.costPerChild;
                expenses += childCost;
                index = AddItemValueAsCurrency(
                    _panelExpenses.transform.parent,
                    index,
                    _panelExpenses.tabCount + 1,
                    "Children",
                    childCost,
                    true);
            }

            if (player.portfolio.hasHealthInsurance)
            {
                expenses += PersonalEventManager.Instance.healthInsuranceCost;
                index = AddItemValueAsCurrency(
                    _panelExpenses.transform.parent,
                    index,
                    _panelExpenses.tabCount + 1,
                    "Health Insurance",
                    PersonalEventManager.Instance.healthInsuranceCost,
                    true,
                    () => CancelHealthInsurance.Run(player, reloadWindow));
            }

            // Assets with negative cashflow
            addInvestmentsByType(
                _panelExpenses,
                player,
                "Real Estate",
                player.portfolio.properties.ConvertAll(p => (AbstractAsset)p),
                _panelExpenses.tabCount + 1,
                ref index,
                ref expenses,
                false);
            addInvestmentsByType(
                _panelExpenses,
                player,
                "Businesses",
                player.portfolio.businesses.ConvertAll(p => (AbstractAsset)p),
                _panelExpenses.tabCount + 1,
                ref index,
                ref expenses,
                false);
            addInvestmentsByType(
                _panelExpenses,
                player,
                "Other Assets",
                player.portfolio.otherAssets,
                _panelExpenses.tabCount + 1,
                ref index,
                ref expenses,
                false);
            /*
            foreach (AbstractAsset asset in player.portfolio.assets)
            {
                int income = asset.expectedIncome;
                if (income < 0)
                {
                    expenses -= income;

                    index = AddItemValueAsCurrency(
                        _panelExpenses.transform.parent,
                        index,
                        _panelExpenses.tabCount + 1,
                        asset.name,
                        -1 * income,
                        true,
                        getClickAction(asset, asset.combinedLiability));
                }
            }
            */
            foreach (AbstractLiability liability in player.portfolio.liabilities)
            {
                int expense = liability.expense;
                if (expense > 0)
                {
                    expenses += expense;

                    index = AddItemValueAsCurrency(
                        _panelExpenses.transform.parent,
                        index,
                        _panelExpenses.tabCount + 1,
                        liability.shortName,
                        expense,
                        true,
                        () => liability.OnDetail(player, reloadWindow));
                }
            }
            if (_showTotalValues)
            {
                _panelExpenses.setValueAsCurrency(expenses, true);
            }
            else
            {
                _panelExpenses.removeValue();
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
            _panelTotalCashflow.setValueAsCurrency(snapshot.expectedCashflow);
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
