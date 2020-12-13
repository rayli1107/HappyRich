using ScriptableObjects;
using TMPro;
using UnityEngine;
using Actions;

namespace UI.Panels.PlayerDetails
{
    public class JobPanel : MonoBehaviour, IActionCallback
    {
#pragma warning disable 0649
        [SerializeField]
        private TextMeshProUGUI _textJob;
        [SerializeField]
        private TextMeshProUGUI _textSalary;
#pragma warning restore 0649

        public Player player;
        public Profession job;

        public void Refresh()
        {
            if (job != null)
            {
                _textJob.text = job.professionName;

                Localization local = GameManager.Instance.Localization;
                _textSalary.text = local.GetCurrency(job.salary);
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
                new QuitJob(player, job, this).Start();
            }
        }

        public void OnActionCallback(bool success)
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
