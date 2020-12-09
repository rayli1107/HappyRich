using UnityEngine;

namespace UI.Panels
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
/*
            Player player = GameManager.Instance.player;
            if (player.jobs.Count > 0)
            {
                JobListPanel panel = Instantiate<JobListPanel>(_prefabJobListPanel);
                panel.GetComponent<JobListPanel>().player = player;
                UIManager.ShowMessageBox(panel.gameObject, null, ButtonChoiceType.NONE);
            }
            else
            {
                ShowSimpleMessageBox(
                    "You are currently unemployed.", ButtonChoiceType.OK_ONLY, null);
            }
            */
        }
    }
}
