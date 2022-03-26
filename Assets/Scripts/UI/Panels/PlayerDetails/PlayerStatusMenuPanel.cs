using Actions;
using PlayerInfo;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Panels.PlayerDetails
{
    public class PlayerStatusMenuPanel : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField]
        private Button _buttonContacts;
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
            UIManager.Instance.ShowJobListPanel(JobListPanelMode.kCurrentJobs);
        }

        public void ShowContactListPanel()
        {
            UIManager.Instance.ShowContactListPanel(player.GetPartners());
        }

        public void ShowHappinessListPanel()
        {
            UIManager.Instance.ShowHappinessListPanel();
        }

        public void ShowTraitsSkillListPanel()
        {
            UIManager.Instance.ShowTraitsSkillsListPanel();
        }

        public void ShowEventLogPanel()
        {
            UIManager.Instance.ShowEventLogPanel();
        }

        public void Refresh()
        {
            if (_buttonContacts != null)
            {
                _buttonContacts.gameObject.SetActive(
                    player != null &&
                    player.contacts.Count + player.specialists.Count > 0);
            }
        }

        public void OnEnable()
        {
            Refresh();
        }
    }
}
