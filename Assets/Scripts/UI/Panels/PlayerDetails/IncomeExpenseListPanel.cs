using Assets;
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

        private int AddItemValueAsCurrency(
            Transform parentTranform,
            int index,
            int tab,
            string label,
            int value,
            bool flip)
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

            if (_showTotalValues)
            {
                _panelActiveIncome.setValueAsCurrency(activeIncome);
            }
            else
            {
                _panelActiveIncome.removeValue();
            }
        }

        private void AddPassiveIncome()
        {
            int passiveIncome = 0;
            int index = _panelPassiveIncome.transform.GetSiblingIndex() + 1;

            foreach (AbstractAsset asset in player.portfolio.assets)
            {
                int income = asset.getIncome();
                if (income > 0)
                {
                    passiveIncome += income;

                    index = AddItemValueAsCurrency(
                        _panelPassiveIncome.transform.parent,
                        index,
                        _panelPassiveIncome.tabCount + 1,
                        asset.name,
                        income,
                        false);
                }
            }

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

            // Assets with negative cashflow
            foreach (AbstractAsset asset in player.portfolio.assets)
            {
                int income = asset.getIncome();
                if (income < 0)
                {
                    expenses -= income;

                    index = AddItemValueAsCurrency(
                        _panelExpenses.transform.parent,
                        index,
                        _panelExpenses.tabCount + 1,
                        asset.name,
                        -1 * income,
                        true);
                }
            }

            foreach (AbstractLiability liability in player.portfolio.liabilities)
            {
                int expense = liability.getExpense();
                if (expense > 0)
                {
                    expenses += expense;

                    index = AddItemValueAsCurrency(
                        _panelExpenses.transform.parent,
                        index,
                        _panelExpenses.tabCount + 1,
                        liability.name,
                        expense,
                        true);
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

            PlayerSnapshot snapshot = new PlayerSnapshot(player);
            _panelTotalCashflow.setValueAsCurrency(snapshot.cashflow);
        }

        private void OnEnable()
        {
            RefreshContent();
        }
    }
}
