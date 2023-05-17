using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets
{
    [Serializable]
    public class BusinessData
    {
        [SerializeField]
        private InvestmentData _investmentData;
        public InvestmentData investmentData => _investmentData;

        [SerializeField]
        private string _description;
        public string description => _description;

        public string label;

        [SerializeField]
        private int _franchiseFee;
        public int franchiseFee => _franchiseFee;

        [SerializeField]
        private int _restructuredLoanAmount;
        public int restructuredLoanAmount => _restructuredLoanAmount;

        private void initialize(
            string description,
            int originalPrice,
            int value,
            int incomeRangeLow,
            int incomeRangeHigh)
        {
            _investmentData.Initialize(originalPrice, value, incomeRangeLow, incomeRangeHigh);

            _description = description;
            label = description;
        }

        public void InitializeSmallBusiness(
            string description,
            int originalPrice,
            int value,
            int incomeRangeLow,
            int incomeRangeHigh)
        {
            initialize(description, originalPrice, value, incomeRangeLow, incomeRangeHigh);
        }

        public void InitializeFranchise(
            string description,
            int originalPrice,
            int franchiseFee,
            int value,
            int incomeRangeLow,
            int incomeRangeHigh)
        {
            initialize(description, originalPrice, value, incomeRangeLow, incomeRangeHigh);
            _franchiseFee = franchiseFee;

            Localization local = Localization.Instance;
            investmentData.purchaseDetails = new List<string>();
            investmentData.purchaseDetails.Add(
                string.Format(
                    "Startup Cost: {0}",
                    local.GetCurrency(originalPrice)));
            investmentData.purchaseDetails.Add(
                string.Format(
                    "Franchise Fee: {0}",
                    local.GetCurrency(franchiseFee)));
        }

        public void InitializePublicCompany(Startup startup, int value, int income)
        {
            int originalPrice = startup.totalCost;
            int originalLoanAmount = startup.combinedLiability.amount;
            int originalLoanInterest = startup.accruedDelayedInterest;
            initialize(startup.description, originalPrice, value, income, income);

            _description = startup.description;
            label = startup.label;
            _restructuredLoanAmount = originalLoanAmount + originalLoanInterest;

            Localization local = Localization.Instance;
            investmentData.purchaseDetails = new List<string>();
            investmentData.purchaseDetails.Add(
                string.Format(
                    "Startup Cost: {0}",
                    local.GetCurrency(originalPrice)));
            investmentData.purchaseDetails.Add(
                string.Format(
                    "Total Startup Loan Interest : {0}",
                    local.GetCurrency(originalLoanInterest, true)));
        }
    }

    public abstract class AbstractBusiness : AbstractInvestment
    {
        protected BusinessData _businessData;

        public override string label => _businessData.label;
        public override string description => _businessData.description;

        public override bool returnCapital => false;
        private int _defaultLoanLtv;
        private int _maxLoanLtv;

        protected void AddBusinessLoan()
        {
            if (_maxLoanLtv > 0)
            {
                _businessData.investmentData.securedLoan = new AdjustableLoanData();
                _businessData.investmentData.securedLoan.Initialize(
                    _defaultLoanLtv,
                    _maxLoanLtv,
                    InvestmentPartnerManager.Instance.partnerCount);
                setupSecuredLoan();
            }
        }

        protected void setupSecuredLoan()
        {
            if (_businessData.investmentData.securedLoan != null)
            {
                primaryLoan = new BusinessLoan(
                    this, _businessData.investmentData.securedLoan, false);
            }
        }

        protected override void resetLoans()
        {
            ClearPrivateLoan();
            AddBusinessLoan();
        }

        public AbstractBusiness(
            BusinessData businessData,
            int defaultLoanLtv,
            int maxLoanLtv)
            : base(businessData.investmentData)
        {
            _businessData = businessData;
            _defaultLoanLtv = defaultLoanLtv;
            _maxLoanLtv = maxLoanLtv;
            setupSecuredLoan();
        }

        public void SetName(string name)
        {
            _businessData.label = name;
        }
    }
    public class SmallBusiness : AbstractBusiness
    {
        public override string investmentType => "Small Business";

        public SmallBusiness(
            BusinessData businessData, int loanLtv, int maxLoanLtv)
            : base(businessData, loanLtv, maxLoanLtv)
        {
        }
    }

    public class Franchise : AbstractBusiness
    {
        public override string investmentType => "Franchise";
        public int franchiseFee => _businessData.franchiseFee;
        public override int value => originalPrice + franchiseFee;
        public override int totalCost => value;

        public Franchise(
            BusinessData businessData, int loanLtv, int maxLoanLtv)
            : base(businessData, loanLtv, maxLoanLtv)
        {
        }
    }

    public class PublicCompany : AbstractInvestment
    {
        private BusinessData _businessData;
        public override bool returnCapital => false;
        public override string investmentType => "Public Company";
        public override string label => _businessData.label;
/*        public Startup startup { get; private set; }
        public int originalLoanAmount { get; private set; }
        public int originalInterest { get; private set; }
*/
        public override int loanValue => value;

        private RestructuredBusinessLoan _restructuredLoan;

        public override List<AbstractSecuredLoan> securedLoans
        {
            get
            {
                List<AbstractSecuredLoan> loans = base.securedLoans;
                if (_restructuredLoan != null)
                {
                    loans.Add(_restructuredLoan);
                }
                return loans;
            }
        }

        public PublicCompany(BusinessData data) 
            : base(data.investmentData)
        {
            _businessData = data;
/*
            this.startup = startup;
            label = startup.label;
            originalLoanAmount = startup.combinedLiability.amount;
            originalInterest = startup.accruedDelayedInterest;
 */
            if (_businessData.restructuredLoanAmount > 0)
            {
                _restructuredLoan = new RestructuredBusinessLoan(
                    this, _businessData.restructuredLoanAmount);
            }
//            startup.ClearPrivateLoan();
        }
        /*
        public override List<string> getPurchaseDetails()
        {
            Localization local = Localization.Instance;
            List<string> details = new List<string>();
            details.Add(
                string.Format(
                    "Startup Cost: {0}",
                    local.GetCurrency(originalPrice)));
            details.Add(
                string.Format(
                    "Total Startup Loan Interest : {0}",
                    local.GetCurrency(originalInterest, true)));
            return details;
        }
        */
    }
}
