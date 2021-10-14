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
        public override int loanValue => originalPrice;

        public int appraisalPrice { get; private set; }
        public int actualIncome { get; private set; }

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
                int rate = InterestRateManager.Instance.distressedLoanRate;
                AddPrivateLoan(debtPartners, maxPrivateLoanLtv, rate, true);
            }

            label = string.Format("Distressed {0}", label);
            description = string.Format("Distressed {0}", description);
        }
    }
}
