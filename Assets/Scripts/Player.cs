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

        foreach (Assets.AbstractLiability liability in player.liabilities)
        {
            netWorth -= liability.amount;
            expenses += liability.getExpense();
        }
    }
}

public class Player
{
    public List<Profession> jobs { get; private set; }
    public List<Assets.AbstractAsset> assets { get; private set; }
    public List<Assets.AbstractLiability> liabilities { get
        {
            List<Assets.AbstractLiability> ret = new List<Assets.AbstractLiability>();
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
    public int personalExpenses { get; private set; }
    public int costPerChild { get; private set; }
    public int numChild { get; private set; }
    public int age { get; private set; }
    public int cash { get; private set; }
    public List<PlayerState.PlayerStateInterface> playerStates { get; private set; }

    private int _defaultHappiness;

    public Assets.StudentLoan studentLoan { get; private set; }
    public Assets.PersonalLoan personalLoan { get; private set; }

    public Player(Profession profession, int defaultHappiness)
    {
        jobs = new List<Profession>();
        jobs.Add(profession);

        assets = new List<Assets.AbstractAsset>();
        if (profession.autoLoan > 0)
        {
            assets.Add(new Assets.Car(profession.autoLoan));
        }

        if (profession.jobCost > 0)
        {
            studentLoan = new Assets.StudentLoan(profession.jobCost);
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

    public void AddJob(Profession job)
    {
        jobs.Add(job);
    }

    public void AddCash(int amount)
    {
        cash += amount;
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
}
