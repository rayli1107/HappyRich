using PlayerInfo;
using PlayerState;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SelfImprovementManager : MonoBehaviour
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
    private int _extrovertHappinessModifier = 10;
    [SerializeField]
    private int _meditatedHappinessModifier = 10;
    [SerializeField]
    private int _meditatedDuration = 3;
    [SerializeField]
    private int _enlightenedThreshold = 5;
    [SerializeField]
    private int _enlightenedHappinessModifier = 30;

#pragma warning restore 0649

    public static SelfImprovementManager Instance { get; private set; }
    private List<Action<Player, Action<AbstractPlayerState>>> _selfReflectionStates;
    public int tragedyDuration => _tragedyDuration;
    public int extrovertThreshold => _extrovertThreshold;
    public int extrovertHappinessModifier => _extrovertHappinessModifier;
    public int meditatedHappinessModifier => _meditatedHappinessModifier;
    public int meditatedDuration => _meditatedDuration;
    public int enlightenedThreshold => _enlightenedThreshold;
    public int enlightenedHappinessModifier => _enlightenedHappinessModifier;

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
        _selfReflectionStates = new List<Action<Player, Action<AbstractPlayerState>>>();
        _selfReflectionStates.Add((p, cb) => cb(new FamilyOrientedState(p)));
/*
        _selfReflectionStates.Add((p, cb) => cb(new Frugality(p)));
        _selfReflectionStates.Add((p, cb) => cb(new Minimalism(p)));
        _selfReflectionStates.Add((p, cb) => cb(new Hustling(p)));
        _selfReflectionStates.Add((p, cb) => cb(new Tranquil(p)));
        _selfReflectionStates.Add((p, cb) => cb(new Extrovert(p)));
        _selfReflectionStates.Add((p, cb) => cb(new Extravagant(p)));
        */
    }

    public Action<Player, Action<AbstractPlayerState>> GetSelfReflectionState(System.Random random)
    {
        if (_selfReflectionStates == null || _selfReflectionStates.Count == 0)
        {
            return (p, cb) => cb(null);
        }

        int index = random.Next(_selfReflectionStates.Count);
        Action<Player, Action<AbstractPlayerState>> action = _selfReflectionStates[index];
        _selfReflectionStates.RemoveAt(index);
        return action;
    }
}

