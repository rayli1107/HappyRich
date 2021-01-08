using System;
using System.Collections.Generic;
using UnityEngine;

using Investment = System.Tuple<InvestmentPartner, int>;

namespace Assets
{    
    public class PartialRealEstate : AbstractAsset
    {
        public AbstractRealEstate asset { get; private set; }
        public float equitySplit { get; private set; }
        public float equityPerShare { get; private set; }
        public int amountPerShare => Mathf.FloorToInt(asset.downPayment * equityPerShare / equitySplit);

        private List<Investment> _investorShares;
        public override string name => asset.name;

        public int investorShares
        {
            get
            {
                int shares = 0;
                foreach (Investment investment in _investorShares)
                {
                    shares += investment.Item2;
                }
                return shares;

            }
        }
        public int investorAmount => investorShares * amountPerShare;
        public float investorEquity => investorShares * equityPerShare;
        public int investorCashflow => Mathf.FloorToInt(investorEquity * asset.income);

        public bool hasInvestors => _investorShares.Count > 0;

        public int fundsNeeded => asset.downPayment - investorAmount;
        public float equity => 1 - investorEquity;

        public override int value => asset.value - investorAmount;
        public override int income => asset.income - investorCashflow;
        public override List<AbstractLiability> liabilities => asset.liabilities;

        public PartialRealEstate(
            AbstractRealEstate asset, float equitySplit, float equityPerShare) :
            base("", 0, 0)
        {
            this.asset = asset;
            this.equitySplit = equitySplit;
            this.equityPerShare = equityPerShare;

            _investorShares = new List<Investment>();
        }

        public void ClearInvestors()
        {
            foreach (Investment investment in _investorShares)
            {
                investment.Item1.cash += investment.Item2 * amountPerShare;
            }
            _investorShares.Clear();
        }

        public void AddInvestor(InvestmentPartner partner, int shares)
        {
            partner.cash -= shares * amountPerShare;
            _investorShares.Add(new Investment(partner, shares));
        }

        public override void OnPurchase()
        {
            base.OnPurchase();
            asset.OnPurchase();
        }

        public override void OnPurchaseCancel()
        {
            base.OnPurchaseCancel();
            ClearInvestors();
            asset.OnPurchaseCancel();
        }
    }

}
