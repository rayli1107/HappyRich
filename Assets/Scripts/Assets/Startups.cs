using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets
{
    public class Startup : AbstractInvestment
    {
        public override string investmentType => "Startup";
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

        protected override List<string> getValueDetails()
        {
            return new List<string>();
        }

        public override List<string> getPurchaseDetails()
        {
            Localization local = Localization.Instance;
            List<string> details = new List<string>()
            {
                string.Format(
                    "Startup Cost: {0}",
                    local.GetCurrency(totalCost))
            };
            int interest = delayedInterest;
            if (interest > 0)
            {
                details.Add(
                    string.Format(
                        "Annual Interest: {0}",
                        local.GetCurrency(interest, true)));
            }
            return details;
        }

    }
}
