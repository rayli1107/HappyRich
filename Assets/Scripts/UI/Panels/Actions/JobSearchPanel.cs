using Actions;
using PlayerInfo;
using UnityEngine;

namespace UI.Panels.Actions
{
    public class JobSearchPanel : MonoBehaviour
    {
        public Player player;

        public void OnNewJobButton()
        {
            JobActions.SearchNewJobs(
                GameManager.Instance.player, GameManager.Instance.Random);
        }

        public void OnApplyOldJobButton()
        {
            JobActions.SearchOldJobs(
                GameManager.Instance.player, GameManager.Instance.Random);
//            UIManager.Instance.ShowJobListPanel(
                //PlayerDetails.JobListPanelMode.kOldJobs);
        }
    }
}
