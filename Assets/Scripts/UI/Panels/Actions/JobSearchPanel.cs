using Actions;
using UnityEngine;

namespace UI.Panels.Actions
{
    public class JobSearchPanel : MonoBehaviour
    {
        public Player player;

        public void OnNewJobButton()
        {
            UIManager.Instance.DestroyAllModal();
            new FindNewJob(player).Start();
        }

        public void OnApplyOldJobButton()
        {
            UIManager.Instance.ShowJobListPanel(
                PlayerDetails.JobListPanelMode.kOldJobs);
        }
    }
}
