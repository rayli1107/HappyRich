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

        public override int amount => asset.purchasePrice * ltv / 100;

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
        public int purchasePrice { get; private set; }
        public int marketPrice { get; private set; }
        public RealEstateTemplate template { get; private set; }
        public int unitCount { get; private set; }
        public virtual string label =>
            unitCount > 1 ? string.Format(template.label, unitCount) : template.label;
        public virtual string description =>
            unitCount > 1 ? string.Format(template.description, unitCount) : template.description;
        public override string name => label;

        public virtual int downPayment => purchasePrice - combinedLiability.amount;

        protected List<PrivateLoan> privateLoans { get; private set; }

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

        public AbstractRealEstate(
            RealEstateTemplate template,
            int purchasePrice,
            int marketPrice,
            int annualIncome,
            int unitCount)
            : base(template.label, marketPrice, annualIncome)
        {
            this.template = template;
            this.purchasePrice = purchasePrice;
            this.marketPrice = marketPrice;
            this.unitCount = unitCount;
            privateLoans = new List<PrivateLoan>();
        }

        public AbstractRealEstate(AbstractRealEstate asset)
            : base(asset.template.label, asset.marketPrice, asset.totalIncome)
        {
            template = asset.template;
            purchasePrice = asset.purchasePrice;
            marketPrice = asset.marketPrice;
            unitCount = asset.unitCount;
            privateLoans = new List<PrivateLoan>();
        }

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
