using InvestmentPartnerInfo;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets
{
    public partial class RealEstateData
    {
        [SerializeField]
        private int _originalTotalCost;
        public int originalTotalCost => _originalTotalCost;

        [SerializeField]
        private int _originalLoanAmount;
        public int originalLoanAmount => _originalLoanAmount;


        public void InitializeRentalRealEstateData(
            string templateLabel,
            int originalPrice,
            int marketValue,
            int annualIncome,
            int unitCount)
        {
            initialize(templateLabel, originalPrice, marketValue, annualIncome, unitCount);
        }

        public void InitializeRefinancedRealEstate(DistressedRealEstate distressedAsset)
        {
            initialize(
                distressedAsset.template.label,
                distressedAsset.totalCost,
                distressedAsset.appraisalPrice,
                distressedAsset.actualIncome,
                distressedAsset.unitCount);

            _originalTotalCost = distressedAsset.totalCost;
            _originalLoanAmount = distressedAsset.combinedLiability.amount;
            _investmentData.purchaseDetails = distressedAsset.getPurchaseDetails();
        }
    }

    public class RentalRealEstate : AbstractRealEstate
    {
        public override string investmentType => "Rental Real Estate";

        protected int _maxMortgageLtv;

        protected override void resetLoans()
        {
            ClearPrivateLoan();
            AddMortgage(_maxMortgageLtv);
        }

        public RentalRealEstate(
            RealEstateTemplate template,
            RealEstateData data,
            int maxMortgateLtv)
            : base(template, data)
        {
            _maxMortgageLtv = maxMortgateLtv;
            setupSecuredLoan();
        }
    }
    
    public class RefinancedRealEstate : RentalRealEstate
    {
//        public DistressedRealEstate distressedAsset { get; private set; }
        public int originalLoanAmount => realEstateData.originalLoanAmount;
        public int originalTotalCost => realEstateData.originalTotalCost;

        public int returnedCapital => Mathf.Max(
            combinedLiability.amount - originalLoanAmount, 0);
        public override int loanValue => value;

        private int _maxPrivateLoanLtv;
        private List<InvestmentPartner> _debtPartners;

        protected override void resetLoans()
        {
            base.resetLoans();

            primaryLoan.setMinimumLoanAmount(originalLoanAmount);
            primaryLoan.ltv = _maxMortgageLtv;

            int remainingLoanAmount = Mathf.Max(originalLoanAmount - primaryLoan.amount, 0);
            if (remainingLoanAmount > 0)
            {
                AddPrivateLoan(_debtPartners, _maxPrivateLoanLtv);
                privateLoan.setMinimumLoanAmount(remainingLoanAmount);
            }
        }

        public RefinancedRealEstate(
            DistressedRealEstate distressedAsset,
            RealEstateData realEstateData,
            List<InvestmentPartner> debtPartners,
            int maxMortgageLtv,
            int maxPrivateLoanLtv)
            : base(distressedAsset.template, realEstateData, maxMortgageLtv)
/*                  
                  distressedAsset.template,
                   distressedAsset.totalCost,
                   distressedAsset.appraisalPrice,
                   distressedAsset.actualIncome,
                   0,
                   maxMortgageLtv,
                   distressedAsset.unitCount)*/
        {
//            this.distressedAsset = distressedAsset;
//            originalTotalCost = distressedAsset.totalCost;
//            originalLoanAmount = distressedAsset.combinedLiability.amount;
//            distressedAsset.ClearPrivateLoan();
//           _maxMortgageLtv = maxMortgageLtv;
            _maxPrivateLoanLtv = maxPrivateLoanLtv;
            _debtPartners = debtPartners;

//            resetLoans();

            setupSecuredLoan();
            setupPrivateLoan(null);

            Debug.LogFormat(
                "Refinance mortgage ltv {0} private loan ltv {1}",
                primaryLoan.ltv,
                privateLoan == null ? 0 : privateLoan.ltv);
            Debug.LogFormat(
                "Refinance income {0} {1}",
                distressedAsset.actualIncome,
                Localization.Instance.GetIncomeRange(netIncomeRange));
        }
/*
        public override List<string> getPurchaseDetails()
        {
            return distressedAsset.getPurchaseDetails();
        }
*/
    }
}
