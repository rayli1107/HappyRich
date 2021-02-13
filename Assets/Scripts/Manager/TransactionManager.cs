using Assets;
using PlayerInfo;
using ScriptableObjects;

using DistressedProperty = System.Tuple<
    Assets.PartialRealEstate, Assets.DistressedRealEstate>;
using RentalProperty = System.Tuple<
    Assets.PartialRealEstate, Assets.RentalRealEstate>;

public delegate void TransactionHandler(bool success);

public static class TransactionManager
{
    private static void TryDebit(Player player, int amount, TransactionHandler handler)
    {
        if (player.cash >= amount)
        {
            player.portfolio.AddCash(-1 * amount);
            handler?.Invoke(true);
        }
        else
        {
            int loanAmount = amount - player.cash;
            int maxLoanAmount = new Snapshot(player).availablePersonalLoanAmount;
            if (loanAmount <= maxLoanAmount)
            {
                new Actions.TakePersonalLoan(player, amount, handler).Start();
            }
            else
            {
                Localization local = Localization.Instance;
                string message = string.Format(
                    "You'd need to take out a personal loan of {0} but the maximum amount " +
                    "you can borrow is {1}",
                    local.GetCurrency(loanAmount),
                    local.GetCurrency(maxLoanAmount));
                UI.UIManager.Instance.ShowSimpleMessageBox(
                    message,
                    UI.Panels.Templates.ButtonChoiceType.OK_ONLY,
                    (_) => handler?.Invoke(false));
            }
        }
    }

    private static void buyRentalTransactionHandler(
        Player player,
        PartialRealEstate partialAsset,
        RentalRealEstate rentalAsset,
        TransactionHandler handler,
        bool success)
    {
        if (success &&
            player != null &&
            partialAsset != null &&
            rentalAsset != null)
        {
            player.portfolio.rentalProperties.Add(
                new RentalProperty(partialAsset, rentalAsset));
        }
        handler?.Invoke(success);
    }

    public static void BuyRentalRealEstate(
        Player player,
        PartialRealEstate partialasset,
        RentalRealEstate rentalAsset,
        TransactionHandler handler)
    {
        TryDebit(
            player,
            partialasset.fundsNeeded,
            (bool b) => buyRentalTransactionHandler(
                player, partialasset, rentalAsset, handler, b));
    }

    private static void buyDistressedTransactionHandler(
        Player player,
        PartialRealEstate partialAsset,
        DistressedRealEstate distressedAsset,
        TransactionHandler handler,
        bool success)
    {
        if (success &&
            player != null &&
            partialAsset != null &&
            distressedAsset != null)
        {
            player.portfolio.distressedProperties.Add(
                new DistressedProperty(partialAsset, distressedAsset));
        }
        handler?.Invoke(success);
    }

    public static void BuyDistressedRealEstate(
        Player player,
        PartialRealEstate partialAsset,
        DistressedRealEstate distressedAsset,
        TransactionHandler handler)
    {
        TryDebit(
            player,
            partialAsset.fundsNeeded,
            (bool b) => buyDistressedTransactionHandler(
                player, partialAsset, distressedAsset, handler, b));
    }

    private static void applyJobTransactionHandler(
        Player player, Profession job, TransactionHandler handler, bool success)
    {
        if (success)
        {
            player.AddJob(job);
        }
        handler?.Invoke(success);
    }

    public static void ApplyJob(
        Player player, Profession job, TransactionHandler handler)
    {
        TryDebit(
            player,
            job.jobCost,
            (bool b) => applyJobTransactionHandler(player, job, handler, b));
    }

    public static void BuyStock(
        Player player,
        AbstractStock stock,
        int amount,
        TransactionHandler handler)
    {
        bool success = false;
        int cost = amount * stock.value;

        if (player.cash >= amount)
        {
            player.portfolio.AddCash(-1 * cost);
            player.portfolio.AddStock(stock, amount);
            success = true;
        }

        handler?.Invoke(success);
    }

    public static void SellStock(
        Player player,
        AbstractStock stock,
        int amount,
        TransactionHandler handler)
    {
        bool success = false;
        if (player.portfolio.TryRemoveStock(stock, amount))
        {
            player.portfolio.AddCash(amount * stock.value);
            success = true;
        }

        handler?.Invoke(success);
    }

    private static void learnSkillTransactionHandler(
        Player player, SkillInfo skill, TransactionHandler handler, bool success)
    {
        if (success)
        {
            player.AddSkill(skill);
        }
        handler?.Invoke(success);
    }

    public static void LearnSkill(
        Player player,
        SkillInfo skill,
        int cost,
        TransactionHandler handler)
    {
        TryDebit(
            player,
            cost,
            (bool b) => learnSkillTransactionHandler(player, skill, handler, b));
    }
}
