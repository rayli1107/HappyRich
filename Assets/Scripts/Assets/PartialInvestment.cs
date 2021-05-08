using System;
using System.Collections.Generic;
using UnityEngine;

using Investment = System.Tuple<InvestmentPartner, int>;

namespace Assets
{
    public class PartialInvestment : AbstractAsset
    {
        public AbstractInvestment asset { get; private set; }
        public float equitySplit { get; private set; }
        public int maxShares { get; private set; }
        public float equityPerShare => equitySplit / maxShares;
        public int capitalPerShare { get; private set; }
        public List<Investment> investments { get; private set; }
        public override string name => asset.name;
        public int totalShares { get; private set; }

        private int _investorShares;
        public int investorShares
        {
            get => _investorShares;
            set
            {
                int newValue = Mathf.Clamp(value, 0, totalShares);
                int delta = newValue - _investorShares;
                if (delta > 0)
                {
                    AddShares(delta);
                }
                else if (delta < 0)
                {
                    RemoveShares(-1 * delta);
                }
                _investorShares = newValue;
            }
        }

        public int totalCapital => asset.downPayment; 
        public int investorCapital => investorShares * capitalPerShare;

        public float investorEquity => investorShares * equityPerShare;
        public int investorCashflow => Mathf.FloorToInt(investorEquity * asset.income);

        public bool hasInvestors => investorShares > 0;

        public int fundsNeeded => asset.downPayment - investorCapital;
        public float equity => 1 - investorEquity;

        public override int value
        {
            get
            {
//                Localization local = Localization.Instance;
                int loanAmount = combinedLiability.amount;
                int loanEquity = Mathf.Min(loanAmount, asset.value);
//                Debug.LogFormat("Loan Equity {0}", loanEquity);

                int capital = asset.downPayment;
                int capitalEquity = fundsNeeded;
/*                Debug.LogFormat(
                    "Capital {0} {1}",
                    local.GetCurrencyPlain(capital),
                    local.GetCurrencyPlain(capitalEquity));
                    */
                int newEquityTotal = Mathf.Max(
                    asset.value - loanAmount - capital, 0);
                int newEquity = Mathf.FloorToInt(equity * newEquityTotal);
/*                Debug.LogFormat(
                    "New Equity {0} {1} {2}",
                    local.GetCurrencyPlain(newEquityTotal),
                    local.GetPercentPlain(equity),
                    local.GetCurrencyPlain(newEquity));
                    */

                return loanEquity + capitalEquity + newEquity;
            }
        }

        public override int income => asset.income - investorCashflow;
        public override List<AbstractLiability> liabilities => asset.liabilities;

        public PartialInvestment(
            AbstractInvestment asset,
            List<InvestmentPartner> partners,
            float equitySplit,
            int maxShares) :
            base("", 0, 0)
        {
            this.asset = asset;
            this.equitySplit = equitySplit;
            this.maxShares = maxShares;

            totalShares = 0;
            _investorShares = 0;

            investments = new List<Investment>();
            foreach (InvestmentPartner partner in partners)
            {
                investments.Add(new Investment(partner, 0));
            }

            Reset();
        }

        public void Reset()
        {
            investorShares = 0;
            capitalPerShare = Mathf.FloorToInt(asset.downPayment / maxShares);

            totalShares = 0;
            for (int i = 0; i < investments.Count; ++i)
            {
                totalShares += investments[i].Item1.cash / capitalPerShare;
            }

            totalShares = Mathf.Min(totalShares, maxShares);
        }

        private void AddShares(int delta)
        {
            for (int i = 0; i < investments.Count; ++i)
            {
                InvestmentPartner partner = investments[i].Item1;
                int availableShares = Mathf.Min(
                    delta, partner.cash / capitalPerShare);
                if (availableShares > 0)
                {
                    delta -= availableShares;
                    partner.cash -= availableShares * capitalPerShare;
                    investments[i] = new Investment(
                        partner, investments[i].Item2 + availableShares);
                }
            }
            Debug.Assert(delta == 0);
        }

        private void RemoveShares(int delta)
        {
            for (int i = 0; i < investments.Count; ++i)
            {
                InvestmentPartner partner = investments[i].Item1;
                int removedShares = Mathf.Min(delta, investments[i].Item2);
                if (removedShares > 0)
                {
                    delta -= removedShares;
                    partner.cash += removedShares * capitalPerShare;
                    investments[i] = new Investment(
                        partner, investments[i].Item2 - removedShares);
                }
            }
            Debug.Assert(delta == 0);
        }

        public override void OnPurchase()
        {
            base.OnPurchase();
            asset.OnPurchase();
        }

        public override void OnPurchaseCancel()
        {
            base.OnPurchaseCancel();
            Reset();
            asset.OnPurchaseCancel();
        }

        public void Refinance(RefinancedRealEstate newAsset)
        {
            int returnedCapital = newAsset.returnedCapital;
            int returnedCapitalPerShare = returnedCapital / maxShares;
            capitalPerShare = Mathf.Max(
                0, capitalPerShare - returnedCapitalPerShare);
            asset = newAsset;
        }
    }
}
