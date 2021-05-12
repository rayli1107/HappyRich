﻿using System.Collections.Generic;
using UnityEngine;

using Investment = System.Tuple<InvestmentPartner, int>;

namespace Assets
{
    public class AbstractSecuredLoan : AbstractLiability
    {
        public AbstractInvestment asset { get; private set; }
        public int maxltv { get; protected set; }
        public int minltv { get; protected set; }

        public override int amount => ltv * asset.loanUnitValue;

        private int _ltv;
        public int defaultltv { get; private set; }

        public int ltv
        {
            get => _ltv;
            set
            {
                int newltv = Mathf.Clamp(value, minltv, maxltv);
                int delta = (newltv - _ltv) * asset.loanUnitValue;
                if (delta > 0)
                {
                    AddLoan(delta);
                }
                else if (delta < 0)
                {
                    RemoveLoan(-1 * delta);
                }
                _ltv = newltv;
            }
        }

        protected int getUnitCount(int loanAmount)
        {
            int unitValue = asset.loanUnitValue;
            return (loanAmount + unitValue - 1) / unitValue;
        }

        public AbstractSecuredLoan(
            AbstractInvestment asset,
            string label,
            int defaultltv,
            int maxltv,
            int interestRate) :
            base(label, 0, interestRate)
        {
            this.asset = asset;
            this.maxltv = getUnitCount(
                maxltv * asset.loanUnitValue - asset.combinedLiability.amount);
            minltv = 0;
            this.defaultltv = defaultltv;
            _ltv = 0;
            ltv = defaultltv;
        }

        protected virtual void AddLoan(int delta)
        {
        }

        protected virtual void RemoveLoan(int delta)
        {
        }

        public override int PayOff(int _)
        {
            return 0;
        }

        public void setMinimumLoanAmount(int loanAmount)
        {
            minltv = Mathf.Min(getUnitCount(loanAmount), maxltv);
            ltv = ltv;
        }

        public void setLoanAmount(int loanAmount)
        {
            ltv = getUnitCount(loanAmount);
        }
    }

    public class Mortgage : AbstractSecuredLoan
    {
        public Mortgage(AbstractInvestment asset, int defaultltv, int maxltv)
            : base(asset,
                   string.Format("Mortgage - {0}", asset.label),
                   defaultltv,
                   maxltv,
                   InterestRateManager.Instance.realEstateLoanRate)
        {
        }
    }

    public class BusinessLoan : AbstractSecuredLoan
    {
        public BusinessLoan(AbstractInvestment asset, int defaultltv, int maxltv)
            : base(asset,
                   string.Format("Business Loan - {0}", asset.label),
                   defaultltv,
                   maxltv,
                   InterestRateManager.Instance.businessLoanRate)
        {
        }
    }

    public class PrivateLoan : AbstractSecuredLoan
    {
        private List<Investment> _investments;
        private bool _delayed;

        public override int expense => _delayed ? 0 : base.expense;
        public int delayedExpense => _delayed ? base.expense : 0;

        public List<InvestmentPartner> privateLenders
        {
            get
            {
                List<InvestmentPartner> partners = new List<InvestmentPartner>();
                foreach (Investment investment in _investments)
                {
                    if (investment.Item2 > 0)
                    {
                        partners.Add(investment.Item1);
                    }
                }
                return partners;
            }
        }

        public PrivateLoan(
            AbstractInvestment asset,
            List<InvestmentPartner> partners,
            int maxltv,
            int interestRate,
            bool delayed) :
            base(asset,
                 string.Format("Private Loan - %s", asset.label),
                 0,
                 maxltv,
                 interestRate)
        {
            _investments = new List<Investment>();
            int availableCash = 0;
            foreach (InvestmentPartner partner in partners)
            {
                if (partner.cash > 0)
                {
                    _investments.Add(new Investment(partner, 0));
                    availableCash += partner.cash;
                }
            }

            this.maxltv = Mathf.Min(
                this.maxltv, availableCash / asset.loanUnitValue);
            _delayed = delayed;
        }

        protected override void AddLoan(int delta)
        {
            for (int i = 0; i < _investments.Count && delta > 0; ++i)
            {
                InvestmentPartner partner = _investments[i].Item1;
                int partnerAmount = Mathf.Min(partner.cash, delta);
                if (partnerAmount > 0)
                {
                    partner.cash -= partnerAmount;
                    delta -= partnerAmount;

                    _investments[i] = new Investment(
                        partner, _investments[i].Item2 + partnerAmount);
                }
            }
        }

        protected override void RemoveLoan(int delta)
        {
            int totalPaidOff = 0;
            for (int i = 0; i < _investments.Count && delta > 0; ++i)
            {
                InvestmentPartner partner = _investments[i].Item1;
                int partnerAmount = Mathf.Min(
                    _investments[i].Item2, delta);
                if (partnerAmount > 0)
                {
                    totalPaidOff += partnerAmount;
                    delta -= partnerAmount;
                    partner.cash += partnerAmount;
 
                    _investments[i] = new Investment(
                        partner, _investments[i].Item2 - partnerAmount);
                }
            }
        }
    }
}