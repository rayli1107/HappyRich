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
    PlayerInfo.Player, System.Random, Actions.ActionCallback, Actions.AbstractBuyInvestmentAction>;

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

    private GetInvestmentFn GetRandomInvestmentFn(System.Random random, GetInvestmentFn fn1, GetInvestmentFn fn2)
    {
        return random.Next(2) == 0 ? fn1 : fn2;
    }

    private List<AbstractBuyInvestmentAction> getAvailableInvestments(
        Player player,
        System.Random random,
        GetInvestmentFn getRealEstateFn,
        GetInvestmentFn getBusinessFn,
        Action<int, bool> callback)
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

        List<AbstractBuyInvestmentAction> actions = new List<AbstractBuyInvestmentAction>();
        for (int i = 0; i < randomCount; ++i)
        {
            int index = actions.Count;
            ActionCallback cb = (bool b) => callback(index, b);
            GetInvestmentFn fn = GetRandomInvestmentFn(random, getRealEstateFn, getBusinessFn);
            actions.Add(fn(player, random, cb));
        }

        foreach (SpecialistInfo info in player.specialists)
        {
            int index = actions.Count;
            ActionCallback cb = (bool b) => callback(index, b);
            if (info.specialistType == SpecialistType.REAL_ESTATE_BROKER)
            {
                for (int i = 0; i < _additionalInvestmentRealEstateBroker; ++i)
                {
                    actions.Add(getRealEstateFn(player, random, cb));
                }
            }
            else if (info.specialistType == SpecialistType.ENTREPRENEUR)
            {
                for (int i = 0; i < _additionalInvestmentEntrepreneaur; ++i)
                {
                    actions.Add(getBusinessFn(player, random, cb));
                }
            }
        }

        return actions;
    }

    public List<AbstractBuyInvestmentAction> GetAvailableSmallInvestments(
        Player player,
        System.Random random,
        Action<int, bool> callback)
    {
        return getAvailableInvestments(
            player,
            random,
            RealEstateManager.Instance.GetSmallInvestmentAction,
            BusinessManager.Instance.GetSmallInvestmentAction,
            callback);
    }

    public List<AbstractBuyInvestmentAction> GetAvailableLargeInvestments(
        Player player,
        System.Random random,
        Action<int, bool> callback)
    {
        return getAvailableInvestments(
            player,
            random,
            RealEstateManager.Instance.GetLargeInvestmentAction,
            BusinessManager.Instance.GetLargeInvestmentAction,
            callback);
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
