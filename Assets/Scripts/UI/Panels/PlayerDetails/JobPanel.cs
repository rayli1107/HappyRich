using Actions;
using ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Panels.PlayerDetails
{
    public enum JobPanelMode
    {
        kQuit,
        kApply
    }

    public class JobPanel : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField]
        private TextMeshProUGUI _textJob;
        [SerializeField]
        private TextMeshProUGUI _textSalary;
        [SerializeField]
        private Button _buttonQuit;
        [SerializeField]
        private Button _buttonApply;
#pragma warning restore 0649

        public Player player;
        public Profession job;
        public JobPanelMode mode = JobPanelMode.kQuit;

        public void Refresh()
        {
            if (job != null)
            {
                Localization local = Localization.Instance;
                _textJob.text = local.GetJobName(job);
                _textSalary.text = local.GetCurrency(job.salary);
                _buttonQuit.gameObject.SetActive(mode == JobPanelMode.kQuit);
                _buttonApply.gameObject.SetActive(mode == JobPanelMode.kApply);
            }
        }

        private void OnEnable()
        {
            Refresh();
        }

        public void OnQuitButton()
        {
            if (player != null && job != null)
            {
                new QuitJob(player, job, OnQuitActionCallback).Start();
            }
        }

        public void OnQuitActionCallback(bool success)
        {
            if (success)
            {
                JobListPanel panel = GetComponentInParent<JobListPanel>();
                if (panel != null)
                {
                    panel.Refresh();
                }
            }
        }
    }
}
