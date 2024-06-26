﻿using PlayerInfo;
using ScriptableObjects;
using UI.Panels.Templates;

namespace Actions
{
    public class QuitJob : AbstractAction
    {
        private Player _player;
        private Profession _job;

        public QuitJob(Player player, Profession job, ActionCallback callback) : base(callback)
        {
            _player = player;
            _job = job;
        }

        public override void Start()
        {
            Localization local = Localization.Instance;

            string message = string.Format(
                "Quit your {0} job?", local.GetJobName(_job));
            UI.UIManager.Instance.ShowSimpleMessageBox(
                message, ButtonChoiceType.OK_CANCEL, messageBoxHandler);
        }

        private void messageBoxHandler(ButtonType button)
        {
            if (button == ButtonType.OK)
            {
                _player.LoseJob(_job);
                UI.UIManager.Instance.UpdatePlayerInfo(_player);
                RunCallback(true);
            }
            else
            {
                RunCallback(false);
            }
        }
    }
}
