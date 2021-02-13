using PlayerInfo;
using ScriptableObjects;
using System.Collections.Generic;
using UI.Panels.Templates;
using UnityEngine;

namespace UI.Panels.PlayerDetails
{
    public enum JobListPanelMode
    {
        kCurrentJobs,
        kOldJobs
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
        public JobListPanelMode mode = JobListPanelMode.kCurrentJobs;

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

            List<Profession> jobs;
            JobPanelMode jobPanelMode;

            if (mode == JobListPanelMode.kCurrentJobs)
            {
                if (player.jobs.Count == 0)
                {
                    GetComponent<MessageBox>().Destroy();
                    UIManager.Instance.ShowSimpleMessageBox(
                        "You are currently unemployed.", ButtonChoiceType.OK_ONLY, null);
                    return;
                }

                jobs = player.jobs;
                jobPanelMode = JobPanelMode.kQuit;
            }
            else
            {
                if (player.oldJobs.Count == 0)
                {
                    GetComponent<MessageBox>().Destroy();
                    UIManager.Instance.ShowSimpleMessageBox(
                        "You don't have any old jobs you can apply for.",
                        ButtonChoiceType.OK_ONLY,
                        null);
                    return;
                }

                jobs = player.oldJobs;
                jobPanelMode = JobPanelMode.kApply;
            }

            foreach (Profession job in jobs)
            {
                JobPanel childPanel = Instantiate(_prefabJobPanel, _content);
                childPanel.player = player;
                childPanel.job = job;
                childPanel.mode = jobPanelMode;
                childPanel.Refresh();
            }
        }
    }
}
