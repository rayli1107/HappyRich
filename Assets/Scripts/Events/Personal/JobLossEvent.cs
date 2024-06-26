﻿using PlayerInfo;
using ScriptableObjects;
using System;
using System.Collections.Generic;
using UI.Panels.Templates;

namespace Events.Personal
{
    public static class JobLossEvent
    {
        public static Action<Action> GetEvent(Player player, System.Random random)
        {
            Profession job = findJob(player, random);
            if (job == null)
            {
                return null;
            }

            return cb => Run(player, job, cb);
        }

        private static Profession findJob(Player player, System.Random random)
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
                return player.jobs[random.Next(player.jobs.Count)];
            }
            return null;
        }

        private static void Run(Player player, Profession job, Action callback)
        {
            EventLogManager.Instance.Log("Personal Event: Job Loss Event");
            player.LoseJob(job);
            string message = string.Format(
                "You lost your job as a {0}.", job.professionName);
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
