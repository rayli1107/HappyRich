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
    public class EventLogPanel : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField]
        private AnnualEventLogPanel _prefabAnnualEventLogPanel;
        [SerializeField]
        private VerticalLayoutGroup _content;
#pragma warning restore 0649

        public void Refresh()
        {
            for (int i = _content.transform.childCount - 1; i >= 0; --i)
            {
                DestroyImmediate(_content.transform.GetChild(i).gameObject);
            }
            foreach (EventLogYearContext log in EventLogManager.Instance.annualEventLogs)
            {
                AnnualEventLogPanel panel = Instantiate(
                    _prefabAnnualEventLogPanel, _content.transform);
                panel.eventLog = log;
                panel.gameObject.SetActive(true);
                panel.Refresh();
            }
        }

        private void OnEnable()
        {
            Refresh();
        }
    }
}
