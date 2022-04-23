using PlayerInfo;
using ScriptableObjects;
using System;
using System.Collections.Generic;
using TMPro;
using UI.Panels.Templates;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Panels.PlayerDetails
{
    public class AnnualEventLogPanel : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField]
        private ItemValuePanel _panelAge;
        [SerializeField]
        private ItemValuePanel _panelCash;
        [SerializeField]
        private ItemValuePanel _panelHappiness;
        [SerializeField]
        private ItemValuePanel _panelFinancialProgress;
        [SerializeField]
        private AnnualDetailedLogPanel _prefabAnnualDetailedPanel;
#pragma warning restore 0649

        public EventLogYearContext eventLog;

        public void OnClick()
        {
            AnnualDetailedLogPanel panel = Instantiate(
                _prefabAnnualDetailedPanel, UIManager.Instance.transform);
            panel.eventLog = eventLog;
            panel.gameObject.SetActive(true);
            panel.Refresh();
        }

        public void Refresh()
        {
            if (eventLog == null)
            {
                return;
            }

            Localization local = Localization.Instance;

            if (_panelAge != null)
            {
                _panelAge.SetValue(eventLog.age);
            }

            if (_panelCash != null)
            {
                if (eventLog.yearEndSnapshot != null)
                {
                    _panelCash.SetValue(
                        local.GetCurrencyPlain(eventLog.yearEndSnapshot.cash));
                }
                else
                {
                    _panelCash.RemoveValue();
                }
            }

            if (_panelHappiness != null)
            {
                if (eventLog.yearEndSnapshot != null)
                {
                    _panelHappiness.SetValue(eventLog.yearEndSnapshot.happiness);
                }
                else
                {
                    _panelHappiness.RemoveValue();
                }
            }

            if (_panelFinancialProgress != null)
            {
                if (eventLog.yearEndSnapshot != null)
                {
                    _panelFinancialProgress.SetValue(
                        string.Format(
                            "{0}%",
                            eventLog.yearEndSnapshot.snapshot.financialIndependenceProgress));
                }
                else
                {
                    _panelFinancialProgress.RemoveValue();
                }
            }
        }



        private void OnEnable()
        {
            Refresh();
        }
    }
}
