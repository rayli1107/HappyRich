using PlayerInfo;
using PlayerState;
using System;

namespace Actions
{
    public class SelfReflectionAction : AbstractAction
    {
        private Player _player;

        public SelfReflectionAction(Player player) : base(null)
        {
            _player = player;
        }

        private void addPlayerState(AbstractPlayerState state)
        {
            if (state != null)
            {
                _player.AddMentalState(state);
                UI.UIManager.Instance.ShowPlayerStateInfo(state, null);
                UI.UIManager.Instance.UpdatePlayerInfo(_player);
            }
            GameManager.Instance.StateMachine.OnPlayerActionDone();
            RunCallback(true);
        }

        public override void Start()
        {
            Action<Player, Action<AbstractPlayerState>> addAction = SelfImprovementManager.Instance.GetSelfReflectionState(
                GameManager.Instance.Random);
            if (addAction != null)
            {
                addAction?.Invoke(_player, addPlayerState);
            }
            else
            {
                addPlayerState(null);
            }
        }
    }
}
