using InvestmentPartnerInfo;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets
{
    [Serializable]
    public class InvestmentData
    {
        [SerializeField]
        private int _originalPrice;
        public int originalPrice => _originalPrice;

        [SerializeField]
        private int _marketValue;
        public int marketValue => _marketValue;

        [SerializeField]
        private int _incomeRangeLow;
        public int incomeRangeLow => _incomeRangeLow;

        [SerializeField]
        private int _incomeRangeHigh;
        public int incomeRangeHigh => _incomeRangeHigh;

        public List<string> purchaseDetails;

        public float multiplier;

        [SerializeField]
        public AdjustableLoanData securedLoan;

        [SerializeField]
        public AdjustableLoanData privateLoan;

        public void Initialize(
            int originalPrice,
            int marketValue,
            int incomeRangeLow,
            int incomeRangeHigh)
        {
            _originalPrice = originalPrice;
            _marketValue = marketValue;
            _incomeRangeLow = incomeRangeLow;
            _incomeRangeHigh = incomeRangeHigh;
            purchaseDetails = new List<string>();
            multiplier = 1f;
        }
    }

    public abstract class AbstractInvestment : AbstractAsset
    {
        private InvestmentData _investmentData;
        public abstract string investmentType { get; }

        public int originalPrice => _investmentData.originalPrice;
        public float multiplier
        {
            get => _investmentData.multiplier;
            set { _investmentData.multiplier = value; }
        }

        public virtual int totalCost => originalPrice;
        public virtual int loanValue => originalPrice;
        public virtual int loanUnitValue => loanValue / 100;
        public virtual string label { get; protected set; }
        public virtual string description { get; protected set; }
        public override string name => label;
        public virtual int downPayment => Mathf.Max(
            totalCost - combinedLiability.amount, 0);

        public override Vector2Int totalIncomeRange => new Vector2Int(
            Mathf.FloorToInt(_investmentData.incomeRangeLow * _investmentData.multiplier),
            Mathf.FloorToInt(_investmentData.incomeRangeHigh * _investmentData.multiplier));
        public AdjustableSecuredLoan primaryLoan { get; protected set; }
        public PrivateLoan privateLoan { get; protected set; }

        protected virtual bool _isDebtInterestDelayed => false;
        protected virtual int _privateLoanRate =>
            InterestRateManager.Instance.defaultPrivateLoanRate;
        public virtual bool returnCapital => true;
        public virtual List<AbstractSecuredLoan> securedLoans
        {
            get
            {
                List<AbstractSecuredLoan> loans = new List<AbstractSecuredLoan>();
                if (primaryLoan != null)
                {
                    loans.Add(primaryLoan);
                }
                if (privateLoan != null)
                {
                    loans.Add(privateLoan);
                }
                return loans;
            }
        }

        public override List<AbstractLiability> liabilities
        {
            get
            {
                List<AbstractLiability> ret = base.liabilities;
                ret.AddRange(securedLoans);
                return ret;
            }
        }

        public int delayedInterest
        {
            get
            {
                int interest = 0;
                foreach (AdjustableSecuredLoan loan in securedLoans)
                {
                    interest += loan.delayedExpense;
                }
                return interest;
            }
        }

        public AbstractInvestment(InvestmentData data)
            : base("", data.marketValue, Vector2Int.zero)
        {
            _investmentData = data;
            setupPrivateLoan(null);
        }

        protected void setupPrivateLoan(List<InvestmentPartner> partners)
        {
            if (_investmentData.privateLoan != null)
            {
                privateLoan = new PrivateLoan(
                    this, _investmentData.privateLoan, partners, _privateLoanRate, _isDebtInterestDelayed);
            }
        }

        public void AddPrivateLoan(
            List<InvestmentPartner> partners, int maxltv)
        {
            TODO:
            if (_investmentData.privateLoan == null)
            {
                _investmentData.privateLoan = new AdjustableLoanData();
                _investmentData.privateLoan.Initialize(
                    0, maxltv, InvestmentPartnerManager.Instance.partnerCount);
                setupPrivateLoan(partners);
            }
        }

        public void ClearPrivateLoan()
        {
            if (privateLoan != null)
            {
                privateLoan.ltv = 0;
                privateLoan = null;
            }
            _investmentData.privateLoan = null;
        }

        protected virtual void resetLoans()
        {
            ClearPrivateLoan();
        }

        public override void OnPurchaseStart()
        {
            base.OnPurchaseStart();
            resetLoans();
        }

        public override void OnPurchaseCancel()
        {
            base.OnPurchaseCancel();
            resetLoans();
        }

        /*
         * public override void OnTurnStart(System.Random random)
                {
                    base.OnTurnStart(random);
                    if (_baseIncomeRange.x == _baseIncomeRange.y)
                    {
                        _baseIncome = _baseIncomeRange.x;
                    }
                    else
                    {
                        int x = _baseIncomeRange.x / _incomeIncrement;
                        int y = _baseIncomeRange.y / _incomeIncrement;
                        _baseIncome = random.Next(x, y + 1) * _incomeIncrement;
                        Debug.LogFormat(
                            "{0} newIncome: {1}", name, _baseIncome);
                    }
                }
                private List<string> getIncomeRangeDetail(
                    string label, Vector2Int range)
                {
                    Localization local = Localization.Instance;
                    List<string> details = new List<string>();
                    int incomeLow = range.x;
                    int incomeHigh = range.y;
                    if (incomeLow == incomeHigh)
                    {
                        if (incomeLow != 0)
                        {
                            details.Add(
                                string.Format(
                                    "{0}: {1}",
                                    label,
                                    local.GetCurrency(incomeLow)));
                        }
                    }
                    else
                    {
                        details.Add(
                            string.Format(
                                "{0}: {1} ~ {2}",
                                label,
                                local.GetCurrency(incomeLow),
                                local.GetCurrency(incomeHigh)));
                    }
                    return details;
                }

                protected override List<string> getTotalIncomeDetails()
                {
                    return getIncomeRangeDetail("Total Income", totalIncomeRange);
                }

                protected override List<string> getNetIncomeDetails()
                {
                    return getIncomeRangeDetail("Net Income", incomeRange);
                }
        */

        public string GetActionLabel()
        {
            Localization local = Localization.Instance;
            return string.Format(
                "{0}\nCost: {1}",
                label,
                local.GetCurrency(originalPrice));
        }

        public override List<string> getPurchaseDetails()
        {
            return _investmentData.purchaseDetails;
        }
    }
}
