using Actions;
using Assets;
using PlayerInfo;
using PlayerState;
using ScriptableObjects;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LuxuryManager : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField]
    private LuxuryItemProfile[] _luxuryItems;
    [SerializeField]
    private int _defaultLuxuryHappinessModifier = 5;
    [SerializeField]
    private int _extravagantLuxuryHappinessModifier = 10;
    [SerializeField]
    private int _luxuryHappinessDuration = 2;
#pragma warning restore 0649
    public int happinessDelta =>
        _extravagantLuxuryHappinessModifier - _defaultLuxuryHappinessModifier;
    public int luxuryHappinessDuration => _luxuryHappinessDuration;

    public static LuxuryManager Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }

    public int GetLuxuryHappinessModifier(Player player)
    {
        return player.states.Exists(s => s is Extravagant) ?
            _extravagantLuxuryHappinessModifier : _defaultLuxuryHappinessModifier;
    }

    public List<Action<Action>> GetEvents(Player player, System.Random random)
    {
        List<Action<Action>> events = new List<Action<Action>>();
        if (!player.states.Exists(s => s is LuxuryHappinessState) && _luxuryItems.Length > 0)
        {
            LuxuryItemProfile profile = _luxuryItems[random.Next(_luxuryItems.Length)];
            int priceLow = profile.itemPriceRange.x / profile.itemIncrement;
            int priceHigh = profile.itemPriceRange.y / profile.itemIncrement;
            int price = random.Next(priceLow, priceHigh + 1) * profile.itemIncrement;
            LuxuryItem item = new LuxuryItem(profile, price);
            events.Add(BuyLuxuryItemAction.GetEvent(player, item));
        }
        return events;
    }
}
