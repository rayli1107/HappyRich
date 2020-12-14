using ScriptableObjects;
using UI.Panels;
using UI.Panels.Templates;
using UnityEngine;

namespace Actions
{
    public class QuitJob : AbstractAction, IMessageBoxHandler
    {
        private Player _player;
        private Profession _job;

        public QuitJob(Player player, Profession job, IActionCallback callback) : base(callback)
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
                message, ButtonChoiceType.OK_CANCEL, this);
        }

        public void OnButtonClick(MessageBox msgBox, ButtonType button)
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
//            msgBox.Destroy();
        }
    }
}
