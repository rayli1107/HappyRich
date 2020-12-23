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

            UI.UIManager.Instance.ShowPlayerStateInfo(state, null);
            UI.UIManager.Instance.UpdatePlayerInfo(_player);
            GameManager.Instance.StateMachine.OnPlayerActionDone();
            RunCallback(true);
        }
    }
}
