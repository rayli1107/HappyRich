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

        public Startup(
            string description,
            string label,
            int startupCost,
            int duration,
            int loanLtv,
            int maxLoanLtv)
            : base(description, startupCost, 0, 0, true)
        {
            this.description = description;
            this.label = label;
            _duration = duration;
            _turn = 0;
            if (maxLoanLtv > 0)
            {
                primaryLoan = new BusinessLoan(this, loanLtv, maxLoanLtv, true);
            }
        }

        public void SetName(string name)
        {
            label = name;
        }
    }
}
