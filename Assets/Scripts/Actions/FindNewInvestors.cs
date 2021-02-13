using PlayerInfo;
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
                string description = null;
                switch (partner.riskTolerance)
                {
                    case RiskTolerance.kHigh:
                        description = "prefers investing as equity partners.";
                        break;
                    case RiskTolerance.kMedium:
                        description = "is open to both equity and debt partnership.";
                        break;
                    case RiskTolerance.kLow:
                    default:
                        description = "prefers investing using debt.";
                        break;
                }

                string message = string.Format(
                    "You met {0}, a follow investor, who has {1} of available cash, and {2}",
                    Localization.Instance.GetName(partner.name),
                    Localization.Instance.GetCurrency(partner.cash),
                    description);
                UI.UIManager.Instance.ShowSimpleMessageBox(
                    message, ButtonChoiceType.OK_ONLY, null);
            }
        }
    }
}
