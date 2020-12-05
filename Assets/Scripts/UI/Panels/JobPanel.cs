using ScriptableObjects;
using TMPro;
using UnityEngine;
using Actions;

namespace UI.Panels
{
    public class JobPanel : MonoBehaviour, IActionCallback
    {
#pragma warning disable 0649
        [SerializeField]
        private TextMeshProUGUI _text;
#pragma warning restore 0649

        public Player player;
        public Profession job;

        private void OnEnable()
        {
            _text.text = string.Format(
                "{0}\n{1}",
                job.professionName,
                GameManager.Instance.Localization.GetCurrency(job.salary));
        }

        public void OnQuitButton()
        {
            new QuitJob(player, job, this).Start();
        }

        public void OnActionCallback(bool success)
        {
            if (success)
            {
                JobListPanel panel = GetComponentInParent<JobListPanel>();
                if (panel != null)
                {
                    panel.Restart();
                }
            }
        }
    }
}
