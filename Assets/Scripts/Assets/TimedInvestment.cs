using PlayerInfo;
using System;
using System.Collections.Generic;
using UI;
using UI.Panels.Templates;
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
            : base(name, originalPrice, marketValue, annualIncome, false)
        {
            this.turnsLeft = turnsLeft;
        }

        public void OnResolve(Player player, Action<bool> callback)
        {
            --turnsLeft;
            Debug.LogFormat("Resolving timed investment: {0}", turnsLeft);
            if (turnsLeft <= 0)
            {
                OnInvestmentEnd(player, () => callback?.Invoke(true));
            }
            else
            {
                callback?.Invoke(false);
            }

        }

        protected abstract void OnInvestmentEnd(Player player, Action callback);
    }

    public class StartupInvestment : AbstractTimedInvestment
    {
        private StartupExitAction _exitAction;

        public StartupInvestment(
            int originalPrice,
            int turnsLeft,
            StartupExitAction exitAction)
            : base("Unnamed Startup Company", originalPrice, originalPrice, 0, turnsLeft)
        {
            _exitAction = exitAction;
        }

        protected override void OnInvestmentEnd(Player player, Action callback)
        {
            _exitAction.RunExitAction(player, this, callback);
        }
    }

    public class StartupExitAction
    {
        protected string _message;
        private float _multiplier;

        public StartupExitAction(float multiplier)
        {
            _multiplier = multiplier;
        }

        public void RunExitAction(
            Player player,
            StartupInvestment investment,
            Action callback)
        {
            string message;
            if (_multiplier == 0)
            {
                message = _message;
            }
            else
            {
                int gain = Mathf.FloorToInt(_multiplier * investment.originalPrice);
                player.portfolio.AddCash(gain);

                Localization local = Localization.Instance;
                message = string.Format(_message, local.GetCurrency(gain));
            }

            UIManager.Instance.ShowSimpleMessageBox(
                message,
                ButtonChoiceType.OK_ONLY,
                (_) => callback?.Invoke());
        }
    }

    public class StartupExitBankruptAction : StartupExitAction
    {
        public StartupExitBankruptAction() : base(0)
        {
            _message = "Unfortunately, your friend's startup that you invested in " +
                "went backrupt and you were not able to recoup any losses.";
        }
    }

    public class StartupExitAcquiredAction : StartupExitAction
    {
        public StartupExitAcquiredAction(float multiplier) : base(multiplier)
        {
            _message = "Your friend's startup that you invested in was acquired by " +
                "another company, giving you a return of {0} from your investment.";
        }
    }

    public class StartupExitIPOAction : StartupExitAction
    {
        public StartupExitIPOAction(float multiplier) : base(multiplier)
        {
            _message = "Great news! Your friend's startup that you invested in finally " +
                "went public, giving you a return of {0} from your investment.";
        }
    }
}

