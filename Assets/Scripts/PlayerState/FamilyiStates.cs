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

        public MarriageState() : base("Married")
        {
        }
    }

    public class DivorcedState : AbstractPlayerState
    {
        public override string description =>
            "You're still sad from your recent divorce.";

        public override int happinessModifier
        {
            get
            {
                if (player.spouse == null && player.divorcedPenalty.Item1 > 0)
                {
                    return player.divorcedPenalty.Item2;
                }
                return 0;
            }
        }

        public DivorcedState() : base("Recently Divorced")
        {
        }
    }

    public class ChildrenState : AbstractPlayerState
    {
        public override string description =>
            "You're happily married to the love of your life.";

        public override int happinessModifier =>
            player.numChild > 0 ? FamilyManager.Instance.childHappiness : 0;
        public ChildrenState() : base("Children")
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

        public FamilyOrientedState() : base("Family Oriented")
        {
        }
    }
}
