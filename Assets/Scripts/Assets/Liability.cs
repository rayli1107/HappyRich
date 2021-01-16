using UnityEngine;

namespace Assets
{
    public class AbstractLiability
    {
        public virtual string name { get; private set; }
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
        public InvestmentPartner partner { get; private set; }
        private bool _delayed;
        public override int expense => _delayed ? 0 : base.expense;
        public int delayedExpense => _delayed ? base.expense : 0;

        public PrivateLoan(
            InvestmentPartner partner, int amount, int interestRate, bool delayed) :
            base("Private Loan", amount, interestRate)
        {
            this.partner = partner;
            partner.cash -= amount;
            _delayed = delayed;
        }

        public override int PayOff(int payment)
        {
            payment = base.PayOff(payment);
            partner.cash += payment;
            return payment;
        }
    }

    public class CombinedLiability : AbstractLiability
    {
        public AbstractAsset asset { get; private set; }
        public override string name => string.Format("Liability - {0}", asset.name);
        public override int amount
        {
            get
            {
                int amount = 0;
                foreach (AbstractLiability liability in asset.liabilities)
                {
                    amount += liability.amount;
                }
                return amount;
            }
        }

        public override int expense
        {
            get
            {
                int expense = 0;
                foreach (AbstractLiability liability in asset.liabilities)
                {
                    expense += liability.expense;
                }
                return expense;
            }
        }

        public CombinedLiability(AbstractAsset asset) : base("", 0, 0)
        {
            this.asset = asset;
        }
    }

    public class PartialLiability : AbstractLiability
    {
        public PartialLiability(AbstractLiability liability, float equity) :
            base(liability.name, Mathf.FloorToInt(liability.amount * equity), liability.interestRate)
        {
        }
    }
}
