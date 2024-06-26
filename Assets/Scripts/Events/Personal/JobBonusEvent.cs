﻿using PlayerInfo;
using PlayerState;
using ScriptableObjects;
using System;
using System.Collections.Generic;
using UI.Panels.Templates;

namespace Events.Personal
{
    public static class JobBonusEvent
    {
        public static Action<Action> GetEvent(Player player, System.Random random)
        {
            Profession job = findJob(player, random);
            if (job == null)
            {
                return null;
            }
            return cb => run(player, job, random, cb);
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
            return null;
        }

        private static void run(Player player, Profession job, System.Random random, Action callback)
        {
            int bonus = JobManager.Instance.GetJobBonus(job, random);
            string formattedBonus = Localization.Instance.GetCurrency(bonus);
            EventLogManager.Instance.LogFormat(
                "Personal Event: Job Bonus {0}", formattedBonus);
            string message = string.Format(
                "Your boss decided to give you an year end bonus of {0} " +
                "because of your exemplary performance at work.",
                formattedBonus);
            UI.UIManager.Instance.ShowSimpleMessageBox(
                message,
                ButtonChoiceType.OK_ONLY,
                _ => messageBoxHandler(player, bonus, callback));
        }

        private static void messageBoxHandler(Player player, int bonus, Action callback)
        {
            player.portfolio.AddCash(bonus);
            UI.UIManager.Instance.UpdatePlayerInfo(player);
            callback?.Invoke();
        }
    }
}
