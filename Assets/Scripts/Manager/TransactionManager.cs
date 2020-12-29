using Assets;
using ScriptableObjects;

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
            new Actions.TakePersonalLoan(player, amount, handler).Start();
        }
    }

    private static void buyRealEstateTransactionHandler(
        Player player, AbstractRealEstate asset, TransactionHandler handler, bool success)
    {
        if (success && player != null && asset != null)
        {
            player.portfolio.properties.Add(asset);
        }
        handler?.Invoke(success);
    }

    public static void BuyRealEstate(
        Player player, AbstractRealEstate asset, TransactionHandler handler)
    {
        TryDebit(
            player,
            asset.downPayment,
            (bool b) => buyRealEstateTransactionHandler(player, asset, handler, b));
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
}
