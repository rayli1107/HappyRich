using Assets;
using ScriptableObjects;
using UnityEngine;

namespace PlayerInfo
{
    public class Snapshot
    {
        public Player player { get; private set; }
        public int age => player.age;
        public int activeIncome { get; private set; }
        public int passiveIncome { get; private set; }
        public int expenses { get; private set; }
        public int cash => player.cash;
        public int happiness => player.happiness;

        public int netWorth { get; private set; }

        public int cashflow => activeIncome + passiveIncome - expenses;
        public int availablePersonalLoanAmount
        {
            get
            {
                int amount = cashflow / InterestRateManager.Instance.personalLoanRate * 100;
                if (player.portfolio.personalLoan != null)
                {
                    amount -= player.portfolio.personalLoan.amount;
                }
                return Mathf.Max(amount, 0);
            }
        }

        public Snapshot(Player player)
        {
            this.player = player;

            netWorth = player.cash;

            activeIncome = 0;
            if (player.spouse != null)
            {

                activeIncome += player.spouse.additionalIncome;
            }
            foreach (Profession job in player.jobs)
            {
                activeIncome += job.salary;
            }

            passiveIncome = 0;

            expenses = player.personalExpenses;
            if (player.spouse != null)
            {
                expenses += player.spouse.additionalExpense;
            }
            expenses += player.numChild * player.costPerChild;

            foreach (AbstractAsset asset in player.portfolio.assets)
            {
                int income = asset.income;
                if (income > 0)
                {
                    passiveIncome += income;
                }
                else
                {
                    expenses -= income;
                }
                netWorth += asset.value;
                netWorth -= asset.combinedLiability.amount;
            }

            foreach (AbstractLiability liability in player.portfolio.liabilities)
            {
                netWorth -= liability.amount;
                expenses += liability.expense;
            }
        }
    }
}
