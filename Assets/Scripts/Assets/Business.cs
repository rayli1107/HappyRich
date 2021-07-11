using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets
{
    public class Business : AbstractInvestment
    {
        public int startupCost { get; private set; }
        public int franchiseFee { get; private set; }
        public override int value => startupCost + franchiseFee;
        public override int totalCost => value;
        public int minIncome { get; private set; }
        public int maxIncome { get; private set; }

        public Business(
            string description,
            int startupCost,
            int franchiseFee,
            int minIncome,
            int maxIncome,
            int actualIncome,
            int loanLtv,
            int maxLoanLtv)
            : base(description, startupCost, 0, actualIncome)
        {
            this.description = description;
            this.startupCost = startupCost;
            this.franchiseFee = franchiseFee;
            this.minIncome = minIncome;
            this.maxIncome = maxIncome;
            label = description;

            primaryLoan = new BusinessLoan(this, loanLtv, maxLoanLtv);
        }

        public void SetName(string name)
        {
            label = name;
        }
    }
}
