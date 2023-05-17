using System;
using UnityEngine;

namespace Assets
{
    [Serializable]
    public partial class RealEstateData
    {
        [SerializeField]
        private string _templateLabel;
        public string templateLabel => _templateLabel;

        [SerializeField]
        private int _unitCount;
        public int unitCount => _unitCount;

        [SerializeField]
        private InvestmentData _investmentData;
        public InvestmentData investmentData => _investmentData;

        private void initialize(
            string templateLabel,
            int originalPrice,
            int marketValue,
            int annualIncome,
            int unitCount)
        {
            _templateLabel = templateLabel;
            _unitCount = unitCount;

            _investmentData = new InvestmentData();
            _investmentData.Initialize(originalPrice, marketValue, annualIncome, annualIncome);
        }
    }

    public abstract class AbstractRealEstate : AbstractInvestment
    {
        public RealEstateData realEstateData { get; private set; }
        public override bool returnCapital => true;
        public RealEstateTemplate template { get; private set; }
        public int unitCount => realEstateData.unitCount;

        protected void AddMortgage(int maxMortgageLtv)
        {
            if (maxMortgageLtv > 0)
            {
                realEstateData.investmentData.securedLoan = new AdjustableLoanData();
                realEstateData.investmentData.securedLoan.Initialize(
                    maxMortgageLtv,
                    maxMortgageLtv,
                    InvestmentPartnerManager.Instance.partnerCount);
                setupSecuredLoan();
            }
        }

        protected void setupSecuredLoan()
        {
            if (realEstateData.investmentData.securedLoan != null)
            {
                primaryLoan = new Mortgage(
                    this, realEstateData.investmentData.securedLoan, false);
            }
        }


        public AbstractRealEstate(
            RealEstateTemplate template,
            RealEstateData realEstateData)
            : base(realEstateData.investmentData)
        {
            this.realEstateData = realEstateData;
            this.template = template;

            label = realEstateData.unitCount > 1 ?
                string.Format(template.label, realEstateData.unitCount) :
                template.label;
            description = realEstateData.unitCount > 1 ?
                string.Format(template.description, realEstateData.unitCount) :
                template.description;
        }
    }
}
