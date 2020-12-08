using ScriptableObjects;
using System.Collections.Generic;
using Transaction;
using UI.Panels;
using UnityEngine;

namespace Actions
{
    public class FindNewJob : AbstractAction, ITransactionHandler, IMessageBoxHandler
    {
        private Player _player;
        private MessageBox _msgBox;
        private Profession _job;

        public FindNewJob(Player player) : base(null)
        {
            _player = player;
        }

        public void OnButtonClick(MessageBox msgBox, ButtonType button)
        {
            if (button == ButtonType.OK)
            {
                _msgBox = msgBox;
                GameManager.Instance.TryDebit(_player, _job.jobCost, this);
            }
            else
            {
                msgBox.Destroy();
                GameManager.Instance.StateMachine.OnPlayerActionDone();
            }
        }

        public void OnTransactionFailure()
        {
            RunCallback(false);
        }

        public void OnTransactionSuccess()
        {
            _msgBox.Destroy();
            _player.AddJob(_job);
            UI.UIManager.Instance.UpdatePlayerInfo(_player);
            GameManager.Instance.StateMachine.OnPlayerActionDone();
            RunCallback(true);
        }

        public override void Start()
        {
            if (_player.jobs.Count > 1)
            {
                return;
            }

            Localization local = GameManager.Instance.Localization;

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

            List<string> message = new List<string>();
            message.Add(_job.professionName);
            message.Add(string.Format(
                "Training Cost: {0}", local.GetCurrency(_job.jobCost)));
            message.Add(string.Format(
                "Salary: {0}", local.GetCurrency(_job.salary)));
            message.Add(string.Format("Apply?"));
            UI.UIManager.Instance.ShowSimpleMessageBox(
                string.Join("\n", message), ButtonChoiceType.OK_CANCEL, this);
        }
    }
}
