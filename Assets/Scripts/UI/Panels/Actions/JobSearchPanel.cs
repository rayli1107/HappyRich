using Actions;
using PlayerInfo;
using UnityEngine;

namespace UI.Panels.Actions
{
    public class JobSearchPanel : MonoBehaviour
    {
        public Player player;

        private TutorialAction _tutorialAction => TutorialManager.Instance.JobSearchOnce;

        public void OnNewJobButton()
        {
            JobActions.SearchNewJobs(
                GameManager.Instance.player, GameManager.Instance.Random);
        }

        public void OnApplyOldJobButton()
        {
            JobActions.SearchOldJobs(
                GameManager.Instance.player, GameManager.Instance.Random);
        }

        public void OnHelpButton()
        {
            _tutorialAction.ForceRun(null);
        }

        private void OnEnable()
        {
            _tutorialAction.Run(null);
        }
    }
}
