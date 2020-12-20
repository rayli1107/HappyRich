using UnityEngine;

namespace UI.Panels.PlayerDetails
{
    public class PlayerStatusMenuPanel : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField]
        private JobListPanel _prefabJobListPanel;
#pragma warning restore 0649
        public Player player;

        public void ShowAssetLiabilityStatusPanel()
        {
            UIManager.Instance.ShowAssetLiabilityStatusPanel();
        }

        public void ShowIncomeExpenseStatusPanel()
        {
            UIManager.Instance.ShowIncomeExpenseStatusPanel();
        }

        public void ShowJobListPanel()
        {
            Player player = GameManager.Instance.player;
            JobListPanel panel = Instantiate(
                _prefabJobListPanel, UIManager.Instance.transform);
            panel.player = player;
            panel.Refresh();
        }

        public void ShowContactListPanel()
        {
            UIManager.Instance.ShowContactListPanel();
        }
    }
}
