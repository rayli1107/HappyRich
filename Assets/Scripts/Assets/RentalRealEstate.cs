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
            int ltv,
            int unitCount)
            : base(template, purchasePrice, marketValue, annualIncome, unitCount)
        {
            mortgage = new Mortgage(this, ltv);
        }
    }
    
    public class RefinancedRealEstate : RentalRealEstate
    {
        public DistressedRealEstate distressedAsset { get; private set; }
        public int originalLoanAmount { get; private set; }

        public int returnedCapital => Mathf.Max(
            combinedLiability.amount - originalLoanAmount, 0);

        private List<PrivateLoan> _mandatoryPrivateLoans;

        public RefinancedRealEstate(
            DistressedRealEstate distressedAsset, int ltv, int loanRate)
            : base(distressedAsset.template,
                   distressedAsset.value,
                   distressedAsset.appraisalPrice,
                   distressedAsset.actualIncome,
                   ltv,
                   distressedAsset.unitCount)
        {
            this.distressedAsset = distressedAsset;
            originalLoanAmount = distressedAsset.combinedLiability.amount;

            mortgage.ltv = Mathf.Min(
                ltv, Mathf.CeilToInt((float)originalLoanAmount / value * 100));

            int remainingLoanAmount = originalLoanAmount - mortgage.amount;

            List<InvestmentPartner> lenders = distressedAsset.privateLenders;
            distressedAsset.ClearPrivateLoans();

            _mandatoryPrivateLoans = new List<PrivateLoan>();
            foreach (InvestmentPartner lender in lenders)
            {
                int amount = Mathf.Min(remainingLoanAmount, lender.cash);
                if (amount > 0)
                {
                    _mandatoryPrivateLoans.Add(
                        new PrivateLoan(lender, amount, loanRate, false));
                }
                remainingLoanAmount -= amount;
            }
        }
    }
}
