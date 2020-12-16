using UI.Panels.Templates;
using UnityEngine;

namespace UI.Panels.PlayerDetails
{
    public class JobListPanelDestroy : IMessageBoxHandler
    {
        private MessageBox _messageBox;

        public JobListPanelDestroy(MessageBox messageBox)
        {
            _messageBox = messageBox;
        }

        public void OnButtonClick(ButtonType button)
        {
            _messageBox.Destroy();
        }
    }

    public class JobListPanel : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField]
        private Transform _content;
        [SerializeField]
        private JobPanel _prefabJobPanel;
#pragma warning restore 0649

        public Player player;

        private void OnEnable()
        {
            Refresh();
        }

        public void Refresh()
        {
            foreach (Transform transform in _content)
            {
                Destroy(transform.gameObject);
            }

            if (player == null)
            {
                return;
            }

            if (player.jobs.Count == 0)
            {
                JobListPanelDestroy handler = new JobListPanelDestroy(
                    GetComponent<MessageBox>());
                UIManager.Instance.ShowSimpleMessageBox(
                    "You are currently unemployed.", ButtonChoiceType.OK_ONLY, handler);
                return;
            }

            for (int i = 0; i < player.jobs.Count; ++i)
            {
                JobPanel childPanel = Instantiate(_prefabJobPanel, _content);
                childPanel.player = player;
                childPanel.job = player.jobs[i];
                childPanel.Refresh();
            }
        }
    }
}
