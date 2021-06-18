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
}
