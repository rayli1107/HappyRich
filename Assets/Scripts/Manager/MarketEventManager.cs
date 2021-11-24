using Actions;
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
#pragma warning restore 0649

    public static MarketEventManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
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

        List<Action<Action>> allEvents = new List<Action<Action>>();
        allEvents.Add(InvestmentManager.Instance.GetMarketEvent(player, random));
        allEvents.Add(StockManager.Instance.GetMarketEvent(random));
        allEvents.Add(RiskyInvestmentManager.Instance.GetMarketEvent(player, random));
        allEvents.Add(InvestmentPartnerManager.Instance.GetMarketEvent(player, random));
        allEvents.Add(RealEstateManager.Instance.GetMarketEvent(player, random));
        Action <Action> marketEvent = getRandomEvent(allEvents, random);
        return marketEvent == null ? cb => noOpEvent(cb) : marketEvent;
    }
}
