﻿using Actions;
using PlayerInfo;
using PlayerState;
using ScriptableObjects;
using System;
using System.Collections.Generic;
using UI.Panels.Templates;
using UnityEngine;

namespace Events.Personal
{
    public static class PersonalAccidentEvent
    {
        public static Action<Action> GetEvent(Player player, System.Random random)
        {
            if (player.portfolio.hasHealthInsurance)
            {
                return cb => runWithInsurance(player, random, cb);
            }
            else
            {
                return cb => runWithoutInsurance(player, random, cb);
            }
        }

        private static void runWithInsurance(
           Player player, System.Random random, Action callback)
        {
            int original = PersonalEventManager.Instance.GetPersonalAccidentLoss(random);
            int loss = PersonalEventManager.Instance.insuranceOutOfPocket;
            string formattedLoss = Localization.Instance.GetCurrency(loss, true);
            EventLogManager.Instance.LogFormat(
                "Personal Event: Personal Accident {0}", formattedLoss);
            string message = string.Format(
                "You injured yourself and had to pay {0} for surgery. " +
                "Fortunately since you have insurance, you only need to pay {1} " +
                "out of pocket.",
                Localization.Instance.GetCurrency(original, true),
                formattedLoss);
            UI.UIManager.Instance.ShowSimpleMessageBox(
                message,
                ButtonChoiceType.OK_ONLY,
                _ => ForceDebit.Run(
                    player, loss, () => debitHandler(player, callback)));
        }

        private static void runWithoutInsurance(
            Player player, System.Random random, Action callback)
        {
            int loss = PersonalEventManager.Instance.GetPersonalAccidentLoss(random);
            string message = string.Format(
                "You injured yourself and had to pay {0} for surgery. " +
                "Unfortunately since you don't have insurance you had to pay " +
                "the full amount.",
                Localization.Instance.GetCurrency(loss, true));
            UI.UIManager.Instance.ShowSimpleMessageBox(
                message,
                ButtonChoiceType.OK_ONLY,
                _ => ForceDebit.Run(
                    player, loss, () => debitHandler(player, callback)));
        }
        private static void debitHandler(Player player, Action callback)
        {
            UI.UIManager.Instance.UpdatePlayerInfo(player);
            callback?.Invoke();
        }

    }
    public static class CarAccidentEvent
    {
        public static Action<Action> GetEvent(Player player, System.Random random)
        {
            return cb => run(player, random, cb);
        }

        private static void run(Player player, System.Random random, Action callback)
        {
            int loss = Mathf.FloorToInt(
                player.portfolio.car.value *
                PersonalEventManager.Instance.GetCarAccidentLoss(random));
            string formattedLoss = Localization.Instance.GetCurrency(loss, true);
            EventLogManager.Instance.LogFormat(
                "Personal Event: Car Accident Event {0}", formattedLoss);
            string message = string.Format(
                "You got into a minor car accident and had to pay {0} to repair your car.",
                formattedLoss);
            UI.UIManager.Instance.ShowSimpleMessageBox(
                message,
                ButtonChoiceType.OK_ONLY,
                _ => ForceDebit.Run(
                    player, loss, () => debitHandler(player, callback)));
        }

        private static void debitHandler(Player player, Action callback)
        {
            UI.UIManager.Instance.UpdatePlayerInfo(player);
            callback?.Invoke();
        }
    }
}
