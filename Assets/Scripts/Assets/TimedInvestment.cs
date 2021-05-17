using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets
{
    public abstract class AbstractTimedInvestment : AbstractInvestment
    {
        public int turnsLeft { get; private set; }
        public AbstractTimedInvestment(
            string name,
            int originalPrice,
            int marketValue,
            int annualIncome,
            int turnsLeft)
            : base(name, originalPrice, marketValue, annualIncome)
        {
            this.turnsLeft = turnsLeft;
        }

        public bool OnMarketEventState()
        {
            --turnsLeft;
            if (turnsLeft <= 0)
            {
                OnInvestmentEnd();
                return true;
            }
            return false;
        }

        protected abstract void OnInvestmentEnd();
    }

    public class StartupInvestment : AbstractTimedInvestment
    {
        private Action _exitAction;

        public StartupInvestment(
            int originalPrice,
            int turnsLeft,
            Action exitAction)
            : base("Unnamed Startup Company", originalPrice, originalPrice, 0, turnsLeft)
        {
            _exitAction = exitAction;
        }

        protected override void OnInvestmentEnd()
        {
            _exitAction?.Invoke();
        }
    }
}

