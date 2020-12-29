using ScriptableObjects;
using UI.Panels.Templates;
using UnityEngine;

namespace Actions
{
    public class ApplyOldJob : AbstractAction
    {
        private Player _player;
        private Profession _job;

        private float successChance => (
            _job.searchable ? 1f : JobManager.Instance.applyOldJobSuccessChance);

        public ApplyOldJob(Player player, Profession job, ActionCallback actionCallback)
            : base(actionCallback)
        {
            _player = player;
            _job = job;
        }

        public override void Start()
        {
            Localization local = Localization.Instance;
            string message = string.Format(
                "Apply for your old job as a {0}?",
                local.GetJobName(_job));
            if (successChance < 1)
            {
                string warning = string.Format(
                    "({0} chance of success)",
                    local.GetPercentPlain(successChance, false));
                message = string.Format(
                    "{0} {1}", message, local.GetWarning(warning));
            }
            UI.UIManager.Instance.ShowSimpleMessageBox(
                message, ButtonChoiceType.OK_CANCEL, messageBoxHandler);
        }

        private void messageBoxHandler(ButtonType buttonType)
        {
            if (buttonType == ButtonType.OK)
            {
                bool success = GameManager.Instance.Random.NextDouble() < successChance;
                Localization local = Localization.Instance;
                string message;
                if (success)
                {
                    message = string.Format(
                        "You've successfully applied for your old job as a {0}.",
                        local.GetJobName(_job));
                    _player.AddJob(_job);
                }
                else
                {
                    message = string.Format(
                        "Unfortunately you were not able to apply for your old job as a {0}.",
                        local.GetJobName(_job));
                }
                Debug.LogFormat("Showing message {0}", message);
                GameManager.Instance.StateMachine.OnPlayerActionDone();
                UI.UIManager.Instance.ShowSimpleMessageBox(
                    message, ButtonChoiceType.OK_ONLY, (_) => RunCallback(true));
            }
            else
            {
                RunCallback(false);
            }
        }
    }

    /*
    public class FindNewJob : AbstractAction, ITransactionHandler, IMessageBoxHandler
    {
        private Player _player;

        public FindNewJob(Player player) : base(null)
        {
            _player = player;
        }

        public void OnButtonClick(ButtonType button)
        {
            if (button == ButtonType.OK)
            {
                TransactionManager.ApplyJob(_player, _job, this);
            }
            else
            {
                GameManager.Instance.StateMachine.OnPlayerActionDone();
                RunCallback(false);
            }
        }

        public void OnTransactionFinish(bool success)
        {
            if (success)
            {
                UI.UIManager.Instance.UpdatePlayerInfo(_player);
                GameManager.Instance.StateMachine.OnPlayerActionDone();
                RunCallback(true);
            }
            else
            {
                ShowApplyConfirmation();
            }
        }

        private void ShowApplyConfirmation()
        {
            Localization local = Localization.Instance;

            List<string> messages = new List<string>();
            messages.Add(string.Format("Apply for the {0} job?", local.GetJobName(_job)));
            messages.Add("");
            messages.Add(string.Format(
                "Training Cost: {0}", local.GetCurrency(_job.jobCost, true)));
            messages.Add(string.Format(
                "Salary: {0}", local.GetCurrency(_job.salary)));

            UI.UIManager.Instance.ShowSimpleMessageBox(
                string.Join("\n", messages),
                ButtonChoiceType.OK_CANCEL,
                this);
        }

        public override void Start()
        {
            if (_player.jobs.Count > 1)
            {
                return;
            }

            bool hasFullTime = false;
            foreach (Profession job in _player.jobs)
            {
                if (job.fullTime)
                {
                    hasFullTime = true;
                }
            }
            _job = JobManager.Instance.FindJob(
                GameManager.Instance.Random, hasFullTime);
            ShowApplyConfirmation();
        }
    }
    */
}
