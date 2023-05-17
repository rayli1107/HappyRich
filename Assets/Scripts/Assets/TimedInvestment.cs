using PlayerInfo;
using System;
using UI;
using UI.Panels.Templates;
using UnityEngine;

namespace Assets
{
    [Serializable]
    public class TimedInvestmentData
    {
        public enum TimedInvestmentType
        {
            STARTUP_EXIT,
        };

        [SerializeField]
        private InvestmentData _investmentData;
        public InvestmentData investmentData => _investmentData;

        [SerializeField]
        private string _investmentName;
        public string investmentName => _investmentName;

        [SerializeField]
        private int _turnsLeft;
        public int turnsLeft => _turnsLeft;

        [SerializeField]
        private TimedInvestmentType _timedInvestmentType;
        public TimedInvestmentType timedInvestmentType => _timedInvestmentType;

        [SerializeField]
        private float _startupExitMultiplier;
        public float startupExitMultiplier => _startupExitMultiplier;

        [SerializeField]
        private string _startupExitMessage;
        public string startupExitMessage => _startupExitMessage;

        private void initializeInvestmentData(
            int originalPrice,
            int marketValue,
            int annualIncome,
            string investmentName,
            int turnsLeft)
        {
            _investmentData = new InvestmentData();
            _investmentData.Initialize(originalPrice, marketValue, annualIncome, annualIncome);
            _investmentName = investmentName;
            _turnsLeft = turnsLeft;
        }

        private void initializeTimedInvestmentAsStartup(
            int originalPrice,
            string investmentName,
            int turnsLeft,
            float multilpier,
            string message)
        {
            initializeInvestmentData(originalPrice, originalPrice, 0, investmentName, turnsLeft);

            _timedInvestmentType = TimedInvestmentType.STARTUP_EXIT;
            _startupExitMultiplier = multilpier;
            _startupExitMessage = message;
        }

        public void InitializeStartupFailedInvestmentData(
            int originalPrice, int turnsLeft)
        {
            string message = "Unfortunately, the startup company that you invested in " +
                "went backrupt and you were not able to recoup any losses.";
            initializeTimedInvestmentAsStartup(
                originalPrice, "Startup Company", turnsLeft, 0, message);
        }

        public void InitializeStartupIPOInvestmentData(
            int originalPrice, int turnsLeft, float multiplier)
        {
            string message = "Great news! The startup company that you invested in finally " +
                "went public, giving you a return of {0} from your investment.";
            initializeTimedInvestmentAsStartup(
                originalPrice, "Startup Company", turnsLeft, multiplier, message);
        }

        public void InitializeStartupAcquiredInvestmentData(
            int originalPrice, int turnsLeft, float multiplier)
        {
            string message = "The startup company that you invested in was acquired by " +
                "another company, giving you a return of {0} from your investment.";
            initializeTimedInvestmentAsStartup(
                originalPrice, "Startup Company", turnsLeft, multiplier, message);
        }

        public void ResolveTurn(Action<bool> callback)
        {
            --_turnsLeft;
            callback?.Invoke(turnsLeft <= 0);
        }
    }


    public abstract class AbstractTimedInvestment : AbstractInvestment
    {
        protected TimedInvestmentData _timedInvestmentData;

        public AbstractTimedInvestment(TimedInvestmentData timedInvestmentData)
            : base(timedInvestmentData.investmentData)
        {
            _timedInvestmentData = timedInvestmentData;
        }

        private void resolveHandler(Player player, Action<bool> callback, bool done)
        {
            if (done)
            {
                OnInvestmentEnd(player, () => callback?.Invoke(true));
            }
            else
            {
                callback?.Invoke(false);
            }
        }

        public void OnResolve(Player player, Action<bool> callback)
        {
            _timedInvestmentData.ResolveTurn(
                done => resolveHandler(player, callback, done));
        }

        protected abstract void OnInvestmentEnd(Player player, Action callback);
    }

    public class StartupInvestment : AbstractTimedInvestment
    {
        public override string investmentType => "Startup Investment";

        public StartupInvestment(TimedInvestmentData data)
            : base(data)
        {
        }

        protected override void OnInvestmentEnd(Player player, Action callback)
        {
            int gain = Mathf.FloorToInt(
                _timedInvestmentData.startupExitMultiplier * _timedInvestmentData.investmentData.originalPrice);
            player.portfolio.AddCash(gain);

            string message = _timedInvestmentData.startupExitMessage;
            if (gain > 0)
            {
                Localization local = Localization.Instance;
                message = string.Format(message, local.GetCurrency(gain));
            }

            UIManager.Instance.ShowSimpleMessageBox(
                message,
                ButtonChoiceType.OK_ONLY,
                (_) => callback?.Invoke());
        }
    }
/*
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
*/
}

