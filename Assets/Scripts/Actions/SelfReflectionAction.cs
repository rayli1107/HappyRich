using PlayerInfo;
using PlayerState;
using System;
using UI.Panels.Templates;
using UnityEngine;

namespace Actions
{
    public static class SelfReflectionAction
    {
        private static void showStateHandler(Player player, Action callback)
        {
            UI.UIManager.Instance.UpdatePlayerInfo(player);
            GameManager.Instance.StateMachine.OnPlayerActionDone();
            callback?.Invoke();
        }

        private static void addPlayerState(
            Player player, AbstractPlayerState state, bool show, Action callback)
        {
            if (state == null)
            {
                state = new MedidatedState(player);
                ++player.meditatedCount;
                UI.UIManager.Instance.ShowSimpleMessageBox(
                    "You didn't learn anything new about yourself, but you got a" +
                    "chance to meditate, bringing you a sense of calm and happiness.",
                    ButtonChoiceType.OK_ONLY,
                    _ => addPlayerState(player, state, false, callback));
                return;
            }

            player.AddMentalState(state);
            if (show)
            {
                UI.UIManager.Instance.ShowPlayerStateInfo(
                    state, _ => showStateHandler(player, callback));
            }
            else
            {
                showStateHandler(player, callback);
            }
        }

        public static void Run(Player player, System.Random random, Action<bool> callback)
        {
            Localization local = Localization.Instance;
            if (player.mentalStates.Exists(s => s is Enlightenment))
            {
                string message = string.Format(
                    "You've already reached a state of {0}. There's nothing more " +
                    "you can learn about yourself.",
                    local.GetPlayerState("Enlightenment"));
                UI.UIManager.Instance.ShowSimpleMessageBox(
                    message,
                    ButtonChoiceType.OK_ONLY,
                    _ => callback?.Invoke(false));
                return;
            }

            Action cb = () => callback?.Invoke(true);

            if (player.meditatedCount >=
                MentalStateManager.Instance.enlightenedThreshold)
            {
                string message = string.Format(
                    "After many years of meditation, you've finally reached a " +
                    "state of {0}.",
                    local.GetPlayerState("Enlightenment"));
                player.RemoveMentalState<MedidatedState>();
                Enlightenment state = new Enlightenment(player);
                UI.UIManager.Instance.ShowSimpleMessageBox(
                    message,
                    ButtonChoiceType.OK_ONLY,
                    _ => addPlayerState(player, state, false, cb));
                return;
            }

            Action<Player, Action<AbstractPlayerState>> addAction =
                MentalStateManager.Instance.GetSelfReflectionState(random);
            addAction?.Invoke(
                player, s => addPlayerState(player, s, true, cb));
        }
    }
}
