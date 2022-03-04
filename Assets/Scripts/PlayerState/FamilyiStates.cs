using PlayerInfo;

namespace PlayerState
{
    public class MarriageState : AbstractPlayerState
    {
        public override string description =>
            "You're happily married to the love of your life.";

        public override int happinessModifier =>
            player.spouse != null ? player.spouse.additionalHappiness : 0;

        public MarriageState(Player player) : base(player, "Married")
        {
        }
    }

    public class ChildrenState : AbstractPlayerState
    {
        public override string description =>
            "You're happily married to the love of your life.";

        public override int happinessModifier =>
            player.numChild > 0 ? FamilyManager.Instance.childHappiness : 0;
        public ChildrenState(Player player) : base(player, "Children")
        {
        }
    }
}
