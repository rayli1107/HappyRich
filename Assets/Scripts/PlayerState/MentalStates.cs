using PlayerInfo;

namespace PlayerState
{
    public class Frugality : AbstractPlayerState
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

    public class Minimalism : AbstractPlayerState
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

    public class Hustling : AbstractPlayerState
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

    public class Tranquil : AbstractPlayerState
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

}
