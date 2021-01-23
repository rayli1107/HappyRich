using Assets;
using PlayerState;
using ScriptableObjects;
using System.Collections.Generic;
using UnityEngine;
public class PlayerSnapshot
{
    public Player player { get; private set; }
    public int age => player.age;
    public int activeIncome { get; private set; }
    public int passiveIncome { get; private set; }
    public int expenses { get; private set; }
    public int cash => player.cash;
    public int happiness => player.happiness;
    public int netWorth { get; private set; }

    public int cashflow => activeIncome + passiveIncome - expenses;
    public int availablePersonalLoanAmount
    {
        get
        {
            int amount = cashflow / InterestRateManager.Instance.personalLoanRate * 100;
            if (player.portfolio.personalLoan != null)
            {
                amount -= player.portfolio.personalLoan.amount;
            }
            return Mathf.Max(amount, 0);
        }
    }

    public PlayerSnapshot(Player player)
    {
        this.player = player;

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
            netWorth -= asset.combinedLiability.amount;
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
    public List<PartialRealEstate> properties { get; private set; }

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
        properties = new List<PartialRealEstate>();
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

    private int _personalExpenses;
    private int _costPerChild;

    public int expenseModifier
    {
        get
        {
            int modifier = 100;
            foreach (AbstractPlayerState state in states)
            {
                modifier += state.expenseModifier;
            }
            return modifier;
        }
    }
    public int personalExpenses => (_personalExpenses * expenseModifier) / 100;
    public int costPerChild => (_costPerChild * expenseModifier) / 100;
    public int numChild { get; private set; }
    public int age { get; private set; }

    public int happiness
    {
        get
        {
            int happiness = defaultHappiness;
            foreach (AbstractPlayerState state in states)
            {
                happiness += state.happinessModifier;
            }
            return happiness;
        }
    }

    public List<AbstractPlayerState> passiveStates { get; private set; }
    public List<AbstractPlayerState> mentalStates { get; private set; }
    public List<AbstractPlayerState> states
    {
        get
        {
            List<AbstractPlayerState> states = new List<AbstractPlayerState>();
            states.AddRange(passiveStates);
            states.AddRange(mentalStates);
            return states;
        }
    }

    public int defaultHappiness { get; private set; }
    public List<InvestmentPartner> contacts { get; private set; }
    public List<SkillInfo> skills { get; private set; }

    public Player(Profession profession, int defaultHappiness)
    {
        oldJobs = new List<Profession>();
        jobs = new List<Profession>();
        jobs.Add(profession);
        portfolio = new Portfolio(profession);
        skills = new List<SkillInfo>();

        _personalExpenses = profession.personalExpenses;
        _costPerChild = profession.costPerChild;
        numChild = 0;
        age = profession.startingAge;
        this.defaultHappiness = defaultHappiness;

        contacts = new List<InvestmentPartner>();

        passiveStates = new List<AbstractPlayerState>()
        {
            new OneJobState(),
            new TwoJobState()
        };

        mentalStates = new List<AbstractPlayerState>();
    }

    public void OnPlayerTurnStart()
    {
        DistributeCashflow();
        UpdateContacts();
    }

    private void UpdateContacts()
    {
        List<InvestmentPartner> newContacts = new List<InvestmentPartner>();
        foreach (InvestmentPartner contact in contacts)
        {
            contact.OnTurnStart();
            if (contact.duration > 0)
            {
                newContacts.Add(contact);
            }
        }
        contacts = newContacts;
    }

    private void DistributeCashflow()
    {
        PlayerSnapshot snapshot = new PlayerSnapshot(this);
        portfolio.AddCash(snapshot.cashflow);
    }

    public void AddJob(Profession job)
    {
        jobs.Add(job);
        oldJobs.Remove(job);
    }

    public void LoseJob(Profession job)
    {
        if (jobs.Remove(job) && !oldJobs.Contains(job))
        {
            oldJobs.Add(job);
        }
    }

    public void AddMentalState(AbstractPlayerState state)
    {
        mentalStates.Add(state);
    }

    public bool HasSkill(SkillType skillType)
    {
        foreach (SkillInfo skillInfo in skills)
        {
            if (skillInfo.skillType == skillType)
            {
                return true;
            }
        }
        return false;
    }

    public void AddSkill(SkillInfo skillInfo)
    {
        if (!HasSkill(skillInfo.skillType))
        {
            skills.Add(skillInfo);
        }
    }

    public List<InvestmentPartner> GetDebtPartners()
    {
        return GetPartners(true, false, false);
    }

    public List<InvestmentPartner> GetEquityPartners()
    {
        return GetPartners(false, false, true);
    }

    public List<InvestmentPartner> GetPartners(
        bool showLowRiskPartners=true,
        bool showMediumRiskPartners=true,
        bool showHighRiskPartners=true)
    {
        List<InvestmentPartner> ret = new List<InvestmentPartner>();

        foreach (InvestmentPartner partner in contacts)
        {
            bool show = false;
            switch (partner.riskTolerance)
            {
                case RiskTolerance.kHigh:
                    show = showHighRiskPartners;
                    break;
                case RiskTolerance.kMedium:
                    show = showMediumRiskPartners;
                    break;
                case RiskTolerance.kLow:
                    show = showLowRiskPartners;
                    break;
            }
            if (show)
            {
                ret.Add(partner);
            }
        }
        return ret;
    }
}
