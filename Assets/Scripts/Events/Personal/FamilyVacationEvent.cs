using PlayerInfo;
using PlayerState;
using ScriptableObjects;
using System;
using System.Collections.Generic;
using UI.Panels.Templates;

namespace Events.Personal
{
    public static class FamilyVacationEvent
    {
        public static Action<Action> GetEvent(Player player)
        {
            if (player.mentalStates.Exists(s => s is FamilyVacationHappinessState) ||
                player.numChild <= 0) 
            {
                return null;
            }

            return cb => run(player, cb);
        }

        private static void run(Player player, Action callback)
        {
            UI.UIManager.Instance.ShowSimpleMessageBox(
                "You decided to take a vacation and spend some time with your family",
                ButtonChoiceType.OK_ONLY,
                _ => messageBoxHandler(player, callback));
        }

        private static void messageBoxHandler(Player player, Action callback)
        {
            player.AddMentalState(new FamilyVacationHappinessState(player));
            UI.UIManager.Instance.UpdatePlayerInfo(player);
            callback?.Invoke();
        }
    }
}
