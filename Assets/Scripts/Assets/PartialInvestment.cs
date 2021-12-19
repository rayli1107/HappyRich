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
        public Vector2Int investorCashflowRange => new Vector2Int(
            getInvestorCashflow(asset.incomeRange.x),
            getInvestorCashflow(asset.incomeRange.y));
        public int investorCashflow => getInvestorCashflow(asset.income);

        public bool hasInvestors => investorShares > 0;

        public int fundsNeeded => asset.downPayment - investorCapital;
        public float equity => 1 - investorEquity;
        public Vector2Int incomeRange => new Vector2Int(
            GetOwnerCashflow(asset.incomeRange.x),
            GetOwnerCashflow(asset.incomeRange.y));
        public override int expectedIncome => GetOwnerCashflow(asset.incomeRange.x);

        public override int value
        {
            get
            {
                int loanAmount = combinedLiability.amount;
                int loanEquity = Mathf.Min(loanAmount, asset.value);

                if (asset.returnCapital)
                {
                    int capital = asset.downPayment;
                    int capitalEquity = fundsNeeded;
                    int newEquityTotal = Mathf.Max(
                        asset.value - loanAmount - capital, 0);
                    int newEquity = Mathf.FloorToInt(equity * newEquityTotal);

                    return loanEquity + capitalEquity + newEquity;
                }
                else
                {
                    int newEquityTotal = Mathf.Max(asset.value - loanAmount, 0);
                    int newEquity = Mathf.FloorToInt(equity * newEquityTotal);
                    return loanEquity + newEquity;
                }
            }
        }

        public override int income => GetOwnerCashflow(asset.income);
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

        public void Restructure(PublicCompany company)
        {
            capitalPerShare = 0;
            asset = company;
        }

        public override List<string> GetDetails()
        {
            Localization local = Localization.Instance;
            List<string> details = asset.GetDetails();
            if (hasInvestors)
            {
                details.Add(
                    string.Format(
                        "Your Equity: {0}",
                        local.GetPercent(equity, false)));
                details.Add(
                    string.Format(
                        "Your Asset Value: {0}",
                        local.GetCurrency(value)));

                int incomeLow = incomeRange.x;
                int incomeHigh = incomeRange.y;
                if (incomeLow == incomeHigh)
                {
                    if (incomeLow != 0)
                    {
                        details.Add(
                            string.Format(
                                "Your Profit Share: {0}",
                                local.GetCurrency(incomeLow)));
                    }
                }
                else
                {
                    details.Add(
                        string.Format(
                            "Your Profit Share: {0} ~ {1}",
                            local.GetCurrency(incomeLow),
                            local.GetCurrency(incomeHigh)));
                }
            }
            return details;
        }

        public override void OnTurnStart(System.Random random)
        {
            base.OnTurnStart(random);
            asset.OnTurnStart(random);
        }

        private int getInvestorCashflow(int assetIncome)
        {
            return Mathf.Max(
                Mathf.FloorToInt(investorEquity * assetIncome), 0);
        }

        public int GetOwnerCashflow(int assetIncome)
        {
            return assetIncome - getInvestorCashflow(assetIncome);
        }
    }
}
