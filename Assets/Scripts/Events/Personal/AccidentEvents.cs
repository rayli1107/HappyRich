using PlayerInfo;
using PlayerState;
using ScriptableObjects;
using System;
using System.Collections.Generic;
using UI.Panels.Templates;
using UnityEngine;

namespace Events.Personal
{
    public static class CarAccidentEvent
    {
        public static Action<Action> GetEvent(Player player, System.Random random)
        {
            return cb => run(player, random, cb);
        }

        private static void run(Player player, System.Random random, Action callback)
        {
            int loss = Mathf.FloorToInt(
                player.portfolio.car.value * PersonalEventManager.Instance.GetCarAccidentLoss(random));
            List<string> messages = new List<string>();
            messages.Add("Personal Event:");
            messages.Add(
                string.Format(
                    "You got into a minor car accident and had to pay {0} to repair your car.",
                    Localization.Instance.GetCurrency(loss, true)));
            UI.UIManager.Instance.ShowSimpleMessageBox(
                string.Join("\n", messages),
                ButtonChoiceType.OK_ONLY,
                _ => messageBoxHandler(player, loss, callback));
        }

        private static void messageBoxHandler(Player player, int loss, Action callback)
        {
            player.portfolio.AddCash(-1 * loss);
            UI.UIManager.Instance.UpdatePlayerInfo(player);
            callback?.Invoke();
        }
    }
}
