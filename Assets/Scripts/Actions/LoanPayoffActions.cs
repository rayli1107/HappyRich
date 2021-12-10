using Assets;
using PlayerInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UI.Panels;
using UI.Panels.Templates;
using UnityEngine;

namespace Actions
{
    public static class LoanPayoffActions
    {
        private const int fontSizeMax = 32;
        private static void messageBoxHandler(Player player, Action callback)
        {
            UI.UIManager.Instance.UpdatePlayerInfo(player);
            callback?.Invoke();
        }

        private static string confirmMessageHandler(
            ButtonType button,
            int number)
        {
            if (button == ButtonType.OK)
            {
                return string.Format(
                    "Pay {0} towards the loan principal?",
                    Localization.Instance.GetCurrency(number));
            }
            return null;
        }

        public static void PayAssetLoanPrincipal(
            Player player,
            AbstractAsset asset,
            AbstractLiability loan,
            Action callback)
        {
            Localization local = Localization.Instance;
            List<string> messages = asset == null ? loan.GetDetails() : asset.GetDetails();
            messages.Add(
                string.Format(
                    "How much of the loan principal do you want to pay off? " +
                    "(Available cash: {0})",
                    local.GetCurrency(player.portfolio.cash)));
            /*
            string message;
            int loanAmount = liability == null ? 0 : liability.amount;
            if (loanAmount == 0)
            {
                message = string.Format(
                    "You have a {0} asset without any outstanding loans.",
                    local.GetAsset(asset));
                UI.UIManager.Instance.ShowSimpleMessageBox(
                    message, ButtonChoiceType.OK_ONLY, _ => callback?.Invoke());
                return;
            }

            if (asset == null)
            {
                message = string.Format(
                    "You have a {0} of {1}, with an interest rate of {2} and annual " +
                    "interest payment of {3}. How much of the loan principal do you " +
                    "want to pay off? (Available cash: {4})",
                    local.GetLiability(liability),
                    local.GetCurrency(loanAmount, true),
                    local.colorWrap(
                        string.Format("{0}%", liability.interestRate), local.colorNegative),
                    local.GetCurrency(liability.expense, true),
                    local.GetCurrency(player.portfolio.cash));
            }
            else
            {
                message = string.Format(
                    "You have a {0} asset with a {1} of {2} at an interest rate of {3} " +
                    "and annual interest payment of {4}. How much of the loan " +
                    "principal do you want to pay off? (Available cash: {5})",
                    local.GetAsset(asset),
                    local.GetLiability(liability),
                    local.GetCurrency(loanAmount, true),
                    local.colorWrap(
                        string.Format("{0}%", liability.interestRate), local.colorNegative),
                    local.GetCurrency(liability.expense, true),
                    local.GetCurrency(player.portfolio.cash));
            }
            */
            SimpleNumberInputPanel panel = UI.UIManager.Instance.ShowNumberInputPanel(
                string.Join("\n", messages),
                Mathf.Min(loan.amount, player.portfolio.cash),
                (b, n) => messageBoxHandler(player, callback),
                confirmMessageHandler,
                (h, n) => TransactionManager.PayLoanPrincipal(player, loan, n, h));
            panel.text.fontSizeMax = fontSizeMax;
        }
    }
}
