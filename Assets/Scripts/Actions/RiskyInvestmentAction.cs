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

    public static class StartupInvestmentAction
    {
        private static string confirmMessageHandler(ButtonType buttonType, int number)
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

        private static void startTransactionHandler(
            Player player,
            TransactionHandler handler,
            StartupExitAction exitAction,
            int turnsLeft,
            int number)
        {
            StartupInvestment investment = new StartupInvestment(number, turnsLeft, exitAction);
            TransactionManager.BuyTimedInvestment(player, investment, handler);
        }

        public static void Start(
            Player player,
            string startupIdea,
            int turnsLeft,
            StartupExitAction exitAction,
            Action callback)
        {
            Localization local = Localization.Instance;
            int maxValue = player.portfolio.cash;
            string message = string.Format(
                "A friend of yours decided to launch a startup focusing on {0}, and asked " +
                "if you're interested in investing in his company.\nAvailable Funds: {1}",
                startupIdea,
                local.GetCurrency(maxValue));
            UIManager.Instance.ShowNumberInputPanel(
                message,
                maxValue,
                (ButtonType button, int n) => messageBoxHandler(player, button, n, callback),
                confirmMessageHandler,
                (TransactionHandler handler, int n) =>
                    startTransactionHandler(player, handler, exitAction, turnsLeft, n));
        }

        private static void messageBoxHandler(
            Player player,
            ButtonType buttonType,
            int number,
            Action callback)
        {
            UI.UIManager.Instance.UpdatePlayerInfo(player);
            callback?.Invoke();
        }
    }
}
