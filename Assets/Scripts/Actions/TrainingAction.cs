using ScriptableObjects;
using System.Collections.Generic;
using UI;
using UI.Panels.Templates;

namespace Actions
{
    public class TrainingAction : AbstractAction
    {
        private Player _player;
        private SkillInfo _skill;
        private int _cost;

        public TrainingAction(Player player) : base(null)
        {
            _player = player;
        }

        private void messageBoxHandler(ButtonType button)
        {
            if (button == ButtonType.OK)
            {
                TransactionManager.LearnSkill(
                    _player, _skill, _cost, onTransactionFinish);
            }
            else
            {
                GameManager.Instance.StateMachine.OnPlayerActionDone();
                RunCallback(false);
            }
        }

        private void onTransactionFinish(bool success)
        {
            if (success)
            {
                UIManager.Instance.UpdatePlayerInfo(_player);
                GameManager.Instance.StateMachine.OnPlayerActionDone();
                RunCallback(true);
            }
            else
            {
                ShowApplyConfirmation();
            }
        }

        private void ShowApplyConfirmation()
        {
            Localization local = Localization.Instance;
            List<string> messages = new List<string>()
            {
                string.Format(
                    "You found a professional training class that teaches " +
                    "{0} for {1}.",
                    local.GetSkill(_skill),
                    local.GetCurrency(_cost, true)),
                "",
                string.Format(
                    "{0} - {1}",
                    local.GetSkill(_skill),
                    _skill.skillDescription),
                "",
                "Take the course?"
            };
            UIManager.Instance.ShowSimpleMessageBox(
                string.Join("\n", messages),
                ButtonChoiceType.OK_CANCEL,
                messageBoxHandler);
        }

        public override void Start()
        {
            _skill = SkillManager.Instance.GetSkill(
                _player, GameManager.Instance.Random);
            _cost = SkillManager.Instance.GetCost(_player);

            ShowApplyConfirmation();
        }
    }
}
