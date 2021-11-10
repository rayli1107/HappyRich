using PlayerInfo;

namespace PlayerState
{
    public class Frugality : SelfReflectionState
    {
        public override string description => string.Join(
            "\n",
            "You learned how to cut down on wasteful spending.",
            "",
            "Personal Expenses -20%");

        public override int expenseModifier => -20;

        public Frugality(Player player) : base(player, "Frugality")
        {
        }
    }

    public class Minimalism : SelfReflectionState
    {
        public override string description => string.Join(
            "\n",
            "You learned that happiness does not depend on material wealth.",
            "",
            "Personal Expenses -10%",
            "Happiness +10");

        public override int expenseModifier => -10;
        public override int happinessModifier => 10;

        public Minimalism(Player player) : base(player, "Minimalism")
        {
        }
    }

    public class Hustling : SelfReflectionState
    {
        public override string description => string.Join(
            "\n",
            "You realized that in order to succeed, you'll need to work harder " +
            "to find more opportunities.",
            "",
            "When you look for investments you are able to find one more than usual.");

        public Hustling(Player player) : base(player, "Hustling")
        {
        }
    }

    public class Tranquil : SelfReflectionState
    {
        public override string description => string.Join(
            "\n",
            "You realized that tragedy and loss is just a part of life. You will no " +
            "longer be devastated when a tragedy occurs.",
            "",
            "Happiness -5 instead of -20 during a tragedy state.");

        public Tranquil(Player player) : base(player, "Tranquil")
        {
        }
    }

    public class Extrovert : SelfReflectionState
    {
        public override string description => string.Join(
            "\n",
            "You realized that surrounding yourself with better people is part of " +
            "what makes you successful.",
            "",
            string.Format("Happiness +{0} when you have {1} or more contacts.",
                SelfImprovementManager.Instance.extrovertHappinessModifier,
                SelfImprovementManager.Instance.extrovertThreshold));

        public override int happinessModifier
        {
            get
            {
                if (player.contacts.Count + player.specialists.Count >=
                    SelfImprovementManager.Instance.extrovertThreshold)
                {
                    return SelfImprovementManager.Instance.extrovertHappinessModifier;
                }
                return 0;
            }
        }
            
        public Extrovert(Player player) : base(player, "Extrovert")
        {
        }
    }

    public class Extravagant : SelfReflectionState
    {
        public override string description => string.Join(
            "\n",
            "You realized that you enjoy an extravagant lifestyle.",
            "",
            string.Format(
                "Additional Happiness +{0} when you buy luxury items.",
                LuxuryManager.Instance.happinessDelta));
        public Extravagant(Player player) : base(player, "Extravagant")
        {

        }
    }

}
