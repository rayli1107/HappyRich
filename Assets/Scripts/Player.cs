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

    public PlayerSnapshot(Player player)
    {
        age = player.age;
        cash = player.cash;
        happiness = player.getHappiness();
        netWorth = player.cash;

        activeIncome = 0;
        foreach (Income job in player.jobs)
        {
            activeIncome += job.income;
        }

        passiveIncome = 0;
        expenses = player.personalExpenses;
        expenses += player.numChild * player.costPerChild;

        foreach (Assets.AbstractAsset asset in player.assets)
        {
            int income = asset.getIncome();
            if (income > 0)
            {
                passiveIncome += income;
            }
            else
            {
                expenses -= income;
            }

            netWorth += asset.value;
            if (asset.liability != null)
            {
                netWorth -= asset.liability.amount;
            }
        }
    }
}

public class Player
{
    public List<Income> jobs { get; private set; }
    public List<Assets.AbstractAsset> assets { get; private set; }
    public int personalExpenses { get; private set; }
    public int costPerChild { get; private set; }
    public int numChild { get; private set; }
    public int age { get; private set; }
    public int cash { get; private set; }
    public List<PlayerState.PlayerStateInterface> playerStates { get; private set; }

    private int _defaultHappiness;

    public Player(ScriptableObjects.Profession profession, int defaultHappiness)
    {
        jobs = new List<Income>();
        jobs.Add(new Income(profession.name, profession.salary));

        assets = new List<Assets.AbstractAsset>();
        if (profession.autoLoan > 0)
        {
            assets.Add(new Assets.Car(profession.autoLoan));
        }

        if (profession.studentLoan > 0)
        {
            assets.Add(new Assets.Education(profession.studentLoan));
        }

        personalExpenses = profession.personalExpenses;
        costPerChild = profession.costPerChild;
        numChild = 0;
        age = profession.startingAge;
        cash = profession.startingCash;
        _defaultHappiness = defaultHappiness;

        playerStates = new List<PlayerState.PlayerStateInterface>();
        playerStates.Add(new PlayerState.OneJobState());
        playerStates.Add(new PlayerState.TwoJobState());
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

    public int getCashflow()
    {
        int cashflow = 0;
        foreach (Income job in jobs)
        {
            cashflow += job.income;
        }
        cashflow -= personalExpenses;
        cashflow -= numChild * costPerChild;
        foreach (Assets.AbstractAsset asset in assets)
        {
            cashflow += asset.getIncome();
        }
        return cashflow;
    }

    /*
    public List<Income> getIncomeList()
    {
        List<Income> result = new List<Income>();
        foreach (Income job in _jobs)
        {
            result.Add(job);
        }
        if (_personalExpenses > 0)
        {
            result.Add(new Income("Personal Expenses", -1 * _personalExpenses));
        }
        if (_numChild > 0)
        {
            result.Add(new Income("Children", -1 * _numChild * _costPerChild));
        }
        foreach (Assets.AbstractAsset asset in _assets)
        {
            int income = asset.getIncome();
            if (income != 0)
            {
                result.Add(new Income(asset.name, income));
            }
        }
        return result;
    }
    */
}
