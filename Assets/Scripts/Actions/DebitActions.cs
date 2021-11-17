using PlayerInfo;
using System;
using System.Collections.Generic;
using UI.Panels.Templates;
using UnityEngine;

namespace Actions
{
    public static class TryDebit
    {
        private static void messageBoxHandler(
            ButtonType button,
            Player player, 
            int amount,
            Action<bool> handler)
        {
            bool success = false;
            if (button == ButtonType.OK)
            {
                int loanAmount = amount - player.cash;
                player.portfolio.AddCash(-1 * player.cash);
                player.portfolio.AddPersonalLoan(loanAmount);
                success = true;
            }
            handler?.Invoke(success);
        }

        public static void Run(Player player, int amount, Action<bool> handler)
        {
            int loanAmount = Mathf.Max(amount - player.cash, 0);
            int maxLoanAmount = new Snapshot(player).availablePersonalLoanAmount;
            Localization local = Localization.Instance;
            if (loanAmount == 0)
            {
                player.portfolio.AddCash(-1 * amount);
                handler?.Invoke(true);
            }
            else if (loanAmount <= maxLoanAmount)
            {
                int interst = loanAmount * InterestRateManager.Instance.personalLoanRate / 100;
                List<string> message = new List<string>();
                message.Add(string.Format("Take out a personal loan of {0}?",
                    local.GetCurrency(loanAmount, true)));
                message.Add(string.Format("You will need to pay an additional annual interest of {0}",
                    local.GetCurrency(interst, true)));
                UI.UIManager.Instance.ShowSimpleMessageBox(
                    string.Join("\n", message),
                    ButtonChoiceType.OK_CANCEL,
                    b => messageBoxHandler(b, player, amount, handler));
            }
            else
            {
                string message = string.Format(
                    "You'd need to take out a personal loan of {0} but the maximum amount " +
                    "you can borrow is {1}",
                    local.GetCurrency(loanAmount),
                    local.GetCurrency(maxLoanAmount));
                UI.UIManager.Instance.ShowSimpleMessageBox(
                    message,
                    ButtonChoiceType.OK_ONLY,
                    (_) => handler?.Invoke(false));
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
