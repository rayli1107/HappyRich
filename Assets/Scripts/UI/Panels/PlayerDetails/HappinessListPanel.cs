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
        private ItemValueListPanel _panelModifiers;
#pragma warning restore 0649

        public Player player;

        private void Awake()
        {
        }

        public void Refresh()
        {
            if (player == null)
            {
                return;
            }

            int totalHappiness = player.defaultHappiness;
            int tabCount = _panelModifiers.firstItemValuePanel.tabCount + 1;
            _panelModifiers.AddItemValue("Default Happiness", tabCount, player.defaultHappiness);

            foreach (AbstractPlayerState state in player.states)
            {
                int modifier = state.happinessModifier;
                if (modifier != 0)
                {
                    ItemValuePanel panel = _panelModifiers.AddItemValue(
                        state.name, tabCount, modifier);
                    panel.clickAction = () => UIManager.Instance.ShowPlayerStateInfo(state, null);
                    totalHappiness += modifier;
                }
            }

            _panelTotalHappiness.SetValue(totalHappiness);
        }

        private void OnEnable()
        {
            Refresh();
        }
    }
}
