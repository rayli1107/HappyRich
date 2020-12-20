using UI.Panels;
using UI.Panels.Templates;

namespace Actions
{
    public class FindNewInvestors : AbstractAction
    {
        private Player _player;

        public FindNewInvestors(Player player) : base(null)
        {
            _player = player;
        }

        public override void Start()
        {
            InvestmentPartner partner = InvestmentPartnerManager.Instance.GetPartner(
                GameManager.Instance.Random);
            if (partner == null)
            {
                UI.UIManager.Instance.ShowSimpleMessageBox(
                    "You made a few friends but none of them were interested in investing.",
                    ButtonChoiceType.OK_ONLY,
                    null);
            }
            else
            {
                _player.contacts.Add(partner);
                UI.UIManager.Instance.UpdatePlayerInfo(_player);
                GameManager.Instance.StateMachine.OnPlayerActionDone();
                RunCallback(true);
                string message = string.Format(
                    "You met {0}, a follow investor, who has {1} of available cash.",
                    Localization.Instance.GetName(partner.name),
                    Localization.Instance.GetCurrency(partner.cash));
                UI.UIManager.Instance.ShowSimpleMessageBox(
                    message, ButtonChoiceType.OK_ONLY, null);
            }
        }
    }
}
