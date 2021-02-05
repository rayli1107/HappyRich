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
        public int maxShares { get; private set; }
        public float equityPerShare => equitySplit / maxShares;
        public int amountPerShare { get; private set; }

        private List<Investment> _investments;
        public override string name => asset.name;
        public int totalShares { get; private set; }

        private int _shares;
        public int shares
        {
            get => _shares;
            set
            {
                int newValue = Mathf.Clamp(value, 0, totalShares);
                int delta = newValue - _shares;
                if (delta > 0)
                {
                    AddShares(delta);
                }
                else if (delta < 0)
                {
                    RemoveShares(-1 * delta);
                }
                _shares = newValue;
            }
        }
 
        public int investorAmount => shares * amountPerShare;
        public float investorEquity => shares * equityPerShare;
        public int investorCashflow => Mathf.FloorToInt(investorEquity * asset.income);

        public bool hasInvestors => shares > 0;

        public int fundsNeeded => asset.downPayment - investorAmount;
        public float equity => 1 - investorEquity;

        public override int value
        {
            get
            {
                Localization local = Localization.Instance;
                int loanAmount = combinedLiability.amount;
                int loanEquity = Mathf.Min(loanAmount, asset.value);
                Debug.LogFormat("Loan Equity {0}", loanEquity);

                int capital = asset.downPayment;
                int capitalEquity = fundsNeeded;
                Debug.LogFormat(
                    "Capital {0} {1}",
                    local.GetCurrencyPlain(capital),
                    local.GetCurrencyPlain(capitalEquity));

                int newEquityTotal = Mathf.Max(asset.value - asset.purchasePrice, 0);
                int newEquity = Mathf.FloorToInt(equity * newEquityTotal);
                Debug.LogFormat(
                    "New Equity {0} {1} {2}",
                    local.GetCurrencyPlain(newEquityTotal),
                    local.GetPercentPlain(equity),
                    local.GetCurrencyPlain(newEquity));

                return loanEquity + capitalEquity + newEquity;
            }
        }

        public override int income => asset.income - investorCashflow;
        public override List<AbstractLiability> liabilities => asset.liabilities;

        public PartialRealEstate(
            AbstractRealEstate asset,
            List<InvestmentPartner> partners,
            float equitySplit,
            int maxShares) :
            base("", 0, 0)
        {
            this.asset = asset;
            this.equitySplit = equitySplit;
            this.maxShares = maxShares;

            totalShares = 0;
            _shares = 0;

            _investments = new List<Investment>();
            foreach (InvestmentPartner partner in partners)
            {
                _investments.Add(new Investment(partner, 0));
            }

            Reset();
        }

        public void Reset()
        {
            shares = 0;
            amountPerShare = Mathf.FloorToInt(asset.downPayment / maxShares);

            totalShares = 0;
            for (int i = 0; i < _investments.Count; ++i)
            {
                totalShares += _investments[i].Item1.cash / amountPerShare;
            }

            totalShares = Mathf.Min(totalShares, maxShares);
        }

        private void AddShares(int delta)
        {
            for (int i = 0; i < _investments.Count; ++i)
            {
                InvestmentPartner partner = _investments[i].Item1;
                int availableShares = Mathf.Min(
                    delta, partner.cash / amountPerShare);
                if (availableShares > 0)
                {
                    delta -= availableShares;
                    partner.cash -= availableShares * amountPerShare;
                    _investments[i] = new Investment(
                        partner, _investments[i].Item2 + availableShares);
                }
            }
            Debug.Assert(delta == 0);
        }

        private void RemoveShares(int delta)
        {
            for (int i = 0; i < _investments.Count; ++i)
            {
                InvestmentPartner partner = _investments[i].Item1;
                int removedShares = Mathf.Min(delta, _investments[i].Item2);
                if (removedShares > 0)
                {
                    delta -= removedShares;
                    partner.cash += removedShares * amountPerShare;
                    _investments[i] = new Investment(
                        partner, _investments[i].Item2 - removedShares);
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
    }

}
