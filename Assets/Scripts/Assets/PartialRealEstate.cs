using System;
using System.Collections.Generic;
using UnityEngine;

using Investment = System.Tuple<InvestmentPartner, int>;

namespace Assets
{
    
    public class PartialRealEstate : AbstractRealEstate
    {
        public AbstractRealEstate asset { get; private set; }
        public float equitySplit { get; private set; }

        private List<Investment> _investments;

        public int investorAmount {
            get
            {
                int amount = 0;
                foreach (Investment investment in _investments)
                {
                    amount += investment.Item2;
                }
                return amount;
            }
        }

        public float investorEquity => (equitySplit * investorAmount) / asset.downPayment;
        public int investorCashflow => Mathf.FloorToInt(investorEquity * income);

        public bool hasInvestors => _investments.Count > 0;

        public override int downPayment => asset.downPayment - investorAmount;
        public float equity => 1 - investorEquity;

        public override int value
        {
            get
            {
                return downPayment + combinedLiability.amount;
            }
        }

        public override List<AbstractLiability> liabilities
        {
            get
            {
                List<AbstractLiability> liability = new List<AbstractLiability>();
                liability.Add(new PartialLiability(asset.combinedLiability, equity));
                return liability;
            }
        }

        public PartialRealEstate(AbstractRealEstate asset, float equitySplit) :
            base(asset)
        {
            this.asset = asset;
            this.equitySplit = equitySplit;

            _investments = new List<Investment>();
        }

        public void ClearInvestors()
        {
            foreach (Investment investment in _investments)
            {
                investment.Item1.cash += investment.Item2;
            }
            _investments.Clear();
        }

        public void AddInvestor(InvestmentPartner partner, int amount)
        {
            partner.cash -= amount;
            _investments.Add(new Investment(partner, amount));
        }
    }

}
