using PlayerInfo;
using ScriptableObjects;
using System;
using System.Collections.Generic;
using UI.Panels.Templates;

namespace Events.Personal
{
    public class JobLossEvent
    {
        private Action _callback;

        public JobLossEvent(Action eventDoneCallback)
        {
            _callback = eventDoneCallback;
        }

        private Profession findJob(Player player)
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

        public void Run()
        {
            Player player = GameManager.Instance.player;
            Profession job = findJob(player);
            if (job == null)
            {
                _callback?.Invoke();
                return;
            }

            player.LoseJob(job);
            List<string> messages = new List<string>();
            messages.Add("Personal Event:");
            messages.Add(string.Format("You lost your job as a {0}.", job.professionName));
            UI.UIManager.Instance.ShowSimpleMessageBox(
                string.Join("\n", messages), ButtonChoiceType.NONE, messageBoxHandler);
        }

        private void messageBoxHandler(ButtonType button)
        {
            UI.UIManager.Instance.UpdatePlayerInfo(GameManager.Instance.player);
            _callback?.Invoke();
        }
    }
}
