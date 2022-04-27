using System.Collections.Generic;
using UnityEngine;

namespace Assets
{
    public class DistressedRealEstate : AbstractRealEstate
    {
        public override string investmentType => "Distressed Real Estate";
        public int rehabPrice { get; private set; }

        public override int value =>
            originalPrice + rehabPrice + delayedInterest;
        public override int totalCost => value;
        public int appraisalPrice { get; private set; }
        public int actualIncome { get; private set; }
        protected override bool _isDebtInterestDelayed => true;
        protected override int _privateLoanRate =>
            InterestRateManager.Instance.distressedLoanRate;

        private int _maxMortgageLtv;
        private int _maxPrivateLoanLtv;
        private List<InvestmentPartner> _debtPartners;
        protected override void resetLoans()
        {
            ClearPrivateLoan();

            if (_maxMortgageLtv > 0)
            {
                primaryLoan = new Mortgage(
                    this, _maxMortgageLtv, _maxMortgageLtv, true);
            }
            else
            {
                AddPrivateLoan(_debtPartners, _maxPrivateLoanLtv);
            }
        }

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
            _maxMortgageLtv = maxMortgageLtv;
            _maxPrivateLoanLtv = maxPrivateLoanLtv;
            _debtPartners = debtPartners;
            label = string.Format("Distressed {0}", label);
            description = string.Format("Distressed {0}", description);
            resetLoans();
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
                    local.GetCurrency(rehabPrice)));
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
