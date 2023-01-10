using PlayerInfo;
using ScriptableObjects;

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

    public class FamilyOrientedState : SelfReflectionState
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

    public class AssetManagementStress : AbstractPlayerState
    {
        public override string description => string.Format(
            "Stress from managing {0} or more investments.",
            InvestmentManager.Instance.investmentHappinessThreshold);

        public override int happinessModifier =>
            !player.HasSkill(SkillType.ASSET_MANAGEMENT) &&
            player.portfolio.managedAssets.Count >=
            InvestmentManager.Instance.investmentHappinessThreshold ?
            InvestmentManager.Instance.investmentHappinessModifier : 0;

        public AssetManagementStress(Player player)
            : base(player, "Asset Management Stress")
        {
        }
    }

    public class Enlightenment : SelfReflectionState
    {
        public override string description =>
            "You've become one with the universe.";

        public override int happinessModifier =>
            MentalStateManager.Instance.enlightenedHappinessModifier;

        public Enlightenment(Player player) : base(player, "Enlightenment")
        {
        }
    }
}
