using PlayerInfo;

namespace PlayerState
{
    [System.Serializable]
    public struct TimedPlayerStateData
    {
        public enum StateType
        {
            DIVORCE_PENALTY,
            TRAGEDY_PENALTY,
            LUXURY_HAPPINESS,
            FAMILY_VACATION_HAPPINESS,
            MEDITATED
        }
        public StateType stateType;
        public int turn;

        // used by divorced penalty
        public int penalty;

        public TimedPlayerStateData(StateType stateType, int turn, int penalty=0)
        {
            this.stateType = stateType;
            this.turn = turn;
            this.penalty = penalty;
        }
    }

    public abstract class TimedPlayerState : AbstractPlayerState
    {
        protected abstract TimedPlayerStateData.StateType _stateType { get; }
        private int _turn;

        public TimedPlayerState(
            Player player,
            int turn,
            string message) : base(player, message)
        {
            _turn = turn;
        }

        public override void OnPlayerTurnStart()
        {
            --_turn;
            if (_turn < 0)
            {
                player.RemoveMentalState(this);
            }
        }

        public virtual TimedPlayerStateData GetData()
        {
            return new TimedPlayerStateData(_stateType, _turn);
        }
    }

    public class DivorcedPenaltyState : TimedPlayerState
    {
        protected override TimedPlayerStateData.StateType _stateType =>
            TimedPlayerStateData.StateType.DIVORCE_PENALTY;
        private int _penalty;

        public override string description =>
            "You're still sad from your recent divorce.";
        public override int happinessModifier => -1 * _penalty;
        public DivorcedPenaltyState(Player player, int turn, int penalty)
            : base(player, turn, "Recently Divorced")
        {
            _penalty = penalty;
        }

        public override TimedPlayerStateData GetData()
        {
            TimedPlayerStateData data = base.GetData();
            data.penalty = _penalty;
            return data;
        }

        public static DivorcedPenaltyState CreateStateFromData(
            Player player, TimedPlayerStateData data)
        {
            return new DivorcedPenaltyState(player, data.turn, data.penalty);
        }
    }

    public class TragedyPenaltyState : TimedPlayerState
    {
        protected override TimedPlayerStateData.StateType _stateType =>
            TimedPlayerStateData.StateType.TRAGEDY_PENALTY;
        public override string description =>
            "You're still devastated from the recent tragedy.";

        public override int happinessModifier =>
            -1 * MentalStateManager.Instance.GetTragedyPenalty(player);

        public TragedyPenaltyState(Player player, int turn)
            : base(player, turn, "Recent tragedy")
        {
        }

        public static TragedyPenaltyState CreateStateFromData(
            Player player, TimedPlayerStateData data)
        {
            return new TragedyPenaltyState(player, data.turn);
        }
    }

    public class LuxuryHappinessState : TimedPlayerState
    {
        protected override TimedPlayerStateData.StateType _stateType =>
            TimedPlayerStateData.StateType.LUXURY_HAPPINESS;

        public override string description =>
            "Buying luxurious items brings you (temporary) happiness.";
        public override int happinessModifier => LuxuryManager.Instance.GetLuxuryHappinessModifier(player);


        public LuxuryHappinessState(Player player, int turn)
            : base(player, turn, "Luxury Item")
        {

        }

        public static LuxuryHappinessState CreateStateFromData(
            Player player, TimedPlayerStateData data)
        {
            return new LuxuryHappinessState(player, data.turn);
        }
    }

    public class FamilyVacationHappinessState : TimedPlayerState
    {
        protected override TimedPlayerStateData.StateType _stateType =>
            TimedPlayerStateData.StateType.FAMILY_VACATION_HAPPINESS;

        public override string description => "Spending time with your family makes you happy.";
        public override int happinessModifier => FamilyManager.Instance.familyVacationHappinessModifier.x;


        public FamilyVacationHappinessState(Player player, int turn)
            : base(player, turn, "Family Vacation")
        {

        }

        public static FamilyVacationHappinessState CreateStateFromData(
            Player player, TimedPlayerStateData data)
        {
            return new FamilyVacationHappinessState(player, data.turn);
        }
    }

    public class MeditatedState : TimedPlayerState
    {
        protected override TimedPlayerStateData.StateType _stateType =>
            TimedPlayerStateData.StateType.MEDITATED;

        public override string description =>
            "Meditation brings wisdom; lack of meditation leaves ignorance. " +
            "Know well what leads you forward and what holds you back, and " +
            "choose the path that leads to wisdom.";
        public override int happinessModifier =>
            MentalStateManager.Instance.meditatedHappinessModifier;


        public MeditatedState(Player player, int turn)
            : base(player, turn, "Meditated")
        {
        }

        public static MeditatedState CreateStateFromData(
            Player player, TimedPlayerStateData data)
        {
            return new MeditatedState(player, data.turn);
        }
    }
}
