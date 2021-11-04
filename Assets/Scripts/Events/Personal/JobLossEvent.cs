using PlayerInfo;
using ScriptableObjects;
using System;
using System.Collections.Generic;
using UI.Panels.Templates;

namespace Events.Personal
{
    public static class JobLossEvent
    {
        public static List<Action<Action>> GetEvents(Player player)
        {
            List<Action<Action>> events = new List<Action<Action>>();
            events.Add((Action cb) => Run(player, cb));
            return events;
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

        private static void Run(Player player, Action callback)
        {
            Profession job = findJob(player);
            if (job == null)
            {
                callback?.Invoke();
                return;
            }

            player.LoseJob(job);
            List<string> messages = new List<string>();
            messages.Add("Personal Event:");
            messages.Add(string.Format("You lost your job as a {0}.", job.professionName));
            UI.UIManager.Instance.ShowSimpleMessageBox(
                string.Join("\n", messages), ButtonChoiceType.NONE,
                (ButtonType b) => messageBoxHandler(b, callback));
        }

        private static void messageBoxHandler(ButtonType button, Action callback)
        {
            UI.UIManager.Instance.UpdatePlayerInfo(GameManager.Instance.player);
            callback?.Invoke();
        }
    }
}
