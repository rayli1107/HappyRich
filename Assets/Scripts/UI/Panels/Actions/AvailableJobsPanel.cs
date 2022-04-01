using Actions;
using Assets;
using PlayerInfo;
using ScriptableObjects;
using System;
using System.Collections.Generic;
using TMPro;
using UI.Panels.Templates;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Panels.Assets
{
    public struct AvailableJobContext
    {
        public Profession job { get; private set; }
        public Action<Action<bool>> applyAction { get; private set; }

        public AvailableJobContext(
            Profession job,
            Action<Action<bool>> applyAction)
        {
            this.job = job;
            this.applyAction = applyAction;
        }
    }

    public class AvailableJobsPanel : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField]
        private Transform _content;
        [SerializeField]
        private ItemValuePanel _panelAvailableCash;
        [SerializeField]
        private ItemValueListPanel  _prefabApplyButton;
#pragma warning restore 0649

        public Player player;

        private void Awake()
        {
            GetComponent<MessageBox>().confirmMessageHandler = GetConfirmMessage;
        }

        private void buyCallback(bool success)
        {
            if (success)
            {
                GetComponent<MessageBox>().OnButtonOk();
            }
        }

        public virtual void Initialize(
            Player player,
            List<AvailableJobContext> applyActions,
            bool showCashLabel)
        {
            Localization local = Localization.Instance;

            if (_panelAvailableCash != null)
            {
                _panelAvailableCash.gameObject.SetActive(showCashLabel);
                if (showCashLabel)
                {
                    _panelAvailableCash.SetValue(local.GetCurrency(player.cash));
                }
            }

            foreach (ItemValueListPanel panel in
                _content.GetComponentsInChildren<ItemValueListPanel>())
            {
                panel.transform.parent = null;
                GameObject.Destroy(panel);
            }

            foreach (AvailableJobContext context in applyActions)
            {
                Profession job = context.job;

                ItemValueListPanel panel = Instantiate(_prefabApplyButton, _content);
                int tabCount = panel.firstItemValuePanel.tabCount + 1;
                panel.firstItemValuePanel.label = local.GetJobName(job);
                if (!player.oldJobs.Contains(context.job))
                {
                    panel.AddItemValue(
                        "Training Cost:", tabCount, local.GetCurrency(job.jobCost, true));
                }

                panel.AddItemValue(
                        "Annual Salary:", tabCount, local.GetCurrency(job.salary));

                if (job.fullTime)
                {
                    float chance = JobManager.Instance.GetJobSuccessChance(player, job);
                    panel.AddItemValue(
                        "Success Chance:", tabCount, local.GetPercentPlain(chance, false));
                }

                panel.buttonAction = () => context.applyAction(buyCallback);
                panel.gameObject.SetActive(true);
            }
        }

        private string GetConfirmMessage(ButtonType buttonType)
        {
            return buttonType == ButtonType.CANCEL ? "Pass on these job opportunities?" : "";
        }
    }
}
