using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets
{
    public class AbstractInvestment : AbstractAsset
    {
        public int originalPrice { get; protected set; }
        public virtual int totalCost => originalPrice;
        public virtual int loanValue => originalPrice;
        public virtual int loanUnitValue => loanValue / 100;
        public string label { get; protected set; }
        public string description { get; protected set; }
        public override string name => label;
        public virtual int downPayment => Mathf.Max(
            totalCost - combinedLiability.amount, 0);

        protected Vector2Int _baseIncomeRange;
        protected int _incomeIncrement;
        protected virtual int _baseIncome { get; private set; }
        public float multiplier;
        public override int totalIncome => Mathf.FloorToInt(_baseIncome * multiplier);
        public Vector2Int totalIncomeRange => new Vector2Int(
            Mathf.FloorToInt(_baseIncomeRange.x * multiplier),
            Mathf.FloorToInt(_baseIncomeRange.y * multiplier));
        public override int expectedTotalIncome => totalIncomeRange.x;
        public Vector2Int incomeRange => new Vector2Int(
            calculateNetIncome(totalIncomeRange.x),
            calculateNetIncome(totalIncomeRange.y));

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
                foreach (AbstractSecuredLoan loan in securedLoans)
                {
                    interest += loan.delayedExpense;
                }
                return interest;
            }
        }

        public AbstractInvestment(
            string name,
            int originalPrice,
            int marketValue,
            int annualIncome)
            : base(name, marketValue, 0)
        {
            _baseIncomeRange = new Vector2Int(annualIncome, annualIncome);
            multiplier = 1;
            this.originalPrice = originalPrice;
            label = name;
        }

        public void AddPrivateLoan(
            List<InvestmentPartner> partners, int maxltv)
        {
            if (privateLoan == null)
            {
                privateLoan = new PrivateLoan(
                    this, partners, maxltv, _privateLoanRate, _isDebtInterestDelayed);
            }
        }

        public void ClearPrivateLoan()
        {
            if (privateLoan != null)
            {
                privateLoan.ltv = 0;
                privateLoan = null;
            }
        }

        public override void OnPurchaseCancel()
        {
            base.OnPurchaseCancel();
            ClearPrivateLoan();
        }

        public override void OnTurnStart(System.Random random)
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
    }
}
