using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets
{
    public class AbstractBusiness : AbstractInvestment
    {
        public int minIncome { get; private set; }
        public int maxIncome { get; private set; }

        public AbstractBusiness(
            string description,
            int originalCost,
            int minIncome,
            int maxIncome,
            int actualIncome)
            : base(description, originalCost, originalCost, actualIncome)
        {
            this.description = description;
            this.minIncome = minIncome;
            this.maxIncome = maxIncome;
        }

        public void SetName(string name)
        {
            label = name;
        }
    }

    public class SmallBusiness : AbstractBusiness
    {
        public SmallBusiness(
            string description,
            int startupCost,
            int minIncome,
            int maxIncome,
            int actualIncome,
            int loanLtv,
            int maxLoanLtv)
            : base(description, startupCost, minIncome, maxIncome, actualIncome)
        {
            primaryLoan = new BusinessLoan(this, loanLtv, maxLoanLtv, false);
        }
    }

    public class Franchise : AbstractBusiness
    {
        public int franchiseFee { get; private set; }
        public override int value => originalPrice + franchiseFee;
        public override int totalCost => value;

        public Franchise(
            string description,
            int startupCost,
            int franchiseFee,
            int minIncome,
            int maxIncome,
            int actualIncome,
            int loanLtv,
            int maxLoanLtv)
            : base(description, startupCost, minIncome, maxIncome, actualIncome)
        {
            this.franchiseFee = franchiseFee;
            primaryLoan = new BusinessLoan(this, loanLtv, maxLoanLtv, false);
        }
    }


    public class PublicCompany : AbstractBusiness
    {
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
            : base(startup.description, value, income, income, income)
        {
            this.startup = startup;
            SetName(startup.label);
            originalLoanAmount = startup.combinedLiability.amount;
            originalInterest = startup.accruedDelayedInterest;
            _restructuredLoan = new RestructuredBusinessLoan(startup);
        }
    }

}
