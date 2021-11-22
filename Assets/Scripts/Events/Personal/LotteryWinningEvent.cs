using PlayerInfo;
using PlayerState;
using ScriptableObjects;
using System;
using System.Collections.Generic;
using UI.Panels.Templates;

namespace Events.Personal
{
    public static class LotteryWinningEvent
    {
        public static Action<Action> GetEvent(Player player, System.Random random)
        {
            return cb => run(player, random, cb);
        }

        private static void run(Player player, System.Random random, Action callback)
        {
            int winning = PersonalEventManager.Instance.GetLotteryWinning(random);
            string message = string.Format(
                "You played the lottery and won {0}!",
                Localization.Instance.GetCurrency(winning));
            UI.UIManager.Instance.ShowSimpleMessageBox(
                message,
                ButtonChoiceType.OK_ONLY,
                _ => messageBoxHandler(player, winning, callback));
        }

        private static void messageBoxHandler(Player player, int winning, Action callback)
        {
            player.portfolio.AddCash(winning);
            UI.UIManager.Instance.UpdatePlayerInfo(player);
            callback?.Invoke();
        }
    }
}
