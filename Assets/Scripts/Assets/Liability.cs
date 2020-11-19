using UnityEngine;

namespace Assets
{
    public class AbstractLiability
    {
        public string name { get; private set; }
        public int amount { get; private set; }
        public int interestRate { get; private set; }

        public AbstractLiability(
            string name, int amount, int interestRate)
        {
            this.name = name;
            this.amount = amount;
            this.interestRate = interestRate;
        }

        public virtual int getExpense()
        {
            return amount * interestRate / 100;
        }

        public virtual void payoff(int amount)
        {
            this.amount = Mathf.Max(this.amount - amount, 0);
        }
    }

    public class AutoLoan : AbstractLiability
    {
        public AutoLoan(int amount) :
            base ("Auto Loan", amount, InterestRateManager.Instance.autoLoanRate)
        {            
        }
    }

    public class StudentLoan : AbstractLiability
    {
        public StudentLoan(int amount) :
            base("Student Loan", amount, InterestRateManager.Instance.studentLoanRate)
        {
        }
    }

}
