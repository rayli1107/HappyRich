using System.Collections.Generic;

namespace Assets
{

    public class RentalRealEstate : AbstractRealEstate
    {
        public Mortgage mortgage { get; private set; }

        private List<PrivateLoan> _privateLoans;
        public override List<AbstractLiability> liabilities
        {
            get
            {
                List<AbstractLiability> ret = new List<AbstractLiability>();
                ret.Add(mortgage);
                ret.AddRange(_privateLoans);
                return ret;
            }
        }

        public int privateLoanAmount
        {
            get
            {
                int amount = 0;
                foreach (PrivateLoan loan in _privateLoans)
                {
                    amount += loan.amount;
                }
                return amount;
            }
        }

        public int privateLoanPayment
        {
            get
            {
                int amount = 0;
                foreach (PrivateLoan loan in _privateLoans)
                {
                    amount += loan.expense;
                }
                return amount;
            }
        }

        public RentalRealEstate(
            RealEstateTemplate template,
            int purchasePrice,
            int marketPrice,
            int annualIncome,
            int ltv,
            int unitCount)
            : base(template, purchasePrice, marketPrice, annualIncome, unitCount)
        {
            mortgage = new Mortgage(this, ltv);
            _privateLoans = new List<PrivateLoan>();
        }

        public void AddPrivateLoan(PrivateLoan loan)
        {
            _privateLoans.Add(loan);
        }

        public void ClearPrivateLoans()
        {
            foreach (PrivateLoan loan in _privateLoans)
            {
                loan.PayOff(loan.amount);
            }
            _privateLoans.Clear();
        }

        public override void OnPurchaseCancel()
        {
            base.OnPurchaseCancel();
            ClearPrivateLoans();
        }
    }
}
