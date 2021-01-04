using UI.Panels;
using UI.Panels.Templates;

namespace Actions
{
    public class MaintainRelationshipAction : AbstractAction
    {
        private Player _player;

        public MaintainRelationshipAction(Player player) : base(null)
        {
            _player = player;
        }

        private void onMessageDone(ButtonType buttonType)
        {
            UI.UIManager.Instance.UpdatePlayerInfo(_player);
            GameManager.Instance.StateMachine.OnPlayerActionDone();
            RunCallback(true);
        }

        public override void Start()
        {
            foreach (InvestmentPartner partner in _player.contacts)
            {
                partner.RefreshDuration();
            }

            UI.UIManager.Instance.ShowSimpleMessageBox(
                "You've reached out to all your professional contacts.",
                ButtonChoiceType.OK_ONLY,
                onMessageDone);
        }
    }
}
