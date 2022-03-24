using Actions;
using PlayerInfo;
using ScriptableObjects;
using System.Collections.Generic;
using UI.Panels.Assets;
using UI.Panels.Templates;
using UnityEngine;

namespace UI.Panels.Actions
{
    public class SelfImprovementPanel : MonoBehaviour
    {
        public Player player;
        private TutorialAction _tutorialAction => TutorialManager.Instance.SelfImprovementOnce;

        private void handler(bool actionDone)
        {
            if (actionDone)
            {
                UIManager.Instance.DestroyAllModal();
            }
        }

        public void OnSelfReflectionButton()
        {
            SelfReflectionAction.Run(player, GameManager.Instance.Random, handler);
        }

        private void trainingHandler(ButtonType buttonType)
        {
            if (buttonType == ButtonType.OK)
            {
                UIManager.Instance.UpdatePlayerInfo(player);
                UIManager.Instance.DestroyAllModal();
                GameManager.Instance.StateMachine.OnPlayerActionDone();
            }
        }

        private string getTrainingActionsLabel()
        {
            List<string> messages = new List<string>()
            {
                "You can choose to take one of the following available professional " +
                "training courses.",
                "",
                string.Format(
                    "Available Cash: {0}",
                    Localization.Instance.GetCurrency(GameManager.Instance.player.cash))
            };
            return string.Join("\n", messages);                
        }

        public void OnTrainingButton()
        {
            if (SkillManager.Instance.currentAvailableSkills.Count == 0)
            {
                UIManager.Instance.ShowSimpleMessageBox(
                    "You didn't find any professional training courses available.",
                    ButtonChoiceType.OK_ONLY,
                    null);
                return;
            }

            int cost = SkillManager.Instance.GetCost(GameManager.Instance.player);
            Localization local = Localization.Instance;
            List<AvailableActionContext> actionContextList = new List<AvailableActionContext>();
            foreach (SkillInfo info in SkillManager.Instance.currentAvailableSkills)
            {
                string label = string.Format(
                    "{0}\nCost: {1}",
                    local.GetSkill(info),
                    local.GetCurrency(cost, true));
                actionContextList.Add(
                    new AvailableActionContext(
                        label,
                        cb => TrainingAction.Run(GameManager.Instance.player, info, cb)));
            }

            UIManager.Instance.ShowAvailableActionsPanel(
                actionContextList, getTrainingActionsLabel, trainingHandler, 1);
        }

        public void OnHelpButton()
        {
            _tutorialAction.ForceRun(null);
        }

        private void OnEnable()
        {
            _tutorialAction.Run(null);
        }
    }
}
