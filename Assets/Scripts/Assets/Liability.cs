using UnityEngine;

namespace Assets
{
    public class AbstractLiability
    {
        public string name { get; private set; }
        public virtual int amount { get; private set; }
        public int interestRate { get; private set; }
        public virtual int expense => amount * interestRate / 100;

        public AbstractLiability(
            string name, int amount, int interestRate)
        {
            this.name = name;
            this.amount = amount;
            this.interestRate = interestRate;
        }

        public virtual int PayOff(int payment)
        {
            payment = Mathf.Min(payment, amount);
            amount -= payment;
            return payment;
        }

        public void Add(int amount)
        {
            this.amount += amount;
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

    public class PersonalLoan : AbstractLiability
    {
        public PersonalLoan(int amount) :
            base("Personal Loan", amount, InterestRateManager.Instance.personalLoanRate)
        {
        }
    }

    public class PrivateLoan : AbstractLiability
    {
        private InvestmentPartner _partner;

        public PrivateLoan(InvestmentPartner partner, int amount, int interestRate) :
            base("Private Loan", amount, interestRate)
        {
            _partner = partner;
            _partner.cash -= amount;
        }

        public override int PayOff(int payment)
        {
            payment = base.PayOff(payment);
            _partner.cash += payment;
            return payment;
        }
    }

}
