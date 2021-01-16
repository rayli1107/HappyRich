using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets
{
    public class Mortgage : AbstractLiability
    {
        public RentalRealEstate asset { get; private set; }
        public int ltv;
        public int maxltv;

        public override int amount => asset.value * ltv / 100;

        public Mortgage(RentalRealEstate asset, int ltv) 
            : base(string.Format("Mortgage - {0}", asset.template.label), 0,
                   InterestRateManager.Instance.realEstateLoanRate)
        {
            this.asset = asset;
            this.ltv = ltv;
            maxltv = ltv;
        }
    }

    public class AbstractRealEstate : AbstractAsset
    {
        public int originalPrice { get; private set; }
        public virtual int purchasePrice => value;
        public RealEstateTemplate template { get; private set; }
        public int unitCount { get; private set; }
        public virtual string label =>
            unitCount > 1 ? string.Format(template.label, unitCount) : template.label;
        public virtual string description =>
            unitCount > 1 ? string.Format(template.description, unitCount) : template.description;
        public override string name => label;

        public virtual int downPayment => value - combinedLiability.amount;

        public virtual int maxPrivateLoanAmount =>
            value * RealEstateManager.Instance.maxPrivateLoanLTV / 100;

        public Mortgage mortgage { get; protected set; }
        protected List<PrivateLoan> privateLoans { get; private set; }

        public List<InvestmentPartner> privateLenders
        {
            get
            {
                List<InvestmentPartner> partners = new List<InvestmentPartner>();
                foreach (PrivateLoan loan in privateLoans)
                {
                    partners.Add(loan.partner);
                }
                return partners;
            }
        }

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
            privateLoans = new List<PrivateLoan>();
        }

        public override List<AbstractLiability> liabilities
        {
            get
            {
                List<AbstractLiability> ret = new List<AbstractLiability>();
                if (mortgage != null)
                {
                    ret.Add(mortgage);
                }
                ret.AddRange(privateLoans);
                return ret;
            }
        }

        public int privateLoanAmount
        {
            get
            {
                int amount = 0;
                foreach (PrivateLoan loan in privateLoans)
                {
                    amount += loan.amount;
                }
                return amount;
            }
        }

        public int privateLoanPayment
        {
            get
            {
                int amount = 0;
                foreach (PrivateLoan loan in privateLoans)
                {
                    amount += loan.expense;
                }
                return amount;
            }
        }

        public int privateLoanDelayedPayment
        {
            get
            {
                int amount = 0;
                foreach (PrivateLoan loan in privateLoans)
                {
                    amount += loan.delayedExpense;
                }
                return amount;
            }
        }

        /*
        public AbstractRealEstate(AbstractRealEstate asset)
            : base(asset.template.label, asset.value, asset.totalIncome)
        {
            template = asset.template;
            unitCount = asset.unitCount;
            privateLoans = new List<PrivateLoan>();
        }
        */
        public void AddPrivateLoan(PrivateLoan loan)
        {
            privateLoans.Add(loan);
        }

        public void ClearPrivateLoans()
        {
            foreach (PrivateLoan loan in privateLoans)
            {
                loan.PayOff(loan.amount);
            }
            privateLoans.Clear();
        }

        public override void OnPurchaseCancel()
        {
            base.OnPurchaseCancel();
            ClearPrivateLoans();
        }
    }
}
