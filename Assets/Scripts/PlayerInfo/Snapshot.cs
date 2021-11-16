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
        public int expectedPassiveIncome { get; private set; }
        public int actualPassiveIncome { get; private set; }
        public int expectedExpenses { get; private set; }
        public int actualExpenses { get; private set; }
        public int cash => player.cash;
        public int happiness => player.happiness;

        public int netWorth { get; private set; }


        public int expectedCashflow => activeIncome + expectedPassiveIncome - expectedExpenses;
        public int actualCashflow => activeIncome + actualPassiveIncome - actualExpenses;

        public int availablePersonalLoanAmount
        {
            get
            {
                int amount = expectedCashflow / InterestRateManager.Instance.personalLoanRate * 100;
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

            expectedPassiveIncome = 0;
            actualPassiveIncome = 0;

            int expenses = player.personalExpenses;
            if (player.spouse != null)
            {
                expenses += player.spouse.additionalExpense;
            }
            expenses += player.numChild * player.costPerChild;
            expenses += player.portfolio.hasHealthInsurance ? PersonalEventManager.Instance.healthInsuranceCost : 0;

            expectedExpenses = expenses;
            actualExpenses = expenses;

            foreach (AbstractAsset asset in player.portfolio.assets)
            {
                int income = asset.expectedIncome;
                if (income > 0)
                {
                    expectedPassiveIncome += income;
                }
                else
                {
                    expectedExpenses -= income;
                }

                income = asset.income;
                if (income > 0)
                {
                    actualPassiveIncome += income;
                }
                else
                {
                    actualExpenses -= income;
                }

                netWorth += asset.value;
                netWorth -= asset.combinedLiability.amount;
            }

            foreach (AbstractLiability liability in player.portfolio.liabilities)
            {
                netWorth -= liability.amount;
                expectedExpenses += liability.expense;
                actualExpenses += liability.expense;
            }
        }
    }
}
