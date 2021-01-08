using System.Collections.Generic;

namespace Assets
{

    public class RentalRealEstate : AbstractRealEstate
    {
        public Mortgage mortgage { get; private set; }

        public override List<AbstractLiability> liabilities
        {
            get
            {
                List<AbstractLiability> ret = new List<AbstractLiability>();
                ret.Add(mortgage);
                ret.AddRange(privateLoans);
                return ret;
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
        }
    }
}
