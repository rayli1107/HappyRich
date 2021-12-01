using Actions;
using Assets;
using Events.Market;
using PlayerInfo;
using PlayerState;
using ScriptableObjects;
using System;
using System.Collections.Generic;
using UnityEngine;

using GetInvestmentFn = System.Func<
    PlayerInfo.Player,
    System.Random,
    BuyInvestmentContext>;

public struct BuyInvestmentContext
{
    public AbstractInvestment asset { get; private set; }
    public Action<Action<bool>> buyAction { get; private set; }

    public BuyInvestmentContext(
        AbstractInvestment asset, Action<Action<bool>> buyAction)
    {
        this.asset = asset;
        this.buyAction = buyAction;
    }
}

public class InvestmentManager : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField]
    private int _defaultAvailableInvestments = 2;
    [SerializeField]
    private int _hustledAvailableInvestments = 1;
    [SerializeField]
    private int _additionalInvestmentRealEstateBroker = 1;
    [SerializeField]
    private int _additionalInvestmentEntrepreneaur = 1;
    [SerializeField]
    private float _incomeMultiplierModifier = 0.05f;
    [SerializeField]
    private int _investmentHappinessThreshold = 5;
    [SerializeField]
    private int _investmentHappinessModifier = -10;
#pragma warning restore 0649

    public static InvestmentManager Instance;
    public int investmentHappinessThreshold => _investmentHappinessThreshold;
    public int investmentHappinessModifier => _investmentHappinessModifier;

    private void Awake()
    {
        Instance = this;
    }

    private GetInvestmentFn GetRandomInvestmentFn(
        System.Random random, GetInvestmentFn fn1, GetInvestmentFn fn2)
    {
        return random.Next(2) == 0 ? fn1 : fn2;
    }

    private List<BuyInvestmentContext> getAvailableInvestments(
        Player player,
        System.Random random,
        GetInvestmentFn getRealEstateFn,
        GetInvestmentFn getBusinessFn)
    {
        int randomCount = _defaultAvailableInvestments;

        foreach (AbstractPlayerState state in player.states)
        {
            if (state is Hustling)
            {
                randomCount += _hustledAvailableInvestments;
                break;
            }
        }

        List<BuyInvestmentContext> actions = new List<BuyInvestmentContext>();
        for (int i = 0; i < randomCount; ++i)
        {
            GetInvestmentFn fn = GetRandomInvestmentFn(random, getRealEstateFn, getBusinessFn);
            actions.Add(fn(player, random));
        }

        foreach (SpecialistInfo info in player.specialists)
        {
            if (info.specialistType == SpecialistType.REAL_ESTATE_BROKER)
            {
                for (int i = 0; i < _additionalInvestmentRealEstateBroker; ++i)
                {
                    actions.Add(getRealEstateFn(player, random));
                }
            }
            else if (info.specialistType == SpecialistType.ENTREPRENEUR)
            {
                for (int i = 0; i < _additionalInvestmentEntrepreneaur; ++i)
                {
                    actions.Add(getBusinessFn(player, random));
                }
            }
        }

        return actions;
    }

    public List<BuyInvestmentContext> GetAvailableSmallInvestments(
        Player player, System.Random random)
    {
        return getAvailableInvestments(
            player,
            random,
            RealEstateManager.Instance.GetSmallInvestmentAction,
            BusinessManager.Instance.GetSmallInvestmentAction);
    }

    public List<BuyInvestmentContext> GetAvailableLargeInvestments(
        Player player, System.Random random)
    {
        return getAvailableInvestments(
            player,
            random,
            RealEstateManager.Instance.GetLargeInvestmentAction,
            BusinessManager.Instance.GetLargeInvestmentAction);
    }

    public Action<Action> GetMarketEvent(Player player, System.Random random)
    {
        List<Action<Action>> events = new List<Action<Action>>();
        events.Add(RentalBoomEvent.GetEvent(player, _incomeMultiplierModifier));
        events.Add(RentalCrashEvent.GetEvent(player, _incomeMultiplierModifier));
        events.Add(MarketBoomEvent.GetEvent(player, _incomeMultiplierModifier));
        events.Add(MarketCrashEvent.GetEvent(player, _incomeMultiplierModifier));
        return CompositeActions.GetRandomAction(events, random);
    }
}
