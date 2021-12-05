using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets
{
    public class Startup : AbstractInvestment
    {
        private int _turn;
        private int _duration;
        public bool exited => _turn >= _duration;
        public override int value => totalCost;

        protected override bool _isDebtInterestDelayed => true;
        protected override int _privateLoanRate =>
            InterestRateManager.Instance.startupPrivateLoanRate;
        public int accruedDelayedInterest => _turn * delayedInterest;

        public Startup(
            string description,
            string label,
            int startupCost,
            int duration,
            int loanLtv,
            int maxLoanLtv)
            : base(description, startupCost, 0, 0)
        {
            this.description = description;
            this.label = label;
            _duration = duration;
            _turn = 0;
            if (maxLoanLtv > 0)
            {
                primaryLoan = new StartupLoan(
                    this, loanLtv, maxLoanLtv, _isDebtInterestDelayed);
            }
        }

        public void SetName(string name)
        {
            label = name;
        }

        public void OnTurnStart()
        {
            ++_turn;
        }
    }
}
