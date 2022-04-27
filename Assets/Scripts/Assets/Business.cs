using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets
{
    public abstract class AbstractBusiness : AbstractInvestment
    {
        public override bool returnCapital => false;
        private int _loanLtv;
        private int _maxLoanLtv;

        protected override void resetLoans()
        {
            ClearPrivateLoan();
            primaryLoan = new BusinessLoan(this, _loanLtv, _maxLoanLtv, false);
        }

        public AbstractBusiness(
            string name,
            int originalPrice,
            int value,
            Vector2Int income,
            int loanLtv,
            int maxLoanLtv)
            : base(name, originalPrice, value, income)
        {
            this.description = description;
            _loanLtv = loanLtv;
            _maxLoanLtv = maxLoanLtv;
            resetLoans();
        }

        public void SetName(string name)
        {
            label = name;
        }
    }
    public class SmallBusiness : AbstractBusiness
    {
        public override string investmentType => "Small Business";

        public SmallBusiness(
            string description,
            int startupCost,
            int minIncome,
            int maxIncome,
            int loanLtv,
            int maxLoanLtv)
            : base(description, startupCost, startupCost, new Vector2Int(minIncome, maxIncome), loanLtv, maxLoanLtv)
        {
        }
    }

    public class Franchise : AbstractBusiness
    {
        public override string investmentType => "Franchise";
        public int franchiseFee { get; private set; }
        public override int value => originalPrice + franchiseFee;
        public override int totalCost => value;

        public Franchise(
            string description,
            int startupCost,
            int franchiseFee,
            int minIncome,
            int maxIncome,
            int loanLtv,
            int maxLoanLtv)
            : base(description, startupCost, startupCost, new Vector2Int(minIncome, maxIncome), loanLtv, maxLoanLtv)
        {
            this.franchiseFee = franchiseFee;
        }

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
                    "Franchise Fee: {0}",
                    local.GetCurrency(franchiseFee)));
            return details;
        }
    }

    public class PublicCompany : AbstractInvestment
    {
        public override bool returnCapital => false;
        public override string investmentType => "Public Company";
        public Startup startup { get; private set; }
        public int originalLoanAmount { get; private set; }
        public int originalInterest { get; private set; }
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

        public PublicCompany(
            Startup startup,
            int value,
            int income)
            : base(startup.description, startup.totalCost, value, new Vector2Int(income, income))
        {
            this.startup = startup;
            label = startup.label;
            originalLoanAmount = startup.combinedLiability.amount;
            originalInterest = startup.accruedDelayedInterest;
            _restructuredLoan = new RestructuredBusinessLoan(startup);
        }

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

    }
}
