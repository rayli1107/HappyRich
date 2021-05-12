using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets
{
    public class AbstractInvestment : AbstractAsset
    {
        public int originalPrice { get; protected set; }
        public virtual int totalCost => originalPrice;
        public virtual int loanValue => value;
        public virtual int loanUnitValue => loanValue / 100;
        public string label { get; protected set; }
        public string description { get; protected set; }
        public override string name => label;
        public virtual int downPayment => Mathf.Max(
            totalCost - combinedLiability.amount, 0);

        public AbstractSecuredLoan primaryLoan { get; protected set; }
        public PrivateLoan privateLoan { get; protected set; }

        public override List<AbstractLiability> liabilities
        {
            get
            {
                List<AbstractLiability> ret = base.liabilities;
                if (primaryLoan != null)
                {
                    ret.Add(primaryLoan);
                }
                if (privateLoan != null)
                {
                    ret.Add(privateLoan);
                }
                return ret;
            }
        }

        public AbstractInvestment(
            string name,
            int originalPrice,
            int marketValue,
            int annualIncome)
            : base(name, marketValue, annualIncome)
        {
            this.originalPrice = originalPrice;
        }

        public void AddPrivateLoan(
            List<InvestmentPartner> partners,
            int maxltv,
            int rate,
            bool delayed)
        {
            if (privateLoan == null)
            {
                privateLoan = new PrivateLoan(this, partners, maxltv, rate, delayed);
            }
        }

        public void ClearPrivateLoan()
        {
            if (privateLoan != null)
            {
                privateLoan.ltv = 0;
                privateLoan = null;
            }
        }

        public override void OnPurchaseCancel()
        {
            base.OnPurchaseCancel();
            ClearPrivateLoan();
        }
    }
}
