using Assets;
using ScriptableObjects;

public interface ITransactionHandler
{
    void OnTransactionFinish(bool success);
}

public class BuyRealEstateTransactionCallback : ITransactionHandler
{
    private Player _player;
    private AbstractRealEstate _asset;
    private ITransactionHandler _handler;

    public BuyRealEstateTransactionCallback(
        Player player,
        AbstractRealEstate asset,
        ITransactionHandler handler)
    {
        _player = player;
        _asset = asset;
        _handler = handler;
    }

    public void OnTransactionFinish(bool success)
    {
        if (success)
        {
            _player.portfolio.properties.Add(_asset);
        }
        _handler.OnTransactionFinish(success);
    }
}

public class ApplyJobTransactionCallback : ITransactionHandler
{
    private Player _player;
    private Profession _job;
    private ITransactionHandler _handler;

    public ApplyJobTransactionCallback(
        Player player,
        Profession job,
        ITransactionHandler handler)
    {
        _player = player;
        _job = job;
        _handler = handler;
    }

    public void OnTransactionFinish(bool success)
    {
        if (success)
        {
            _player.AddJob(_job);
        }

        _handler.OnTransactionFinish(success);
    }
}

public static class TransactionManager
{
    private static void TryDebit(Player player, int amount, ITransactionHandler handler)
    {
        if (player.cash >= amount)
        {
            player.portfolio.AddCash(-1 * amount);
            handler.OnTransactionFinish(true);
        }
        else
        {
            new Actions.TakePersonalLoan(player, amount, handler).Start();
        }
    }

    public static void BuyRealEstate(
        Player player, AbstractRealEstate asset, ITransactionHandler handler)
    {
        ITransactionHandler newHandler = new BuyRealEstateTransactionCallback(
            player, asset, handler);
        TryDebit(player, asset.downPayment, newHandler);
    }

    public static void ApplyJob(
        Player player, Profession job, ITransactionHandler handler)
    {
        ITransactionHandler newHandler = new ApplyJobTransactionCallback(
            player, job, handler);
        TryDebit(player, job.jobCost, newHandler);
    }

    public static void BuyStock(
        Player player,
        AbstractStock stock,
        int amount,
        ITransactionHandler handler)
    {
        bool success = false;
        int cost = amount * stock.value;

        if (player.cash >= amount)
        {
            player.portfolio.AddCash(-1 * cost);
            player.portfolio.AddStock(stock, amount);
            success = true;
        }

        if (handler != null)
        {
            handler.OnTransactionFinish(success);
        }
    }

    public static void SellStock(
        Player player,
        AbstractStock stock,
        int amount,
        ITransactionHandler handler)
    {
        bool success = false;
        if (player.portfolio.TryRemoveStock(stock, amount))
        {
            player.portfolio.AddCash(amount * stock.value);
            success = true;
        }

        if (handler != null)
        {
            handler.OnTransactionFinish(success);
        }
    }
}
