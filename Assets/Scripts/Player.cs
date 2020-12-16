using Assets;
using ScriptableObjects;
using System.Collections.Generic;

public class Income
{
    public string name { get; private set; }
    public int income { get; private set; }

    public Income(string name, int income)
    {
        this.name = name;
        this.income = income;
    }
}

public class PlayerSnapshot
{
    public int age { get; private set; }
    public int activeIncome { get; private set; }
    public int passiveIncome { get; private set; }
    public int expenses { get; private set; }
    public int cash { get; private set; }
    public int happiness {get; private set; }
    public int netWorth { get; private set; }

    public int cashflow { get { return activeIncome + passiveIncome - expenses; } }

    public PlayerSnapshot(Player player)
    {
        age = player.age;
        cash = player.cash;
        happiness = player.getHappiness();
        netWorth = player.cash;

        activeIncome = 0;
        foreach (Profession job in player.jobs)
        {
            activeIncome += job.salary;
        }

        passiveIncome = 0;
        expenses = player.personalExpenses;
        expenses += player.numChild * player.costPerChild;

        foreach (AbstractAsset asset in player.portfolio.assets)
        {
            int income = asset.income;
            if (income > 0)
            {
                passiveIncome += income;
            }
            else
            {
                expenses -= income;
            }

            netWorth += asset.value;
            foreach (AbstractLiability liability in asset.liabilities)
            {
                netWorth -= liability.amount;
            }
        }

        foreach (AbstractLiability liability in player.portfolio.liabilities)
        {
            netWorth -= liability.amount;
            expenses += liability.expense;
        }
    }
}

public class Portfolio
{
    public StudentLoan studentLoan { get; private set; }
    public PersonalLoan personalLoan { get; private set; }
    public Car car { get; private set; }
    public int cash { get; private set; }
    public Dictionary<string, PurchasedStock> stocks { get; private set; }
    public List<AbstractRealEstate> properties { get; private set; }

    public List<AbstractAsset> assets
    {
        get
        {
            List<AbstractAsset> assets = new List<AbstractAsset>();
            assets.AddRange(otherAssets);
            foreach (KeyValuePair<string, PurchasedStock> entry in stocks)
            {
                assets.Add(entry.Value);
            }
            assets.AddRange(properties);
            return assets;
        }
    }

    public List<AbstractAsset> otherAssets
    {
        get
        {
            List<AbstractAsset> otherAssets = new List<AbstractAsset>();
            if (car != null)
            {
                otherAssets.Add(car);
            }
            return otherAssets;
        }
    }

    public List<AbstractLiability> liabilities
    {
        get
        {
            List<AbstractLiability> ret = new List<AbstractLiability>();
            if (studentLoan != null && studentLoan.amount > 0)
            {
                ret.Add(studentLoan);
            }
            if (personalLoan != null && personalLoan.amount > 0)
            {
                ret.Add(personalLoan);
            }
            return ret;
        }
    }


    public Portfolio(Profession profession)
    {
        if (profession.autoLoan > 0)
        {
            car = new Car(profession.autoLoan);
        }

        if (profession.jobCost > 0)
        {
            studentLoan = new StudentLoan(profession.jobCost);
        }

        cash = profession.startingCash;
        stocks = new Dictionary<string, PurchasedStock>();
        properties = new List<AbstractRealEstate>();
    }

    public void AddPersonalLoan(int amount)
    {
        if (personalLoan == null)
        {
            personalLoan = new PersonalLoan(amount);
        }
        else
        {
            personalLoan.Add(amount);
        }
    }

    public void AddCash(int amount)
    {
        cash += amount;
    }

    public void AddStock(AbstractStock stock, int number)
    {
        PurchasedStock purchasedStock = null;
        if (stocks.TryGetValue(stock.name, out purchasedStock))
        {
            purchasedStock.AddCount(number);
        } 
        else
        {
            purchasedStock = new PurchasedStock(stock);
            purchasedStock.AddCount(number);
            stocks.Add(stock.name, purchasedStock);
        }
    }

    public bool TryRemoveStock(AbstractStock stock, int number)
    {
        PurchasedStock purchasedStock = null;
        if (stocks.TryGetValue(stock.name, out purchasedStock))
        {
            if (purchasedStock.count == number)
            {
                stocks.Remove(stock.name);
                return true;
            }
            else if (purchasedStock.count > number)
            {
                return purchasedStock.TryRemoveCount(number);
            }
        }
        return false;
    }
}

public class Player
{
    public List<Profession> jobs { get; private set; }
    public List<Profession> oldJobs { get; private set; }
    public Portfolio portfolio { get; private set; }
    public int cash => portfolio.cash;
    public int personalExpenses { get; private set; }
    public int costPerChild { get; private set; }
    public int numChild { get; private set; }
    public int age { get; private set; }
    public List<PlayerState.PlayerStateInterface> playerStates { get; private set; }

    private int _defaultHappiness;


    public Player(Profession profession, int defaultHappiness)
    {
        oldJobs = new List<Profession>();
        jobs = new List<Profession>();
        jobs.Add(profession);
        portfolio = new Portfolio(profession);

        personalExpenses = profession.personalExpenses;
        costPerChild = profession.costPerChild;
        numChild = 0;
        age = profession.startingAge;
        _defaultHappiness = defaultHappiness;

        playerStates = new List<PlayerState.PlayerStateInterface>();
        playerStates.Add(new PlayerState.OneJobState());
        playerStates.Add(new PlayerState.TwoJobState());
    }

    public void DistributeCashflow()
    {
        PlayerSnapshot snapshot = new PlayerSnapshot(this);
        portfolio.AddCash(snapshot.cashflow);
    }

    public void AddJob(Profession job)
    {
        jobs.Add(job);
    }

    public int getHappiness()
    {
        int happiness = _defaultHappiness;
        foreach (PlayerState.PlayerStateInterface state in playerStates)
        {
            happiness += state.getHappiness(this);
        }
        return happiness;
    }

    public void LoseJob(Profession job)
    {
        if (jobs.Remove(job) && !oldJobs.Contains(job))
        {
            oldJobs.Add(job);
        }
    }
}
