using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets
{
    public class AbstractInvestment : AbstractAsset
    {
        public int originalPrice { get; private set; }
        public virtual int totalCost => originalPrice;
        public virtual int loanValue => value;
        public virtual int loanUnitValue => loanValue / 100;
        public string label { get; protected set; }
        public string description { get; protected set; }
        public override string name => label;

        public virtual int downPayment => Mathf.Max(
            totalCost - combinedLiability.amount, 0);

        public AbstractInvestment(
            string name,
            int originalPrice,
            int marketValue,
            int annualIncome)
            : base(name, marketValue, annualIncome)
        {
            this.originalPrice = originalPrice;
        }
    }
}
