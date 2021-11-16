using Actions;
using Events.Personal;
using PlayerInfo;
using PlayerState;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PersonalEventManager : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField]
    private Vector3Int _lotteryWinning = new Vector3Int(1, 5, 10000);
    [SerializeField]
    private Vector3Int _carAccidentLoss = new Vector3Int(1, 5, 10);
    [SerializeField]
    private int _healthInsuranceCost = 5000;
#pragma warning restore 0649

    public static PersonalEventManager Instance { get; private set; }
    public int healthInsuranceCost => _healthInsuranceCost;

    private void Awake()
    {
        Instance = this;
    }

    public int GetLotteryWinning(System.Random random)
    {
        return random.Next(_lotteryWinning.x, _lotteryWinning.y + 1) * _lotteryWinning.z;
    }

    public float GetCarAccidentLoss(System.Random random)
    {
        return random.Next(_carAccidentLoss.x, _carAccidentLoss.y + 1) / (float)_carAccidentLoss.z;
    }

    private Action<Action> getRandomEvent(List<Action<Action>> events, System.Random random)
    {
        events = events.FindAll(e => e != null);
        return events.Count == 0 ? null : CompositeActions.GetRandomAction(events, random);
    }

    private Action<Action> getGoodEvent(Player player, System.Random random)
    {
        List<Action<Action>> events = new List<Action<Action>>();
        events.Add(JobBonusEvent.GetEvent(player, random));
        events.Add(FamilyVacationEvent.GetEvent(player));
        events.Add(LotteryWinningEvent.GetEvent(player, random));
        return getRandomEvent(events, random);
    }

    private Action<Action> getNeutralEvent(Player player, System.Random random)
    {
        List<Action<Action>> events = new List<Action<Action>>();
        events.AddRange(LuxuryManager.Instance.GetEvents(player, random));
        return getRandomEvent(events, random);
    }


    private Action<Action> getBadEvent(Player player, System.Random random)
    {
        List<Action<Action>> events = new List<Action<Action>>();
//        events.Add(JobLossEvent.GetEvent(player, random));
//        events.Add(TragedyEvents.GetEvent(player, random));
        events.Add(CarAccidentEvent.GetEvent(player, random));
        return getRandomEvent(events, random);
    }

    private Action<Action> getFamilyEvent(Player player, System.Random random)
    {
        List<Action<Action>> events = FamilyManager.Instance.GetEvents(player);
        return getRandomEvent(events, random);
    }


    public Action<Action> GetPersonalEvent()
    {
        Player player = GameManager.Instance.player;
        System.Random random = GameManager.Instance.Random;

        List<Action<Action>> allEvents = new List<Action<Action>>();
        allEvents.Add(getBadEvent(player, random));
//        allEvents.Add(getGoodEvent(player, random));
//        allEvents.Add(getNeutralEvent(player, random));
//        allEvents.Add(getFamilyEvent(player, random));

        allEvents = allEvents.FindAll(e => e != null);
        return CompositeActions.GetRandomAction(allEvents, random);
    }
}
