using Assets;
using PlayerInfo;
using System;
using UI;
using UI.Panels.Templates;
using UnityEngine;

namespace Actions
{
    public enum RiskLevel
    {
        kLow,
        kMedium,
        kHigh
    }

    public class StartupInvestmentAction : AbstractAction
    {
        private Player _player;
        private string _startupIdea;
        private int _turnsLeft;
        private StartupExitAction _exitAction;

        public StartupInvestmentAction(
            Player player,
            ActionCallback callback,
            string startupIdea,
            int turnsLeft,
            StartupExitAction exitAction) : base(callback)
        {
            _player = player;
            _startupIdea = startupIdea;
            _turnsLeft = turnsLeft;
            _exitAction = exitAction;
        }

        private string confirmMessageHandler(ButtonType buttonType, int number)
        {
            if (buttonType == ButtonType.OK)
            {
                Localization local = Localization.Instance;
                return string.Format(
                    "Invest {0} in your friend's startup?",
                    local.GetCurrency(number));
            }
            else
            {
                return "Pass on investing in your friend's startup?";
            }
        }

        private void startTransactionHandler(TransactionHandler handler, int number)
        {
            StartupInvestment investment = new StartupInvestment(
                number, _turnsLeft, _exitAction);
            TransactionManager.BuyTimedInvestment(_player, investment, handler);
        }

        public override void Start()
        {
            Localization local = Localization.Instance;
            int maxValue = _player.portfolio.cash;
            string message = string.Format(
                "A friend of yours decided to launch a startup focusing on {0}, and asked " +
                "if you're interested in investing in his company.\nAvailable Funds: {1}",
                _startupIdea,
                local.GetCurrency(maxValue));
            UIManager.Instance.ShowNumberInputPanel(
                message,
                maxValue,
                messageBoxHandler,
                confirmMessageHandler,
                startTransactionHandler);
        }

        private void messageBoxHandler(ButtonType buttonType, int number)
        {
            UI.UIManager.Instance.UpdatePlayerInfo(_player);
            RunCallback(buttonType == ButtonType.OK);
        }
    }
}
