using System.Collections.Generic;

namespace Assets
{
    public class DistressedRealEstate : AbstractRealEstate
    {
        public override int downPayment =>
            purchasePrice + rehabPrice + privateLoanDelayedPayment - privateLoanAmount;
        public override int totalIncome => 0;
        public override string label =>
            string.Format("Distressed {0}", base.label);
        public override string description =>
            string.Format("Distressed {0}", base.description);

        public int rehabPrice { get; private set; }
        public int appraisalPrice { get; private set; }
        public int actualIncome { get; private set; }
        public override List<AbstractLiability> liabilities
        {
            get
            {
                List<AbstractLiability> ret = new List<AbstractLiability>();
                ret.AddRange(privateLoans);
                return ret;
            }
        }

        public DistressedRealEstate(
            RealEstateTemplate template,
            int purchasePrice,
            int rehabPrice,
            int appraisalPrice,
            int annualIncome,
            int unitCount)
            : base(template, purchasePrice, purchasePrice, 0, unitCount)
        {
            this.rehabPrice = rehabPrice;
            this.appraisalPrice = appraisalPrice;
            actualIncome = annualIncome;
        }
    }
}
