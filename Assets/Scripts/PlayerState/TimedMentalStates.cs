using PlayerInfo;
using UnityEngine;

namespace PlayerState
{
    public class TimedPlayerState : AbstractPlayerState
    {
        private int _turn;

        public TimedPlayerState(Player player, int turn, string message) : base(player, message)
        {
            _turn = turn;
        }

        public override void OnPlayerTurnStart()
        {
            --_turn;
            Debug.LogFormat("OnPlayerTurnStart {0}", _turn);
            if (_turn < 0)
            {
                player.RemoveMentalState(this);
            }
        }
    }

    public class DivorcedPenaltyState : TimedPlayerState
    {
        private int _penalty;
        public override string description =>
            "You're still sad from your recent divorce.";
        public override int happinessModifier => -1 * _penalty;
        public DivorcedPenaltyState(Player player, int turn, int penalty)
            : base(player, turn, "Recently Divorced")
        {
            _penalty = penalty;
        }
    }

    public class TragedyPenaltyState : TimedPlayerState
    {
        public override string description =>
            "You're still devastated from the recent tragedy.";

        public override int happinessModifier =>
            -1 * SelfImprovementManager.Instance.GetTragedyPenalty(player);

        public TragedyPenaltyState(Player player, int turn) : base(player, turn, "Recent tragedy")
        {
        }
    }

    public class LuxuryHappinessState : TimedPlayerState
    {
        public override string description =>
            "Buying luxurious items brings you (temporary) happiness.";
        public override int happinessModifier => LuxuryManager.Instance.GetLuxuryHappinessModifier(player);
        public LuxuryHappinessState(Player player, int turn) : base(player, turn, "Luxury Item")
        {

        }
    }

    public class FamilyVacationHappinessState : TimedPlayerState
    {
        public override string description => "Spending time with your family makes you happy.";
        public override int happinessModifier => FamilyManager.Instance.familyVacationHappinessModifier.x;
        public FamilyVacationHappinessState(Player player) : base(
            player, FamilyManager.Instance.familyVacationHappinessModifier.y, "Family Vacation")
        {

        }
    }
}
