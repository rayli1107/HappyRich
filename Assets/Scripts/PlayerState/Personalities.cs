using PlayerInfo;

namespace PlayerState
{
    public class Personality : AbstractPlayerState
    {
        public Personality(Player player, string name) : base(player, name)
        {

        }
    }

    public class Extrovert : Personality
    {
        public override string description => string.Join(
            "\n",
            "You enjoy social interaction and meeting new people.",
            "",
            string.Format(
                "Happiness +{0} when you have {1} or more contacts.",
                MentalStateManager.Instance.extrovertHappinessModifier,
                MentalStateManager.Instance.extrovertThreshold));

        public override int happinessModifier =>
            player.contacts.Count + player.specialists.Count >=
            MentalStateManager.Instance.extrovertThreshold ?
            MentalStateManager.Instance.extrovertHappinessModifier : 0;

        public Extrovert(Player player) : base(player, "Extrovert")
        {
        }
    }

    public class Introvert : Personality
    {
        public override string description => string.Join(
            "\n",
            "You prefer having a few close friends instead of meeting new people.",
            "",
            string.Format(
                "Happiness +{0} when you less than {1} contacts.",
                MentalStateManager.Instance.introvertHappinessModifier,
                MentalStateManager.Instance.extrovertThreshold));

        public override int happinessModifier =>
            player.contacts.Count + player.specialists.Count <
            MentalStateManager.Instance.extrovertThreshold ?
            MentalStateManager.Instance.introvertHappinessModifier : 0;

        public Introvert(Player player) : base(player, "Introvert")
        {
        }
    }

    public class Romantic : Personality
    {
        public override string description => string.Join(
            "\n",
            "You enjoy an intimate romantic relationship above all else.",
            "",
            string.Format(
                "Additional happiness +{0} when married.",
                MentalStateManager.Instance.romanticHappinessModifier));

        public override int happinessModifier => 
            player.spouse == null ?
            0 : MentalStateManager.Instance.romanticHappinessModifier;

        public Romantic(Player player) : base(player, "Romantic")
        {
        }
    }

    public class RiskTaker : Personality
    {
        public override string description => string.Join(
            "\n",
            "You enjoy the thrill of a high risk high reward lifestyle.",
            "",
            string.Format(
                "Happiness +{0} when you have high risk investments.",
                MentalStateManager.Instance.riskTakerHappinessModifier));

        public override int happinessModifier => 
            player.portfolio.hasHighRiskInvestments ?
            MentalStateManager.Instance.riskTakerHappinessModifier : 0;

        public RiskTaker(Player player) : base(player, "Risk Taker")
        {
        }
    }

    public class RiskAverse : Personality
    {
        public override string description => string.Join(
            "\n",
            "You prefer a safer, low reward lifestyle",
            "",
            string.Format(
                "Happiness +{0} when you don't have high risk investments.",
                MentalStateManager.Instance.riskTakerHappinessModifier));

        public override int happinessModifier =>
            player.portfolio.hasHighRiskInvestments ?
            0 : MentalStateManager.Instance.riskAverseHappinessModifier;

        public RiskAverse(Player player) : base(player, "Risk Averse")
        {
        }
    }
}
