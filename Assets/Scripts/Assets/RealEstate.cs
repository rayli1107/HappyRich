using System.Collections.Generic;

namespace Assets
{
    public class Mortgage : AbstractLiability
    {
        public RentalRealEstate asset { get; private set; }
        public int ltv;
        public override int amount => asset.purchasePrice * ltv / 100;

        public Mortgage(RentalRealEstate asset, int defaultLTV) 
            : base(string.Format("Mortgage - {0}", asset.template.label), 0,
                   InterestRateManager.Instance.realEstateLoanRate)
        {
            this.asset = asset;
            ltv = defaultLTV;
        }
    }

    public class AbstractRealEstate : AbstractAsset
    {
        public int purchasePrice { get; private set; }
        public int marketPrice { get; private set; }
        public RealEstateTemplate template { get; private set; }

        public int downPayment
        {
            get
            {
                int downPayment = purchasePrice;
                foreach (AbstractLiability liability in liabilities)
                {
                    downPayment -= liability.amount;
                }
                return downPayment;
            }
        }

        public AbstractRealEstate(
            RealEstateTemplate template,
            int purchasePrice,
            int marketPrice,
            int annualIncome)
            : base(template.label, marketPrice, annualIncome)
        {
            this.template = template;
            this.purchasePrice = purchasePrice;
            this.marketPrice = marketPrice;
        }
    }

    public class RentalRealEstate : AbstractRealEstate
    {
        public Mortgage mortgage { get; private set; }

        public override List<AbstractLiability> liabilities
        {
            get
            {
                List<AbstractLiability> ret = new List<AbstractLiability>();
                ret.Add(mortgage);
                return ret;
            }
        }

        public RentalRealEstate(
            RealEstateTemplate template,
            int purchasePrice,
            int marketPrice,
            int annualIncome,
            int defaultMortgageRate)
            : base(template, purchasePrice, marketPrice, annualIncome)
        {
            mortgage = new Mortgage(this, defaultMortgageRate);
        }
    }
}
