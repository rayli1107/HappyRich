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
        public static void Run(Player player, int amount, Action handler)
        {
            Localization local = Localization.Instance;

            int loanAmount = Mathf.Max(amount - player.cash, 0);
            amount -= loanAmount;

            player.portfolio.AddCash(-1 * amount);
            if (loanAmount > 0)
            {
                player.portfolio.AddPersonalLoan(loanAmount);
            }

            if (loanAmount == 0)
            {
                handler?.Invoke();
            }
            else
            {
                Snapshot snapshot = new Snapshot(player);
                if (snapshot.totalCashflowRange.x >= 0)
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
                        _ => handler?.Invoke());
                }
                else
                {
                    string message = string.Format(
                        "Unfortunately you were not able to take out a personal loan " +
                        "to help pay off the {0} you owe.",
                        local.GetCurrency(loanAmount, true));
                    UI.UIManager.Instance.ShowSimpleMessageBox(
                        message,
                        ButtonChoiceType.OK_ONLY,
                        _ => handler?.Invoke());
                }
            }
        }
    }
}
