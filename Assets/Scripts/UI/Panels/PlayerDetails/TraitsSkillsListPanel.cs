using PlayerInfo;
using PlayerState;
using ScriptableObjects;
using System;
using UI.Panels.Templates;
using UnityEngine;

namespace UI.Panels.PlayerDetails
{
    public class TraitsSkillsListPanel : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField]
        private ItemValueListPanel _panelPersonality;
        [SerializeField]
        private ItemValueListPanel _panelTraits;
        [SerializeField]
        private ItemValueListPanel _panelSkills;
#pragma warning restore 0649

        public Player player;

        public void Refresh()
        {
            if (player == null)
            {
                return;
            }

            Localization local = Localization.Instance;

            // Personality
            int tabCount = _panelPersonality.firstItemValuePanel.tabCount + 1;
            ItemValuePanel panel = _panelPersonality.AddItem(
                local.GetPlayerState(player.personality), tabCount);
            panel.clickAction = () => UIManager.Instance.ShowPlayerStateInfo(player.personality, null);

            // Traits
            tabCount = _panelTraits.firstItemValuePanel.tabCount + 1;
            foreach (SelfReflectionState trait in player.selfReflectionStates)
            {
                panel = _panelTraits.AddItem(local.GetPlayerState(trait), tabCount);
                panel.clickAction = () => UIManager.Instance.ShowPlayerStateInfo(trait, null);
            }
            _panelTraits.ActivateIfNonEmpty();

            // Skills
            tabCount = _panelSkills.firstItemValuePanel.tabCount + 1;
            foreach (SkillInfo skillInfo in player.skills)
            {
                panel = _panelSkills.AddItem(local.GetSkill(skillInfo), tabCount);
                panel.clickAction = () => UIManager.Instance.ShowSkillInfo(skillInfo, null);
            }
            _panelSkills.ActivateIfNonEmpty();
        }

        private void OnEnable()
        {
            Refresh();
        }
    }
}
