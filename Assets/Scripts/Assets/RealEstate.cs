using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets
{
    public class AbstractRealEstate : AbstractAsset
    {
        public int originalPrice { get; private set; }
        public virtual int purchasePrice => value;
        public virtual int loanValue => value;
        public virtual int loanUnitValue => loanValue / 100;
        public RealEstateTemplate template { get; private set; }
        public int unitCount { get; private set; }
        public virtual string label =>
            unitCount > 1 ? string.Format(template.label, unitCount) : template.label;
        public virtual string description =>
            unitCount > 1 ? string.Format(template.description, unitCount) : template.description;
        public override string name => label;

        public virtual int downPayment => value - combinedLiability.amount;

        public Mortgage mortgage { get; protected set; }
        public RealEstatePrivateLoan privateLoan { get; protected set; }


        public AbstractRealEstate(
            RealEstateTemplate template,
            int originalPrice,
            int marketValue,
            int annualIncome,
            int unitCount)
            : base(template.label, marketValue, annualIncome)
        {
            this.originalPrice = originalPrice;
            this.template = template;
            this.unitCount = unitCount;
        }

        public override List<AbstractLiability> liabilities
        {
            get
            {
                List<AbstractLiability> ret = base.liabilities;
                if (mortgage != null)
                {
                    ret.Add(mortgage);
                }
                if (privateLoan != null)
                {
                    ret.Add(privateLoan);
                }
                return ret;
            }
        }

        public void ClearPrivateLoans()
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
            ClearPrivateLoans();
        }
    }
}
