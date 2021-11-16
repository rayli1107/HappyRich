using Assets;
using PlayerInfo;
using System;
using UI;
using UI.Panels.Templates;

namespace Actions
{
    public static class BuyHealthInsuranceAction 
    {
        public static void Run(Player player, Action callback)
        {
            string message = string.Format(
                "Do you want to purchase health insurance for {0} a year?",
                PersonalEventManager.Instance.healthInsuranceCost);
            UI.UIManager.Instance.ShowSimpleMessageBox(
                message,
                ButtonChoiceType.OK_CANCEL,
                b => messageBoxHandler(b, player, callback));
        }

        private static void messageBoxHandler(ButtonType buttonType, Player player, Action callback)
        {
            if (buttonType == ButtonType.OK)
            {
                player.portfolio.hasHealthInsurance = true;
                UI.UIManager.Instance.UpdatePlayerInfo(player);
            }
            callback?.Invoke();
        }
    }


    public static class CancelHealthInsurance
    {
        public static void Run(Player player, Action callback)
        {
            string message = string.Format(
                "Do you want to cancel your health insurance?",
                PersonalEventManager.Instance.healthInsuranceCost);
            UI.UIManager.Instance.ShowSimpleMessageBox(
                message,
                ButtonChoiceType.OK_CANCEL,
                b => messageBoxHandler(b, player, callback));
        }

        private static void messageBoxHandler(ButtonType buttonType, Player player, Action callback)
        {
            if (buttonType == ButtonType.OK)
            {
                player.portfolio.hasHealthInsurance = false;
                UI.UIManager.Instance.UpdatePlayerInfo(player);
            }
            callback?.Invoke();
        }
    }
}
