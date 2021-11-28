using PlayerInfo;
using ScriptableObjects;
using System;
using System.Collections.Generic;
using UI;
using UI.Panels.Templates;

namespace Actions
{
    public static class TrainingAction
    {
        private static void actionFinished(
            Player player, Action callback)
        {
            UIManager.Instance.UpdatePlayerInfo(player);
            GameManager.Instance.StateMachine.OnPlayerActionDone();
            callback?.Invoke();
        }

        private static void transactionHandler(
            Player player,
            SkillInfo skill,
            int cost,
            Action callback,
            bool success)
        {
            if (success)
            {
                actionFinished(player, callback);
            }
            else
            {
                showApplyConfirmation(player, skill, cost, callback);
            }
        }

        private static void messageBoxHandler(
            Player player,
            SkillInfo skill,
            int cost,
            Action callback,            
            ButtonType button)
        {
            if (button == ButtonType.OK)
            {
                TransactionManager.LearnSkill(
                    player,
                    skill,
                    cost,
                    b => transactionHandler(player, skill, cost, callback, b));
            }
            else
            {
                actionFinished(player, callback);
            }
        }

        private static void showApplyConfirmation(
            Player player,
            SkillInfo skill,
            int cost,
            Action callback)
        {
            Localization local = Localization.Instance;
            List<string> messages = new List<string>()
            {
                string.Format(
                    "You found a professional training class that teaches " +
                    "{0} for {1}.",
                    local.GetSkill(skill),
                    local.GetCurrency(cost)),
                "",
                skill.skillDescription,
                "",
                "Take the course?"
            };
            UIManager.Instance.ShowSimpleMessageBox(
                string.Join("\n", messages),
                ButtonChoiceType.OK_CANCEL,
                b => messageBoxHandler(player, skill, cost, callback, b));
        }

        public static void Run(Player player, System.Random random, Action<bool> callback)
        {
            SkillInfo skill = SkillManager.Instance.GetSkill(player, random);
            if (skill == null)
            {
                UIManager.Instance.ShowSimpleMessageBox(
                    "You didn't find any meaning traning courses.",
                    ButtonChoiceType.OK_ONLY,
                    _ => callback?.Invoke(false));
                return;
            }

            int cost = SkillManager.Instance.GetCost(player);
            showApplyConfirmation(player, skill, cost, () => callback?.Invoke(true));
        }
    }
}
