using PlayerInfo;
using PlayerState;
using System;
using UI.Panels.Templates;
using UnityEngine;

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
            panel.label = label;
            panel.tabCount = 1;
            panel.clickAction = clickAction;
            panel.SetValueAsChange(value);
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
            _panelTotalHappiness.SetValuePlain(totalHappiness);
        }

        private void OnEnable()
        {
            Refresh();
        }
    }
}
