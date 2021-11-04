using PlayerInfo;

namespace PlayerState
{
    public class AbstractTimedPenaltyState : AbstractPlayerState
    {
        private int _penalty;
        private int _turn;

        public override int happinessModifier => -1 * _penalty;

        public AbstractTimedPenaltyState(Player player, int turn, int penalty, string message) : base(player, message)
        {
            _turn = turn;
            _penalty = penalty;
        }

        public override void OnPlayerTurnStart()
        {
            --_turn;
            if (_turn < 0)
            {
                player.RemoveMentalState(this);
            }
        }
    }

    public class DivorcedPenaltyState : AbstractTimedPenaltyState
    {
        public override string description =>
            "You're still sad from your recent divorce.";

        public DivorcedPenaltyState(Player player, int turn, int penalty)
            : base(player, turn, penalty, "Recently Divorced")
        {
        }
    }

    public class TragedyPenaltyState : AbstractTimedPenaltyState
    {
        public override string description =>
            "You're still devastated from the recent tragedy.";

        public override int happinessModifier =>
            -1 * SelfImprovementManager.Instance.GetTragedyPenalty(player);

        public TragedyPenaltyState(Player player, int turn)
            : base(player, turn, 0, "Recent tragedy")
        {
        }
    }
}
