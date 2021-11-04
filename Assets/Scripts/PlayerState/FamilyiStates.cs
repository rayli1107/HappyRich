using PlayerInfo;

namespace PlayerState
{
    public class MarriageState : AbstractPlayerState
    {
        public override string description =>
            "You're happily married to the love of your life.";

        public override int happinessModifier
        {
            get
            {
                if (player.spouse != null)
                {
                    return player.spouse.additionalHappiness;
                }
                return 0;
            }
        }

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

    public class FamilyOrientedState : AbstractPlayerState
    {
        public override string description => string.Format(
            "You realize you're a family oreinted man, and your family means everything " +
            "to you. Happiness +10 when you're married and have at least {0} children.",
            FamilyManager.Instance.familyOrientedChildThreshold);

        public override int happinessModifier =>
            player.spouse != null &&
            player.numChild >= FamilyManager.Instance.familyOrientedChildThreshold ?
            FamilyManager.Instance.familyOrientedHappinessModifier : 0;

        public FamilyOrientedState(Player player) : base(player, "Family Oriented")
        {
        }
    }
}
