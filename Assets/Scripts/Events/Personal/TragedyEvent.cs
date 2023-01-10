using Actions;
using PlayerInfo;
using PlayerState;
using ScriptableObjects;
using System;
using System.Collections.Generic;
using UI.Panels.Templates;

namespace Events.Personal
{
    public static class TragedyEvents
    {
        public static Action<Action> GetEvent(Player player, System.Random random)
        {
            if (player.states.Exists(s => s is TragedyPenaltyState))
            {
                return null;
            }

            List<Action<Action>> events = new List<Action<Action>>();
            events.Add((Action cb) => runFamilyTragedyEvent(player, cb));
            if (player.spouse != null)
            {
                events.Add((Action cb) => runSpouseTragedyEvent(player, cb));
            }
            if (player.numChild > 0)
            {
                events.Add((Action cb) => runChildTragedyEvent(player, cb));
            }
            return CompositeActions.GetRandomAction(events, random);
        }

        private static Profession findJob(Player player)
        {
            foreach (Profession job in player.jobs)
            {
                if (job.fullTime)
                {
                    return job;
                }
            }
            if (player.jobs.Count > 0)
            {
                return player.jobs[GameManager.Instance.Random.Next(player.jobs.Count)];
            }
            return null;
        }

        private static void runFamilyTragedyEvent(Player player, Action callback)
        {
            EventLogManager.Instance.Log("Personal Event: Family Tragedy Event");
            string message = "One of your close relatives passed away due to a tragic accident. " +
                "Sorry for your loss.";
            player.AddTimedState(
                new TragedyPenaltyState(player, MentalStateManager.Instance.tragedyDuration));
            UI.UIManager.Instance.ShowSimpleMessageBox(
                message,
                ButtonChoiceType.OK_ONLY,
                (ButtonType b) => messageBoxHandler(b, callback));
        }

        private static void runSpouseTragedyEvent(Player player, Action callback)
        {
            EventLogManager.Instance.Log("Personal Event: Spouse Tragedy Event");
            string message = "Your spouse recently passed away due to a tragic accident. " +
                "Sorry for your loss.";
            player.spouse = null;
            player.AddTimedState(
                new TragedyPenaltyState(player, MentalStateManager.Instance.tragedyDuration));
            UI.UIManager.Instance.ShowSimpleMessageBox(
                message,
                ButtonChoiceType.OK_ONLY,
                (ButtonType b) => messageBoxHandler(b, callback));
        }

        private static void runChildTragedyEvent(Player player, Action callback)
        {
            EventLogManager.Instance.Log("Personal Event: Child Tragedy Event");
            string message = "One of your children recently passed away due to a tragic accident. " +
                "Sorry for your loss.";
            player.numChild = Math.Max(player.numChild - 1, 0);
            player.AddTimedState(
                new TragedyPenaltyState(player, MentalStateManager.Instance.tragedyDuration));
            UI.UIManager.Instance.ShowSimpleMessageBox(
                message,
                ButtonChoiceType.OK_ONLY,
                (ButtonType b) => messageBoxHandler(b, callback));
        }

        private static void messageBoxHandler(ButtonType button, Action callback)
        {
            UI.UIManager.Instance.UpdatePlayerInfo(GameManager.Instance.player);
            callback?.Invoke();
        }
    }
}
