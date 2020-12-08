using Assets;
using System.Collections.Generic;


namespace UI.Panels
{
    public class IncomeExpenseListPanel : TextListScrollablePanel
    {
        protected override void OnEnable()
        {
            base.OnEnable();

            Localization local = GameManager.Instance.Localization;
            List<string> activeIncomeList = new List<string>();
            List<string> passiveIncomeList = new List<string>();
            List<string> expenseList = new List<string>();
            int activeIncome = 0;
            int passiveIncome = 0;
            int expenses = 0;

            foreach (ScriptableObjects.Profession income in player.jobs)
            {
                activeIncome += income.salary;
                activeIncomeList.Add(
                    string.Format("  {0}: {1}", income.name, local.GetCurrency(income.salary)));
            }

            expenses += player.personalExpenses;
            expenseList.Add(
                string.Format("  Personal Expenses: {0}", local.GetCurrency(player.personalExpenses)));
            if (player.numChild > 0)
            {
                int childCost = player.numChild * player.costPerChild;
                expenses += childCost;
                expenseList.Add(string.Format("  Children: {0}", local.GetCurrency(childCost)));
            }

            foreach (AbstractAsset asset in player.portfolio.assets)
            {
                int income = asset.getIncome();
                if (income > 0)
                {
                    passiveIncome += income;
                    passiveIncomeList.Add(
                        string.Format("  {0}: {1}", asset.name, local.GetCurrency(income)));
                }
                else if (income < 0)
                {
                    expenses -= income;
                    expenseList.Add(
                        string.Format("  {0}: {1}", asset.name, local.GetCurrency(-1 * income)));
                }
            }

            foreach (AbstractLiability liability in player.portfolio.liabilities)
            {
                int expense = liability.getExpense();
                if (expense > 0)
                {
                    expenses += expense;
                    expenseList.Add(
                        string.Format("  {0}: {1}", liability.name, local.GetCurrency(expense)));
                }
            }

            int cashflow = activeIncome + passiveIncome - expenses;
            AddText(string.Format("Total Cashflow: {0}", local.GetCurrency(cashflow)));

            AddText(string.Format("Active Income: {0}", local.GetCurrency(activeIncome)));
            foreach (string s in activeIncomeList)
            {
                AddText(s);
            }

            AddText(string.Format("Passive Income: {0}", local.GetCurrency(passiveIncome)));
            foreach (string s in passiveIncomeList)
            {
                AddText(s);
            }

            AddText(string.Format("Expenses: {0}", local.GetCurrency(expenses)));
            foreach (string s in expenseList)
            {
                AddText(s);
            }
        }
    }
}
