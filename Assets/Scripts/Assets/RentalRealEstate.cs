﻿using System.Collections.Generic;
using UnityEngine;

namespace Assets
{
    public class RentalRealEstate : AbstractRealEstate
    {
        public override string investmentType => "Rental Real Estate";

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
            primaryLoan = new Mortgage(this, mortgageLtv, maxMortgageLtv, false); ;
        }
    }
    
    public class RefinancedRealEstate : RentalRealEstate
    {
        public DistressedRealEstate distressedAsset { get; private set; }
        public int originalLoanAmount { get; private set; }
        public int originalTotalCost { get; private set; }

        public int returnedCapital => Mathf.Max(
            combinedLiability.amount - originalLoanAmount, 0);
        public override int loanValue => value;

        public RefinancedRealEstate(
            DistressedRealEstate distressedAsset,
            List<InvestmentPartner> debtPartners,
            int maxMortgageLtv,
            int maxPrivateLoanLtv)
            : base(distressedAsset.template,
                   distressedAsset.totalCost,
                   distressedAsset.appraisalPrice,
                   distressedAsset.actualIncome,
                   0,
                   maxMortgageLtv,
                   distressedAsset.unitCount)
        {
            this.distressedAsset = distressedAsset;
            originalTotalCost = distressedAsset.totalCost;
            originalLoanAmount = distressedAsset.combinedLiability.amount;
            distressedAsset.ClearPrivateLoan();

            primaryLoan.setMinimumLoanAmount(originalLoanAmount);
            primaryLoan.ltv = maxMortgageLtv;

            int remainingLoanAmount = Mathf.Max(originalLoanAmount - primaryLoan.amount, 0);
            if (remainingLoanAmount > 0)
            {
                AddPrivateLoan(debtPartners, maxPrivateLoanLtv);
                privateLoan.setMinimumLoanAmount(remainingLoanAmount);
            }

            Debug.LogFormat(
                "Refinance mortgage ltv {0} private loan ltv {1}",
                primaryLoan.ltv,
                privateLoan == null ? 0 : privateLoan.ltv);
            Debug.LogFormat(
                "Refinance income {0} {1}",
                distressedAsset.actualIncome,
                income);
        }

        public override List<string> getPurchaseDetails()
        {
            return distressedAsset.getPurchaseDetails();
        }

    }
}
