using PlayerInfo;
using PlayerState;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MentalStateManager : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField]
    private int _defaultTragedyPenalty = 20;
    [SerializeField]
    private int _tranquilTragedyPenalty = 5;
    [SerializeField]
    private int _tragedyDuration = 2;
    [SerializeField]
    private int _extrovertThreshold = 10;
    [SerializeField]
    private int _extrovertHappinessModifier = 20;
    [SerializeField]
    private int _introvertHappinessModifier = 20;
    [SerializeField]
    private int _romanticHappinessModifier = 10;
    [SerializeField]
    private int _meditatedHappinessModifier = 10;
    [SerializeField]
    private int _meditatedDuration = 3;
    [SerializeField]
    private int _enlightenedThreshold = 5;
    [SerializeField]
    private int _enlightenedHappinessModifier = 30;
    [SerializeField]
    private int _riskTakerHappinessModifier = 20;
    [SerializeField]
    private int _riskAverseHappinessModifier = 20;

#pragma warning restore 0649

    public static MentalStateManager Instance { get; private set; }
    private Dictionary<string, Func<Player, Personality>> _personalities;
    private Dictionary<string, Func<Player, SelfReflectionState>> _selfReflectionStates;
    private Func<Player, SelfReflectionState> _enlightenmentState;

    public int tragedyDuration => _tragedyDuration;
    public int extrovertThreshold => _extrovertThreshold;
    public int extrovertHappinessModifier => _extrovertHappinessModifier;
    public int introvertHappinessModifier => _introvertHappinessModifier;
    public int romanticHappinessModifier => _romanticHappinessModifier;
    public int meditatedHappinessModifier => _meditatedHappinessModifier;
    public int meditatedDuration => _meditatedDuration;
    public int enlightenedThreshold => _enlightenedThreshold;
    public int enlightenedHappinessModifier => _enlightenedHappinessModifier;
    public int riskTakerHappinessModifier => _riskTakerHappinessModifier;
    public int riskAverseHappinessModifier => _riskAverseHappinessModifier;

    private void Awake()
    {
        Instance = this;
    }

    public int GetTragedyPenalty(Player player)
    {
        return player.states.Exists(s => s is Tranquil) ? _tranquilTragedyPenalty : _defaultTragedyPenalty;
    }

    public void Initialize()
    {
        _enlightenmentState = p => new Enlightenment(p);

        List<Func<Player, SelfReflectionState>> states =
            new List<Func<Player, SelfReflectionState>>();
        states.Add(p => new FamilyOrientedState(p));
        states.Add(p => new Frugality(p));
        states.Add(p => new Minimalism(p));
        states.Add(p => new Hustling(p));
        states.Add(p => new Tranquil(p));
        states.Add(p => new Extravagant(p));

        _selfReflectionStates = new Dictionary<string, Func<Player, SelfReflectionState>>();
        foreach (Func<Player, SelfReflectionState> action in states)
        {
            _selfReflectionStates[action(null).name] = action;
        }

        List<Func<Player, Personality>> personalities = new List<Func<Player, Personality>>();
        personalities.Add(p => new Extrovert(p));
        personalities.Add(p => new Introvert(p));
        personalities.Add(p => new Romantic(p));
        personalities.Add(p => new RiskTaker(p));
        personalities.Add(p => new RiskAverse(p));

        _personalities = new Dictionary<string, Func<Player, Personality>>();
        foreach (Func<Player, Personality> action in personalities)
        {
            _personalities[action(null).name] = action;
        }

    }

    public Personality GetPersonality(Player player, System.Random random)
    {
        List<Func<Player, Personality>> values =
            new List<Func<Player, Personality>>(_personalities.Values);
        return values[random.Next(_personalities.Count)](player);
    }

    public Personality GetPersonalityByName(Player player, string name, bool throwException)
    {
        Func<Player, Personality> func;
        if (_personalities.TryGetValue(name, out func))
        {
            return func(player);
        }
        if (throwException)
        {
            Debug.LogException(
                new Exception(string.Format("Cannot find Personality: {0}", name)));
        }
        return null;
    }

    public SelfReflectionState GetEnlightenmentState(Player player)
    {
        return _enlightenmentState(player);
    }

    public SelfReflectionState GetSelfReflectionState(Player player, System.Random random)
    {
        if (_selfReflectionStates == null || _selfReflectionStates.Count == 0)
        {
            return null;
        }

        int index = random.Next(_selfReflectionStates.Count);
        string key = new List<string>(_selfReflectionStates.Keys)[index];
        Func<Player, SelfReflectionState> func = _selfReflectionStates[key];
        _selfReflectionStates.Remove(key);
        return func(player);
    }

    public SelfReflectionState GetSelfReflectionStateByName(
        Player player, string name, bool throwException)
    {
        SelfReflectionState state = GetEnlightenmentState(player);
        if (state.name == name)
        {
            return state;
        }

        Func<Player, SelfReflectionState> func;
        if (_selfReflectionStates.TryGetValue(name, out func))
        {
            return func(player);
        }

        if (throwException)
        {
            string message = string.Format(
                "Cannot find Self Reflection: {0}", name);
            Debug.LogException(new Exception(message));
        }

        return null;
    }

    public TimedPlayerState CreateTimedStateFromData(
        Player player, TimedPlayerStateData data, bool throwException)
    {
        switch (data.stateType)
        {
            case TimedPlayerStateData.StateType.DIVORCE_PENALTY:
                return new DivorcedPenaltyState(player, data.turn, data.penalty);
            case TimedPlayerStateData.StateType.TRAGEDY_PENALTY:
                return new TragedyPenaltyState(player, data.turn);
            case TimedPlayerStateData.StateType.LUXURY_HAPPINESS:
                return new LuxuryHappinessState(player, data.turn);
            case TimedPlayerStateData.StateType.FAMILY_VACATION_HAPPINESS:
                return new FamilyVacationHappinessState(player, data.turn);
            case TimedPlayerStateData.StateType.MEDITATED:
                return new MeditatedState(player, data.turn);
            default:
                if (throwException)
                {
                    string message = string.Format(
                        "Cannot find Timed State: {0}", data.stateType.ToString());
                    Debug.LogException(new Exception(message));
                }
                return null;
        }
    }
}

