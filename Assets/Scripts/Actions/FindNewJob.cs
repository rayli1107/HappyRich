using ScriptableObjects;
using System.Collections.Generic;
using UI.Panels;
using UI.Panels.Templates;

namespace Actions
{
    public class FindNewJob : AbstractAction, ITransactionHandler, IMessageBoxHandler
    {
        private Player _player;
        private Profession _job;

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
}
