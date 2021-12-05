using System.Collections.Generic;
using UnityEngine;

namespace Assets
{
    public class DistressedRealEstate : AbstractRealEstate
    {
        public int rehabPrice { get; private set; }

        public override int value =>
            originalPrice + rehabPrice + delayedInterest;
        public override int totalCost => value;
        public int appraisalPrice { get; private set; }
        public int actualIncome { get; private set; }
        protected override bool _isDebtInterestDelayed => true;
        protected override int _privateLoanRate =>
            InterestRateManager.Instance.distressedLoanRate;

        public DistressedRealEstate(
            RealEstateTemplate template,
            List<InvestmentPartner> debtPartners,
            int purchasePrice,
            int rehabPrice,
            int appraisalPrice,
            int annualIncome,
            int unitCount,
            int maxMortgageLtv,
            int maxPrivateLoanLtv)
            : base(template, purchasePrice, 0, 0, unitCount)
        {
            this.rehabPrice = rehabPrice;
            this.appraisalPrice = appraisalPrice;
            actualIncome = annualIncome;
            
            if (maxMortgageLtv > 0)
            {
                primaryLoan = new Mortgage(this, maxMortgageLtv, maxMortgageLtv, true);
            }
            else
            {
                AddPrivateLoan(debtPartners, maxPrivateLoanLtv);
            }

            label = string.Format("Distressed {0}", label);
            description = string.Format("Distressed {0}", description);
        }
    }
}
