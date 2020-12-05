using ScriptableObjects;
using StateMachine;
using System.Collections.Generic;
using UI.Panels;
using UnityEngine;

namespace Events.Personal
{
    public class JobLossEvent : IEvent, IMessageBoxHandler
    {
        private IEventState _state;

        public JobLossEvent(IEventState state)
        {
            _state = state;
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
                _state.OnEventDone();
                return;
            }

            player.LoseJob(job);
            List<string> messages = new List<string>();
            messages.Add("Personal Event:");
            messages.Add(string.Format("You lost your job as a {0}.", job.professionName));
            UI.UIManager.Instance.ShowSimpleMessageBox(
                string.Join("\n", messages), 48, UI.Panels.ButtonChoiceType.NONE, this);
        }

        public void OnButtonClick(MessageBox msgBox, ButtonType button)
        {
            Object.Destroy(msgBox.gameObject);
            UI.UIManager.Instance.UpdatePlayerInfo(GameManager.Instance.player);
            _state.OnEventDone();
        }
    }
}
