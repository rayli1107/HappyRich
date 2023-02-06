using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets
{
    [Serializable]
    public class RealEstateData
    {
        [SerializeField]
        private string _templateLabel;
        public string templateLabel => _templateLabel;

        [SerializeField]
        private int _rehabPrice;
        public int rehabPrice => _rehabPrice;

        [SerializeField]
        private int _appraisalPrice;
        public int appraisalPrice => _appraisalPrice;

        [SerializeField]
        private int _unitCount;
        public int unitCount => _unitCount;

        [SerializeField]
        private InvestmentData _investmentData;
        public InvestmentData investmentData => _investmentData;

        public void Initialize(
            string templateLabel,
            int originalPrice,
            int marketValue,
            int annualIncome,
            int unitCount,
            int rehabPrice = 0,
            int appraisalPrice = 0)
        {
            _templateLabel = templateLabel;
            _rehabPrice = rehabPrice;
            _appraisalPrice = appraisalPrice;
            _unitCount = unitCount;

            _investmentData = new InvestmentData();
            _investmentData.Initialize(originalPrice, marketValue, annualIncome, annualIncome);
        }
    }

    public abstract class AbstractRealEstate : AbstractInvestment
    {
        protected RealEstateData _realEstateData { get; private set; }
        public override bool returnCapital => true;
        public RealEstateTemplate template { get; private set; }

        protected void AddMortgage(int maxMortgageLtv)
        {
            if (maxMortgageLtv > 0)
            {
                _realEstateData.investmentData.securedLoan = new AdjustableLoanData();
                _realEstateData.investmentData.securedLoan.Initialize(
                    maxMortgageLtv, maxMortgageLtv);
                setupSecuredLoan();
            }
        }

        protected void setupSecuredLoan()
        {
            if (_realEstateData.investmentData.securedLoan != null)
            {
                primaryLoan = new Mortgage(
                    this, _realEstateData.investmentData.securedLoan, false);
            }
        }


        public AbstractRealEstate(
            RealEstateTemplate template,
            RealEstateData realEstateData)
            : base(realEstateData.investmentData)
        {
            _realEstateData = realEstateData;
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
