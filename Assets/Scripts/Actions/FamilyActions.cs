﻿using PlayerInfo;
using PlayerState;
using System;
using System.Collections.Generic;
using UI.Panels.Templates;

namespace Actions
{
    public static class MarriageAction
    {
        public static Action<Action> GetEvent(Player player, Spouse spouse)
        {
            return cb => run(player, spouse, cb);
        }

        private static void showSpouseDetails(Spouse spouse)
        {
            Localization local = Localization.Instance;
            List<string> text = new List<string>()
            {
                string.Format("Spouse's Income: {0}", local.GetCurrency(spouse.additionalIncome)),
                string.Format("Spouse's Expense: {0}", local.GetCurrency(spouse.additionalExpense, true)),
                string.Format("Additional Happiness: {0}", local.GetValueAsChange(spouse.additionalHappiness))
            };
            SimpleTextMessageBox messageBox = UI.UIManager.Instance.ShowSimpleMessageBox(
                string.Join("\n", text),
                ButtonChoiceType.OK_ONLY,
                null);
            messageBox.text.enableWordWrapping = false;
        }

        private static void run(Player player, Spouse spouse, Action callback)
        {
            player.RemoveMentalState<DivorcedPenaltyState>();
            player.spouse = spouse;

            Localization local = Localization.Instance;
            EventLogManager.Instance.LogFormat(
                "Personal Event: Marriage Income {0} Expense {1} Happiness {2}",
                local.GetCurrency(spouse.additionalIncome),
                local.GetCurrency(spouse.additionalExpense, true),
                local.GetValueAsChange(spouse.additionalHappiness));
            UI.UIManager.Instance.ShowSimpleMessageBox(
                "You met the love of your life at a party and decided to get married. Congratulations!",
                ButtonChoiceType.OK_ONLY,
                (_) => callback?.Invoke(),
                () => showSpouseDetails(spouse));
        }
    }

    public static class NewChildAction
    {
        public static Action<Action> GetEvent(Player player, bool isBoy)
        {
            return (Action cb) => run(player, isBoy, cb);
        }

        private static void showChildDetails(Player player)
        {
            Localization local = Localization.Instance;
            List<string> text = new List<string>()
            {
                string.Format("Expense per child: {0}", local.GetCurrency(player.costPerChild, true)),
                string.Format("Number of children: {0}", player.numChild),
                string.Format(
                    "Total children expense: {0}",
                    local.GetCurrency(player.costPerChild * player.numChild, true))
            };
            SimpleTextMessageBox messageBox = UI.UIManager.Instance.ShowSimpleMessageBox(
                string.Join("\n", text),
                ButtonChoiceType.OK_ONLY,
                null);
            messageBox.text.enableWordWrapping = false;
        }


        private static void run(Player player, bool isBoy, Action callback)
        {
            EventLogManager.Instance.Log("Personal Event: New Child");
            ++player.numChild;
            string gender = isBoy ? "boy" : "girl";
            UI.UIManager.Instance.ShowSimpleMessageBox(
                string.Format("Congratulations! You and your spouse decided to have a baby. It's a {0}!", gender),
                ButtonChoiceType.OK_ONLY,
                (_) => callback?.Invoke());
        }
    }

    public static class DivorceAction
    {
        public static Action<Action> GetEvent(Player player)
        {
            return (Action cb) => run(player, cb);
        }

        private static void run(Player player, Action callback)
        {
            EventLogManager.Instance.Log("Personal Event: Divorce");
            player.AddTimedState(
                new DivorcedPenaltyState(
                    player,
                    FamilyManager.Instance.divorcePenaltyDuration,
                    player.spouse.additionalHappiness));
            player.spouse = null;

            UI.UIManager.Instance.ShowSimpleMessageBox(
                "Unfortunately you and your spouse decided to split due to irreconcilable differences.",
                ButtonChoiceType.OK_ONLY,
                (_) => callback?.Invoke());
        }
    }
}
