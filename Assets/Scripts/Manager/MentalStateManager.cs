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
    private List<Action<Player, Action<Personality>>> _personalities;
    private List<Action<Player, Action<SelfReflectionState>>> _selfReflectionStates;
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
        _selfReflectionStates = new List<Action<Player, Action<SelfReflectionState>>>();
        _selfReflectionStates.Add((p, cb) => cb(new FamilyOrientedState(p)));
        _selfReflectionStates.Add((p, cb) => cb(new Frugality(p)));
        _selfReflectionStates.Add((p, cb) => cb(new Minimalism(p)));
        _selfReflectionStates.Add((p, cb) => cb(new Hustling(p)));
        _selfReflectionStates.Add((p, cb) => cb(new Tranquil(p)));
        _selfReflectionStates.Add((p, cb) => cb(new Extravagant(p)));

        _personalities = new List<Action<Player, Action<Personality>>>();
        _personalities.Add((p, cb) => cb(new Extrovert(p)));
        _personalities.Add((p, cb) => cb(new Introvert(p)));
        _personalities.Add((p, cb) => cb(new Romantic(p)));
        _personalities.Add((p, cb) => cb(new RiskTaker(p)));
        _personalities.Add((p, cb) => cb(new RiskAverse(p)));

    }

    public Action<Player, Action<Personality>> GetPersonality(System.Random random)
    {
        return _personalities[random.Next(_personalities.Count)];
    }

    public Action<Player, Action<SelfReflectionState>> GetSelfReflectionState(System.Random random)
    {
        if (_selfReflectionStates == null || _selfReflectionStates.Count == 0)
        {
            return (p, cb) => cb(null);
        }

        int index = random.Next(_selfReflectionStates.Count);
        Action<Player, Action<SelfReflectionState>> action = _selfReflectionStates[index];
        _selfReflectionStates.RemoveAt(index);
        return action;
    }
}

