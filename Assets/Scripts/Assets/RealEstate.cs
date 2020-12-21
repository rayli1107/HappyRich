using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets
{
    public class Mortgage : AbstractLiability
    {
        public RentalRealEstate asset { get; private set; }
        public int ltv;
        public override int amount => asset.purchasePrice * ltv / 100;

        public Mortgage(RentalRealEstate asset, int defaultLTV) 
            : base(string.Format("Mortgage - {0}", asset.template.label), 0,
                   InterestRateManager.Instance.realEstateLoanRate)
        {
            this.asset = asset;
            ltv = defaultLTV;
        }
    }

    public class AbstractRealEstate : AbstractAsset
    {
        public int purchasePrice { get; private set; }
        public int marketPrice { get; private set; }
        public RealEstateTemplate template { get; private set; }

        public virtual int downPayment => purchasePrice - combinedLiability.amount;

        public AbstractRealEstate(
            RealEstateTemplate template,
            int purchasePrice,
            int marketPrice,
            int annualIncome)
            : base(template.label, marketPrice, annualIncome)
        {
            this.template = template;
            this.purchasePrice = purchasePrice;
            this.marketPrice = marketPrice;
        }

        public AbstractRealEstate(AbstractRealEstate asset)
            : base(asset.template.label, asset.marketPrice, asset.totalIncome)
        {
            template = asset.template;
            purchasePrice = asset.purchasePrice;
            marketPrice = asset.marketPrice;
        }
    }
}
