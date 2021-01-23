using System.Collections.Generic;
using UnityEngine;

namespace Assets
{
    public class RentalRealEstate : AbstractRealEstate
    {
        public RentalRealEstate(
            RealEstateTemplate template,
            int purchasePrice,
            int marketValue,
            int annualIncome,
            int mortgageLtv,
            int maxMortgageLtv,
            int unitCount)
            : base(template, purchasePrice, marketValue, annualIncome, unitCount)
        {
            mortgage = new Mortgage(this, mortgageLtv, maxMortgageLtv);
        }
    }
    
    public class RefinancedRealEstate : RentalRealEstate
    {
        public DistressedRealEstate distressedAsset { get; private set; }
        public int originalLoanAmount { get; private set; }

        public int returnedCapital => Mathf.Max(
            combinedLiability.amount - originalLoanAmount, 0);

        public RealEstatePrivateLoan mandatoryPrivateLoan { get; private set; }

        public override List<AbstractLiability> liabilities
        {
            get
            {
                List<AbstractLiability> ret = base.liabilities;
                if (mandatoryPrivateLoan != null)
                {
                    ret.Add(mandatoryPrivateLoan);
                }
                return ret;
            }
        }

        public RefinancedRealEstate(
            DistressedRealEstate distressedAsset,
            List<InvestmentPartner> debtPartners,
            int maxMortgageLtv,
            int maxPrivateLoanLtv)
            : base(distressedAsset.template,
                   distressedAsset.value,
                   distressedAsset.appraisalPrice,
                   distressedAsset.actualIncome,
                   0,
                   maxMortgageLtv,
                   distressedAsset.unitCount)
        {
            this.distressedAsset = distressedAsset;
            originalLoanAmount = distressedAsset.combinedLiability.amount;
            distressedAsset.ClearPrivateLoans();

            mortgage.setLoanAmount(originalLoanAmount);

            int remainingLoanAmount = Mathf.Max(originalLoanAmount - mortgage.amount, 0);
            if (remainingLoanAmount > 0)
            {
                int rate = InterestRateManager.Instance.defaultPrivateLoanRate;
                mandatoryPrivateLoan = new RealEstatePrivateLoan(
                    this, debtPartners, maxPrivateLoanLtv, rate, false);
                mandatoryPrivateLoan.setLoanAmount(remainingLoanAmount);
            }
        }
    }
}
