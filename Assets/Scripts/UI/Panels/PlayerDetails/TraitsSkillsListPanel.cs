using Actions;
using Assets;
using PlayerState;
using ScriptableObjects;
using System;
using System.Collections.Generic;
using UI.Panels.Templates;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Panels.PlayerDetails
{
    public class TraitsSkillsListPanel : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField]
        private ItemValuePanel _panelTraits;
        [SerializeField]
        private ItemValuePanel _panelSkills;
        [SerializeField]
        private ItemValuePanel _prefabItemValuePanel;
#pragma warning restore 0649

        public Player player;

        private void AddItem(
            Transform parentTranform, int index, string label, Action clickAction)
        {
            ItemValuePanel panel = Instantiate(_prefabItemValuePanel, parentTranform);
            panel.setLabel(label);
            panel.removeValue();
            panel.setTabCount(1);
            panel.transform.SetSiblingIndex(index);

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

            Localization local = Localization.Instance;
            // Traits
            Transform parentTransform = _panelTraits.transform.parent;
            int index = _panelTraits.transform.GetSiblingIndex() + 1;
            foreach (AbstractPlayerState trait in player.mentalStates)
            {
                Action action = () => UIManager.Instance.ShowPlayerStateInfo(trait, null);
                AddItem(parentTransform, index, local.GetPlayerState(trait), action);
                ++index;
            }

            // Skills
            parentTransform = _panelSkills.transform.parent;
            index = _panelSkills.transform.GetSiblingIndex() + 1;
            foreach (SkillInfo skillInfo in player.skills)
            {
                Action action = () => UIManager.Instance.ShowSkillInfo(skillInfo, null);
                AddItem(parentTransform, index, local.GetSkill(skillInfo), action);
                ++index;
            }
        }

        private void OnEnable()
        {
            Refresh();
        }
    }
}
