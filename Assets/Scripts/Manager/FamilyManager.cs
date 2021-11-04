using Actions;
using PlayerInfo;
using PlayerState;
using ScriptableObjects;
using System;
using System.Collections.Generic;
using UnityEngine;

public class FamilyManager : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField]
    private float _marriageProbability = 0.2f;
    [SerializeField]
    private Vector2 _divorceProbabilityRange = new Vector2(0f, 0.4f);
    [SerializeField]
    private int _divorcePenaltyDuration = 3;
    [SerializeField]
    private SpouseProfile[] _spouses;
    [SerializeField]
    private float[] _newChildProbabilities;
    [SerializeField]
    private int _childHappiness = 10;
    [SerializeField]
    private int _familyOrientedChildThreshold = 3;
    [SerializeField]
    private int _familyOrientedHappinessModifier = 10;
#pragma warning restore 0649

    public static FamilyManager Instance { get; private set; }

    public int divorcePenaltyDuration => _divorcePenaltyDuration;
    public int childHappiness => _childHappiness;
    public int familyOrientedChildThreshold => _familyOrientedChildThreshold;
    public int familyOrientedHappinessModifier => _familyOrientedHappinessModifier;
    private System.Random _random;

    private void Awake()
    {
        Instance = this;
    }

    public void Initialize(System.Random random)
    {
        _random = random;
    }


    private int calculateValue(
        int min, int max, int increment)
    {
        int n1 = min / increment;
        int n2 = max / increment;
        return _random.Next(n1, n2 + 1) * increment;
    }

    private int calculateValue(
        int price, Vector2 range, int increment)
    {
        int x = Mathf.FloorToInt(price * range.x / increment);
        int y = Mathf.FloorToInt(price * range.y / increment);
        return _random.Next(x, y + 1) * increment;
    }

    private Spouse generateSpouseFromProfile(SpouseProfile profile)
    {
        int income = calculateValue(profile.salaryRange.x, profile.salaryRange.y, profile.salaryIncrement);
        int expense = calculateValue(
            income > 0 ?  income : profile.expenseBase, profile.expenseRange, profile.expenseIncrement);
        return new Spouse(income, expense, profile.happiness);
    }

    private float getDivorceProbability(Player player)
    {
        float delta = _divorceProbabilityRange.y - _divorceProbabilityRange.x;
        delta *= -1 * (float)player.happiness / player.maxHappiness;
        return _divorceProbabilityRange.x + delta;
    }

    public List<Action<Action>> GetEvents(Player player)
    {
        List<Action<Action>> events = new List<Action<Action>>();
        if (player.spouse == null)
        {
            if (_random.NextDouble() < _marriageProbability) {
                SpouseProfile profile = _spouses[_random.Next(_spouses.Length)];
                Spouse spouse = generateSpouseFromProfile(profile);
                events.Add(MarriageAction.GetEvent(player, spouse));
            }
        }
        else
        {
            float divorceProbability = getDivorceProbability(player);
            if (!player.states.Exists(s => s is DivorcedPenaltyState) &&
                _random.NextDouble() < divorceProbability)
            {
                events.Add(DivorceAction.GetEvent(player));
            } else if (player.numChild < _newChildProbabilities.Length &&
                _random.NextDouble() < _newChildProbabilities[player.numChild])
            {
                bool isBoy = _random.Next(2) == 0;
                events.Add(NewChildAction.GetEvent(player, isBoy));
            }
        }
        return events;
    }
}