using Actions;
using PlayerInfo;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Panels.Actions
{
    public class JobSearchPanel : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField]
        private Button _buttonNewJob;
        [SerializeField]
        private Button _buttonOldJob;
#pragma warning restore 0649

        public Player player;

        private TutorialAction _tutorialAction => TutorialManager.Instance.JobSearchOnce;
        public Button buttonNewJob => _buttonNewJob;
        public Button buttonOldJob => _buttonOldJob;
        

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
//            _tutorialAction.Run(null);
        }
    }
}
