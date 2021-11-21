using PlayerInfo;
using System;
using System.Collections.Generic;
using UI.Panels.Templates;
using UnityEngine;

namespace Actions
{
    public static class TryDebit
    {
        private static void raiseHandler(
            Player player,
            int amount,
            bool success,
            Action<bool> callback)
        {
            if (success)
            {
                Debug.Assert(player.portfolio.cash >= amount);
                player.portfolio.AddCash(-1 * amount);
            }
            callback?.Invoke(success);
        }

        public static void Run(Player player, int amount, Action<bool> callback)
        {
            int loanAmount = Mathf.Max(amount - player.cash, 0);
            if (loanAmount == 0)
            {
                raiseHandler(player, amount, true, callback);
            }
            else
            {
                UI.UIManager.Instance.ShowRaisePersonalFundsMessageBox(
                    player, amount, b => raiseHandler(player, amount, b, callback));
            }
        }
    }

    public static class ForceDebit
    {
        private static void messageBoxHandler(
            Player player,
            int amount,
            Action handler)
        {
            int loanAmount = amount - player.cash;
            player.portfolio.AddCash(-1 * player.cash);
            player.portfolio.AddPersonalLoan(loanAmount);
            handler?.Invoke();
        }

        public static void Run(Player player, int amount, Action handler)
        {
            int loanAmount = Mathf.Max(amount - player.cash, 0);
            int maxLoanAmount = new Snapshot(player).availablePersonalLoanAmount;
            Localization local = Localization.Instance;
            if (loanAmount == 0)
            {
                player.portfolio.AddCash(-1 * amount);
                handler?.Invoke();
            }
            else if (loanAmount <= maxLoanAmount)
            {
                int interst = loanAmount * InterestRateManager.Instance.personalLoanRate / 100;
                string message = string.Format(
                    "You had to take out a personal loan of {0} and " +
                    "pay an additional annual interest of {1}.",
                    local.GetCurrency(loanAmount, true),
                    local.GetCurrency(interst, true));
                UI.UIManager.Instance.ShowSimpleMessageBox(
                    message,
                    ButtonChoiceType.OK_ONLY,
                    _ => messageBoxHandler(player, amount, handler));
            }
            else
            {
                string message = string.Format(
                    "Unfortunately you were not able to take out a personal loan " +
                    "to help pay off the {0} you owe.",
                    local.GetCurrency(amount, true));
                UI.UIManager.Instance.ShowSimpleMessageBox(
                    message,
                    ButtonChoiceType.OK_ONLY,
                    _ => messageBoxHandler(player, amount, handler));
            }
        }
    }
}
