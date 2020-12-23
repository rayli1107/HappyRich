namespace PlayerState
{
    public class OneJobState : AbstractPlayerState
    {
        public override string description => string.Join(
            "\n",
            "Working naturally makes you stressed a bit.",
            "",
            "Happiness -10");

        public OneJobState() : base("Stressed")
        {
        }

        public override int happinessModifier => player.jobs.Count > 0 ? -10 : 0;
    }

    public class TwoJobState : AbstractPlayerState
    {
        public override string description => string.Join(
            "\n",
            "Working two jobs really stresses you out.",
            "",
            "Happiness -20");

        public TwoJobState() : base("Overstressed")
        {
        }

        public override int happinessModifier => player.jobs.Count > 1 ? -20 : 0;
    }
}