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

        public Frugality() : base("Frugality")
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

        public Minimalism() : base("Minimalism")
        {
        }
    }
}
