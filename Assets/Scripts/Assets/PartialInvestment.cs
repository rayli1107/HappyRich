using InvestmentPartnerInfo;
using System;
using System.Collections.Generic;
using UnityEngine;

using Investment = System.Tuple<InvestmentPartnerInfo.InvestmentPartner, int>;

namespace Assets
{
    [Serializable]
    public class PartialInvestmentData
    {
        [SerializeField]
        private float _equitySplit;
        public float equitySplit => _equitySplit;

        [SerializeField]
        private int _maxShares;
        public int maxShares => _maxShares;

        [SerializeField]
        private List<int> _partnerShares;
        public List<int> partnerShares => _partnerShares;

        public void Initialize(float equitySplit, int maxShares, int partnerCount)
        {
            _equitySplit = equitySplit;
            _maxShares = maxShares;
            _partnerShares = new List<int>(partnerCount);
            for (int i = 0; i < partnerCount; ++i)
            {
                _partnerShares.Add(0);
            }
        }
    }
    public class PartialInvestment : AbstractAsset
    {
        public PartialInvestmentData data { get; private set; }
        public AbstractInvestment asset { get; private set; }
        public float equitySplit => data.equitySplit;
        public int maxShares => data.maxShares;
        public float equityPerShare => equitySplit / maxShares;
        public override string name => asset.name;
        public int capitalPerShare { get; private set; }
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
            getInvestorCashflow(asset.netIncomeRange.x),
            getInvestorCashflow(asset.netIncomeRange.y));
        public bool hasInvestors => investorShares > 0;

        public int fundsNeeded => asset.downPayment - investorCapital;
        public float equity => 1 - investorEquity;
        public override Vector2Int netIncomeRange => GetOwnerCashflowRange(asset.netIncomeRange);
        public override Vector2Int totalIncomeRange
        {
            get
            {
                int expense = combinedLiability.expense;
                Vector2Int netRange = netIncomeRange;
                return new Vector2Int(netRange.x + expense, netRange.y + expense);
            }
        }

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

        public override List<AbstractLiability> liabilities => asset.liabilities;

        private InvestmentPartnerManager Manager => InvestmentPartnerManager.Instance;

        public PartialInvestment(AbstractInvestment asset, PartialInvestmentData data)
            : base("", 0, Vector2Int.zero)
        {
            this.data = data;
            this.asset = asset;
            Reset();
        }

        public void Reset()
        {
            investorShares = 0;
            capitalPerShare = Mathf.FloorToInt(asset.downPayment / maxShares);

            totalShares = 0;
            for (int i = 0; i < data.partnerShares.Count; ++i)
            {
                InvestmentPartner partner =
                    InvestmentPartnerManager.Instance.GetPartnerById(i);
                totalShares += partner.cash / capitalPerShare;
            }

            totalShares = Mathf.Min(totalShares, maxShares);
        }

        private void AddShares(int delta)
        {
            for (int i = 0; i < data.partnerShares.Count; ++i)
            {
                InvestmentPartner partner =
                    InvestmentPartnerManager.Instance.GetPartnerById(i);
                int availableShares = Mathf.Min(
                    delta, partner.cash / capitalPerShare);
                if (availableShares > 0)
                {
                    delta -= availableShares;
                    partner.cash -= availableShares * capitalPerShare;
                    data.partnerShares[i] += availableShares;
                }
            }
            Debug.Assert(delta == 0);
        }

        private void RemoveShares(int delta)
        {
            for (int i = 0; i < data.partnerShares.Count; ++i)
            {
                InvestmentPartner partner =
                    InvestmentPartnerManager.Instance.GetPartnerById(i);
                int removedShares = Mathf.Min(delta, data.partnerShares[i]);
                if (removedShares > 0)
                {
                    delta -= removedShares;
                    partner.cash += removedShares * capitalPerShare;
                    data.partnerShares[i] -= removedShares;
                }
            }
            Debug.Assert(delta == 0);
        }

        public override void OnPurchase()
        {
            base.OnPurchase();
            asset.OnPurchase();
        }

        public override void OnPurchaseStart()
        {
            base.OnPurchaseStart();
            Reset();
            asset.OnPurchaseStart();
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

                string formatted = getFormattedIncomeRange(netIncomeRange);
                if (formatted != null && formatted.Length > 0)
                {
                    details.Add(string.Format("Your Profit Share: {0}", formatted));
                }
            }
            return details;
        }
/*
        public override void OnTurnStart(System.Random random)
        {
            base.OnTurnStart(random);
            asset.OnTurnStart(random);
        }
*/
        private int getInvestorCashflow(int assetIncome)
        {
            return Mathf.Max(Mathf.FloorToInt(investorEquity * assetIncome), 0);
        }

        private Vector2Int getInvestorCashflowRange(Vector2Int range)
        {
            return new Vector2Int(
                getInvestorCashflow(range.x),
                getInvestorCashflow(range.y));
        }

        public Vector2Int GetOwnerCashflowRange(Vector2Int range) 
        {
            return new Vector2Int(
                range.x - getInvestorCashflow(range.x),
                range.y - getInvestorCashflow(range.y));
        }
    }
}
