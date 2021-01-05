using Actions;
using Assets;
using PlayerState;
using System;
using System.Collections.Generic;
using UI.Panels.Templates;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Panels.PlayerDetails
{
    public class HappinessListPanel : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField]
        private ItemValuePanel _panelTotalHappiness;
        [SerializeField]
        private ItemValuePanel _prefabItemValuePanel;
#pragma warning restore 0649

        public Player player;

        private void Awake()
        {
        }

        private void AddItem(
            Transform parentTranform, string label, int value, Action clickAction)
        {
            if (value == 0)
            {
                return;
            }

            ItemValuePanel panel = Instantiate(_prefabItemValuePanel, parentTranform);
            panel.setLabel(label);
            panel.setValueAsChange(value);
            panel.setTabCount(1);

            if (clickAction != null)
            {
                panel.clickAction = clickAction;
                panel.EnableClick(true);
            }
        }

        public void Refresh()
        {
            if (player == null)
            {
                return;
            }

            Transform parentTransform = _panelTotalHappiness.transform.parent;
            int totalHappiness = player.defaultHappiness;

            AddItem(parentTransform, "Starting Happiness", player.defaultHappiness, null);
            foreach (AbstractPlayerState state in player.states)
            {
                int modifier = state.happinessModifier;
                Action action = () => UIManager.Instance.ShowPlayerStateInfo(state, null);
                AddItem(parentTransform, state.name, modifier, action);
                totalHappiness += modifier;
            }
            _panelTotalHappiness.setValuePlain(totalHappiness);
        }

        private void OnEnable()
        {
            Refresh();
        }
    }
}
