using PlayerInfo;
using System.Collections.Generic;
using UI.Panels.Templates;

namespace Actions
{
    public class MarriageAction : AbstractAction
    {
        private Player _player;
        private Spouse _spouse;

        public MarriageAction(Player player, Spouse spouse, ActionCallback actionCallback)
            : base(actionCallback)
        {
            _player = player;
            _spouse = spouse;
        }

        private void showSpouseDetails()
        {
            Localization local = Localization.Instance;
            List<string> text = new List<string>()
            {
                string.Format("Spouse's Income: {0}", local.GetCurrency(_spouse.additionalIncome)),
                string.Format("Spouse's Expense: {0}", local.GetCurrency(_spouse.additionalExpense, true)),
                string.Format("Additional Happiness: {0}", local.GetValueAsChange(_spouse.additionalHappiness))
            };
            SimpleTextMessageBox messageBox = UI.UIManager.Instance.ShowSimpleMessageBox(
                string.Join("\n", text),
                ButtonChoiceType.OK_ONLY,
                null);
            messageBox.text.enableWordWrapping = false;
        }

        public override void Start()
        {
            _player.spouse = _spouse;
            UI.UIManager.Instance.ShowSimpleMessageBox(
                "You met the love of your life at a party and decided to get married. Congratulations!",
                ButtonChoiceType.OK_ONLY,
                (_) => RunCallback(true),
                showSpouseDetails);
        }
    }

    public class DivorceAction : AbstractAction
    {
        private Player _player;

        public DivorceAction(Player player, ActionCallback actionCallback)
            : base(actionCallback)
        {
            _player = player;
        }

        public override void Start()
        {
            _player.divorcedPenalty = new System.Tuple<int, int>(
                FamilyManager.Instance.divorcePenaltyDuration,
                -1 * _player.spouse.additionalHappiness);
            _player.spouse = null;

            UI.UIManager.Instance.ShowSimpleMessageBox(
                "Unfortunately you and your spouse decided to split due to irreconcilable differences.",
                ButtonChoiceType.OK_ONLY,
                (_) => RunCallback(true));
        }
    }
}
