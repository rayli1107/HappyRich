using PlayerState;
using UI.Panels.Templates;

namespace Actions
{
    public class SelfReflectionAction : AbstractAction
    {
        private Player _player;

        public SelfReflectionAction(Player player) : base(null)
        {
            _player = player;
        }

        public override void Start()
        {
            AbstractPlayerState state = SelfImprovementManager.Instance.GetSelfReflectionState(
                GameManager.Instance.Random);

            _player.AddMentalState(state);

            Localization local = Localization.Instance;
            string message = string.Join("\n", local.GetPlayerState(state), state.description);
            UI.UIManager.Instance.ShowSimpleMessageBox(message, ButtonChoiceType.OK_ONLY, null);
            UI.UIManager.Instance.UpdatePlayerInfo(_player);
            GameManager.Instance.StateMachine.OnPlayerActionDone();
            RunCallback(true);
        }
    }
}
