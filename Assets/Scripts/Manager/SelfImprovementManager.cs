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
#pragma warning restore 0649

    public static SelfImprovementManager Instance { get; private set; }
    private List<Action<Player, Action<AbstractPlayerState>>> _selfReflectionStates;

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
        _selfReflectionStates.Add((p, cb) => cb(new Frugality(p)));
        _selfReflectionStates.Add((p, cb) => cb(new Minimalism(p)));
        _selfReflectionStates.Add((p, cb) => cb(new Hustling(p)));
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

