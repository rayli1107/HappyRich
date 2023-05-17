using InvestmentPartnerInfo;
using System.Collections.Generic;
using UnityEngine;

namespace Assets
{
    public partial class RealEstateData
    {
        [SerializeField]
        private int _rehabPrice;
        public int rehabPrice => _rehabPrice;

        [SerializeField]
        private int _appraisalPrice;
        public int appraisalPrice => _appraisalPrice;

        public void InitializeDistressedRealEstateData(
            string templateLabel,
            int originalPrice,
            int marketValue,
            int annualIncome,
            int unitCount,
            int rehabPrice,
            int appraisalPrice)
        {
            initialize(templateLabel, originalPrice, marketValue, annualIncome, unitCount);
            _rehabPrice = rehabPrice;
            _appraisalPrice = appraisalPrice;
        }
    }

    public class DistressedRealEstate : AbstractRealEstate
    {
        public override string investmentType => "Distressed Real Estate";
//        public int rehabPrice { get; private set; }

        public override int value =>
            originalPrice + realEstateData.rehabPrice + delayedInterest;
        public override int totalCost => value;
        public int appraisalPrice { get; private set; }
        public int actualIncome { get; private set; }
        protected override bool _isDebtInterestDelayed => true;
        protected override int _privateLoanRate =>
            InterestRateManager.Instance.distressedLoanRate;
        public int rehabPrice => realEstateData.rehabPrice;

        private int _maxMortgageLtv;
        private int _maxPrivateLoanLtv;
        private List<InvestmentPartner> _debtPartners;

        protected override void resetLoans()
        {
            ClearPrivateLoan();

            if (_maxMortgageLtv > 0)
            {
                AddMortgage(_maxMortgageLtv);
            }
            else if (_maxPrivateLoanLtv > 0)
            {
                AddPrivateLoan(_debtPartners, _maxPrivateLoanLtv);
            }
        }

        public DistressedRealEstate(
            RealEstateTemplate template,
            RealEstateData realEstateData,
            List<InvestmentPartner> debtPartners,
            int maxMortgageLtv,
            int maxPrivateLoanLtv)
            : base(template, realEstateData)
        {
            _maxMortgageLtv = maxMortgageLtv;
            _maxPrivateLoanLtv = maxPrivateLoanLtv;
            _debtPartners = debtPartners;
            label = string.Format("Distressed {0}", label);
            description = string.Format("Distressed {0}", description);
            setupSecuredLoan();
            setupPrivateLoan(null);
        }

        public override List<string> getPurchaseDetails()
        {
            Localization local = Localization.Instance;
            List<string> details = new List<string>();
            details.Add(
                string.Format(
                    "Original Price: {0}",
                    local.GetCurrency(originalPrice)));
            details.Add(
                string.Format(
                    "Rehab Price: {0}",
                    local.GetCurrency(realEstateData.rehabPrice)));
            int interest = delayedInterest;
            if (interest > 0)
            {
                details.Add(
                    string.Format(
                        "Interest Fee: {0}",
                        local.GetCurrency(interest)));
            }
            return details;
        }
    }
}
