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
    public class AnnualDetailedLogPanel : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField]
        private ItemValuePanel _panelAge;
        [SerializeField]
        private ItemValueListPanel _panelStartingSnapshot;
        [SerializeField]
        private ItemValueListPanel _panelYearEndSnapshot;
        [SerializeField]
        private ItemValueListPanel _panelEvents;
#pragma warning restore 0649

        public EventLogYearContext eventLog;

        private void writeSnapshot(
            ItemValueListPanel panel,
            EventSnapshot snapshot)
        {
            if (panel == null)
            {
                return;
            }

            panel.gameObject.SetActive(snapshot != null);
            if (snapshot != null)
            {
                Localization local = Localization.Instance;
                int tabCount = panel.firstItemValuePanel.tabCount + 1;
                panel.Clear();
                panel.AddItemValue(
                    "Networth", tabCount, local.GetCurrency(snapshot.snapshot.netWorth));
                panel.AddItemValue(
                    "Cash", tabCount, local.GetCurrency(snapshot.cash));
                panel.AddItemValue(
                    "Cashflow", tabCount, local.GetIncomeRange(snapshot.snapshot.totalCashflowRange));
                panel.AddItemValue(
                    "Happiness", tabCount, snapshot.happiness);
                panel.AddItemValue(
                    "Financial Independence",
                    tabCount,
                    string.Format("{0}%", snapshot.snapshot.financialIndependenceProgress));
            }
        }

        private void writeEvents()
        {
            if (_panelEvents == null)
            {
                return;
            }

            int tabCount = _panelEvents.firstItemValuePanel.tabCount + 1;
            _panelEvents.Clear();
            eventLog.messages.ForEach(
                entry =>
                {
                    ItemValuePanel panel = _panelEvents.AddItem(entry.Item2, tabCount);
                    panel.clickAction = entry.Item1;
                });
            _panelEvents.ActivateIfNonEmpty();
        }

        public void Refresh()
        {
            if (eventLog == null)
            {
                return;
            }

            if (_panelAge != null)
            {
                _panelAge.SetValue(eventLog.age);
            }
            writeSnapshot(_panelStartingSnapshot, eventLog.yearStartSnapshot);
            writeSnapshot(_panelYearEndSnapshot, eventLog.yearEndSnapshot);
            writeEvents();
        }

        private void OnEnable()
        {
            Refresh();
        }
    }
}
