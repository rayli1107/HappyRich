﻿using Actions;
using Events.Personal;
using PlayerInfo;
using PlayerState;
using System;
using System.Collections.Generic;
using UI.Panels.Templates;
using UnityEngine;

public class MarketEventManager : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField]
    private bool _enableInvestmentEvents = true;
    [SerializeField]
    private bool _enableStockEvents = true;
    [SerializeField]
    private bool _enableInvestmentPartnerEvents = true;
    [SerializeField]
    private bool _enableRealEstateEvents = true;
#pragma warning restore 0649

    public static MarketEventManager Instance { get; private set; }

    private LinkedList<Func<Player, System.Random, Action<Action>>> _tutorialActions;

    private void Awake()
    {
        Instance = this;
        _tutorialActions = new LinkedList<Func<Player, System.Random, Action<Action>>>();
    }

    private Action<Action> getRandomEvent(
        List<Action<Action>> events, System.Random random)
    {
        events = events.FindAll(e => e != null);
        return events.Count == 0 ? null : CompositeActions.GetRandomAction(events, random);
    }

    private void noOpEvent(Action callback)
    {
        UI.UIManager.Instance.ShowSimpleMessageBox(
            "Nothing special happened in the market this year.",
            ButtonChoiceType.OK_ONLY,
            _ => callback?.Invoke());
    }

    public Action<Action> GetMarketEvent()
    {
        Player player = GameManager.Instance.player;        
        System.Random random = GameManager.Instance.Random;

        if (_tutorialActions.Count > 0)
        {
            Func<Player, System.Random, Action<Action>> getEventFn =
                _tutorialActions.First.Value;
            _tutorialActions.RemoveFirst();
            return getEventFn(player, random);
        }

        List<Action<Action>> allEvents = new List<Action<Action>>();
        if (_enableInvestmentEvents)
        {
            allEvents.Add(InvestmentManager.Instance.GetMarketEvent(player, random));
        }
        if (_enableStockEvents)
        {
            allEvents.Add(StockManager.Instance.GetMarketEvent(random));
        }
        if (_enableInvestmentPartnerEvents)
        {
            allEvents.Add(InvestmentPartnerManager.Instance.GetMarketEvent(player, random));
        }
        if (_enableRealEstateEvents)
        {
            allEvents.Add(RealEstateManager.Instance.GetMarketEvent(player, random));
        }
        Action <Action> marketEvent = getRandomEvent(allEvents, random);
        return marketEvent == null ? cb => noOpEvent(cb) : marketEvent;
    }

    public void EnableTutorialActions()
    {
    }
}
