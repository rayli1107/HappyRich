using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets
{
    public class AbstractBusiness : AbstractInvestment
    {
        public RealEstateTemplate template { get; private set; }
        public int unitCount { get; private set; }
        public Mortgage mortgage { get; protected set; }
        public RealEstatePrivateLoan privateLoan { get; protected set; }

        public AbstractBusiness(
            RealEstateTemplate template,
            int originalPrice,
            int marketValue,
            int annualIncome,
            int unitCount)
            : base("", originalPrice, marketValue, annualIncome)
        {
            label = unitCount > 1 ?
                string.Format(template.label, unitCount) :
                template.label;
            description = unitCount > 1 ?
                string.Format(template.description, unitCount) :
                template.description;

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
        /*
        public void AddPrivateLoan(
            List<InvestmentPartner> partners,
            int maxltv,
            int rate,
            bool delayed)
        {
            if (privateLoan == null)
            {
                privateLoan = new RealEstatePrivateLoan(
                    this, partners, maxltv, rate, delayed);
            }
        }
        */
        public void ClearPrivateLoan()
        {
            Debug.Log("ClearPrivateLoan");
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
