using Actions;
using PlayerInfo;
using ScriptableObjects;
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
#pragma warning restore 0649

    public static FamilyManager Instance { get; private set; }

    public int divorcePenaltyDuration => _divorcePenaltyDuration;
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

    public AbstractAction GetPersonalEventAction(Player player, ActionCallback callback)
    {
        if (player.spouse == null)
        {
            if (_random.NextDouble() < _marriageProbability) {
                SpouseProfile profile = _spouses[_random.Next(_spouses.Length)];
                Spouse spouse = generateSpouseFromProfile(profile);
                return new MarriageAction(player, spouse, callback);
            }
            return null;
        }
        else
        {
            float divorceProbability = getDivorceProbability(player);
            if (_random.NextDouble() < divorceProbability)
            {
                return new DivorceAction(player, callback);
            }
            return null;
        }
    }
}