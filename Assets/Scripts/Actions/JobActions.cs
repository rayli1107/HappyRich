using PlayerInfo;
using ScriptableObjects;
using System;
using System.Collections.Generic;
using UI.Panels.Assets;
using UI.Panels.Templates;

namespace Actions
{
    public class JobActions
    {
        private static void applyJobMessageBoxHandler(
            ButtonType button,
            Player player,
            System.Random random,
            Profession job,
            Action<bool> callback)
        {
            if (button == ButtonType.OK)
            {
                float chance = JobManager.Instance.GetJobSuccessChance(player, job);
                if (job.fullTime && random.NextDouble() < chance)
                {
                    string message = string.Format(
                        "Unfortunately you were not able to get the {0} job.",
                        Localization.Instance.GetJobName(job));
                    UI.UIManager.Instance.ShowSimpleMessageBox(
                        message,
                        ButtonChoiceType.OK_ONLY,
                        _ => callback?.Invoke(true));
                }
                else
                {
                    TransactionManager.ApplyJob(
                        player, job, b => onTransactionFinish(b, job, callback));

                }
            }
            else
            {
                callback?.Invoke(false);
            }
        }

        private static void onTransactionFinish(
            bool success, Profession job, Action<bool> callback)
        {
            if (success)
            {
                string message = string.Format(
                    "Congratuations! You've successfully applied to the {0} job.",
                    Localization.Instance.GetJobName(job));
                UI.UIManager.Instance.ShowSimpleMessageBox(
                    message, ButtonChoiceType.OK_ONLY, _ => callback?.Invoke(true));
            }
            else
            {
                callback?.Invoke(false);
            }
        }

        private static void applyJob(
            Player player,
            System.Random random,
            Profession job,
            Action<bool> callback)
        {
            Localization local = Localization.Instance;

            List<string> messages = new List<string>()
            {
                string.Format("Apply for the {0} job?", local.GetJobName(job)),
                "",
            };
            messages.AddRange(formatJobInfo(player, job));

            UI.UIManager.Instance.ShowSimpleMessageBox(
                string.Join("\n", messages),
                ButtonChoiceType.OK_CANCEL,
                b => applyJobMessageBoxHandler(b, player, random, job, callback));
        }

        private static List<string> formatJobInfo(Player player, Profession job)
        {
            Localization local = Localization.Instance;

            List<string> results = new List<string>();
            if (!player.oldJobs.Contains(job))
            {
                results.Add(string.Format(
                    "Training Cost: {0}", local.GetCurrency(job.jobCost, true)));
            }
            results.Add(string.Format("Annual Salary: {0}", local.GetCurrency(job.salary)));
            if (job.fullTime)
            {
                float chance = JobManager.Instance.GetJobSuccessChance(player, job);
                results.Add(string.Format(
                    "Sucess Chance: {0}",
                    local.GetPercentPlain(chance, false)));
            }
            return results;
        }

        private static string getApplyLabel(Player player, Profession job)
        {
            List<string> results = new List<string>();
            results.Add(Localization.Instance.GetJobName(job));
            results.AddRange(formatJobInfo(player, job));
            return string.Join("\n", results);
        }

        private static void showAvailableJobsHandler(ButtonType buttonType, Player player)
        {
            if (buttonType == ButtonType.OK)
            {
                UI.UIManager.Instance.UpdatePlayerInfo(player);
                UI.UIManager.Instance.DestroyAllModal();
                GameManager.Instance.StateMachine.OnPlayerActionDone();
            }
        }

        private static string getJobApplyLabel()
        {
            List<string> messages = new List<string>()
            {
                "You can apply to one of the following jobs.",
                "",
                string.Format(
                    "Available Cash: {0}",
                    Localization.Instance.GetCurrency(GameManager.Instance.player.cash))
            };
            return string.Join("\n", messages);
        }

        private static void searchJobs(
            Player player, System.Random random, List<Profession> availableJobs)
        {
            if (player.jobs.Count >= JobManager.Instance.maxAllowedJobs)
            {
                UI.UIManager.Instance.ShowSimpleMessageBox(
                    "You are working too many jobs already!",
                    ButtonChoiceType.OK_ONLY,
                    null);
            }
            else if (availableJobs.Count == 0)
            {
                UI.UIManager.Instance.ShowSimpleMessageBox(
                    "Unfortunately you could not find any available jobs you can apply for.",
                    ButtonChoiceType.OK_ONLY,
                    null);
            }
            else
            {
                List<AvailableActionContext> actions = new List<AvailableActionContext>();
                foreach (Profession job in availableJobs)
                {
                    actions.Add(new AvailableActionContext(
                        getApplyLabel(player, job),
                        cb => applyJob(player, random, job, cb)));
                }

                UI.UIManager.Instance.ShowAvailableActionsPanel(
                    actions,
                    getJobApplyLabel,
                    b => showAvailableJobsHandler(b, player), 1);
            }
        }

        public static void SearchNewJobs(Player player, System.Random random)
        {
            searchJobs(player, random, JobManager.Instance.GetAvailableNewJobs(player));
        }

        public static void SearchOldJobs(Player player, System.Random random)
        {
            List<Profession> availableJobs = player.oldJobs;
            if (player.jobs.Exists(j => j.fullTime))
            {
                availableJobs = availableJobs.FindAll(j => !j.fullTime);
            }

            searchJobs(player, random, availableJobs);
        }
    }
}