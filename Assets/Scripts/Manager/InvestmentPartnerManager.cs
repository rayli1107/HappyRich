using Actions;
using Assets;
using PlayerInfo;
using ScriptableObjects;
using System;
using System.Collections.Generic;
using UnityEngine;

public enum RiskTolerance
{
    kLow,
    kMedium,
    kHigh
}

public class InvestmentPartner
{
    public string name { get; private set; }
    public int cash;
    public int duration { get; private set; }
    private int _initialDuration;
    public RiskTolerance riskTolerance { get; private set; }

    public InvestmentPartner(
        string name, int cash, RiskTolerance riskTolerance, int duration)
    {
        this.name = name;
        this.cash = cash;
        this.riskTolerance = riskTolerance;
        _initialDuration = duration;
        RefreshDuration();
    }

    public void OnTurnStart()
    {
        --duration;
    }

    public void RefreshDuration()
    {
        duration = _initialDuration;
    }
}

public class InvestmentPartnerManager : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField]
    private InvestmentPartnerProfile[] _profiles;
    [SerializeField]
    private string[] _names;
    [SerializeField]
    private int _defaultDuration = 10;
    [SerializeField]
    private int _increment = 50000;
    [SerializeField]
    private Vector2 _partnerCashRange = new Vector2(0.5f, 1.5f);
    [SerializeField]
    private int _defaultPartnerCount = 2;
    [SerializeField]
    private int _publicSpeakingPartnerCount = 2;
    [SerializeField]
    private int _vcPartnerCount = 1;
#pragma warning restore 0649

    public static InvestmentPartnerManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;        
    }

    private InvestmentPartner GetPartnerFromProfile(
        InvestmentPartnerProfile profile, System.Random random)
    {
        int min = profile.cashRange.x / profile.cashIncrement;
        int max = profile.cashRange.y / profile.cashIncrement;
        int cash = random.Next(min, max + 1) * profile.cashIncrement;
        string name = _names[random.Next(_names.Length)];

        Array riskLevels = Enum.GetValues(typeof(RiskTolerance));
        RiskTolerance riskTolerance =
            random.Next(2) == 0 ? RiskTolerance.kLow : RiskTolerance.kHigh;

        return new InvestmentPartner(name, cash, riskTolerance, _defaultDuration);
    }

    private InvestmentPartner getPartner(int lo, int hi, System.Random random)
    {
        int cash = Mathf.Max(1, random.Next(lo, hi + 1)) * _increment;
        string name = _names[random.Next(_names.Length)];
        RiskTolerance riskTolerance =
            random.Next(2) == 0 ? RiskTolerance.kLow : RiskTolerance.kHigh;
        return new InvestmentPartner(name, cash, riskTolerance, _defaultDuration);
    }

    private List<InvestmentPartner> getPartners(
        Player player, System.Random random, int count = 0)
    {
        int value = 0;
        foreach (AbstractAsset asset in player.portfolio.managedAssets)
        {
            value += asset.value - asset.combinedLiability.amount;
        }
        value = Mathf.Max(value, new Snapshot(player).netWorth);
        value = Mathf.Max(value, _increment);
        int lo = Mathf.FloorToInt(value * _partnerCashRange.x) / _increment;
        int hi = Mathf.FloorToInt(value * _partnerCashRange.y) / _increment;

        if (count <= 0)
        {
            count = _defaultPartnerCount;
            if (player.HasSkill(SkillType.PUBLIC_SPEAKING))
            {
                count += _publicSpeakingPartnerCount;
            }
        }

        List<InvestmentPartner> partners = new List<InvestmentPartner>();
        for (int i = 0; i < count; ++i)
        {
            partners.Add(getPartner(lo, hi, random));
        }
        return partners;
    }



    private Action<Action> getActionFromPartnerList(
        Player player,
        List<InvestmentPartner> partners)
    {
        List<Action<Action>> actions = new List<Action<Action>>();
        actions.Add(
            cb =>
            {
                EventLogManager.Instance.Log("Market Event - New Investors");
                cb?.Invoke();
            });
        foreach (InvestmentPartner partner in partners)
        {
            actions.Add(cb => FindNewInvestors.FindInvestor(player, partner, cb));
        }
        return CompositeActions.GetAndAction(actions);
    }

    public Action<Action> GetMarketEvent(Player player, System.Random random)
    {
        if (player.HasSpecialist(SpecialistType.VENTURE_CAPITALIST))
        {
            List<InvestmentPartner> partners = getPartners(
                player, random, _vcPartnerCount);
            return getActionFromPartnerList(player, partners);
        }
        return null;
    }

    public Action<Action> GetAction(Player player, System.Random random)
    {
        List<InvestmentPartner> partners = getPartners(player, random);
        return getActionFromPartnerList(player, partners);
    }
}
