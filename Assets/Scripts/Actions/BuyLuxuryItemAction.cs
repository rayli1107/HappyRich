using Assets;
using PlayerInfo;
using System;
using UI.Panels.Templates;

namespace Actions
{
    public static class BuyLuxuryItemAction
    {
        public static Action<Action> GetEvent(Player player, LuxuryItem item)
        {
            return cb => run(player, item, cb);
        }

        private static void transactionHandler(
            Player player, LuxuryItem item, Action callback, bool success)
        {
            if (success)
            {
                callback?.Invoke();
            }
            else
            {
                run(player, item, callback);
            }
        }

        private static void messageBoxHandler(
            ButtonType button, Player player, LuxuryItem item, Action callback)
        {
            if (button == ButtonType.OK)
            {
                TransactionManager.BuyLuxuryItem(
                    player,
                    item,
                    b => transactionHandler(player, item, callback, b));
            }
            else
            {
                callback?.Invoke();
            }
        }

        private static void helpHandler()
        {
            Localization local = Localization.Instance;
            string message = string.Format(
                "Buying a {0} provides a temporary boost to your happiness.",
                local.GetLuxuryItem("Luxury Item"));
            UI.UIManager.Instance.ShowSimpleMessageBox(message, ButtonChoiceType.OK_ONLY, null);
        }

        private static void run(
            Player player, LuxuryItem item, Action callback)
        {
            Localization local = Localization.Instance;
            string message = string.Format(
                "You came across an opportunity to purchase a {0} for {1}. Do you want to buy it?",
                local.GetLuxuryItem(item.profile),
                local.GetCurrency(item.value));
            UI.UIManager.Instance.ShowSimpleMessageBox(
                message,
                ButtonChoiceType.OK_CANCEL,
                b => messageBoxHandler(b, player, item, callback),
                null,
                helpHandler);
        }
    }
}
