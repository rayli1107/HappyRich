using Actions;
using Assets;
using PlayerInfo;
using ScriptableObjects;
using System.Collections.Generic;
using UnityEngine;

using Investment = System.Tuple<InvestmentPartner, int>;

public class BusinessManager : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField]
    private BusinessProfile[] _smallInvestments;
    [SerializeField]
    private BusinessProfile[] _largeInvestments;
    [SerializeField]
    private int _maxBusinessLoanLTV = 75;
    [SerializeField]
    private int _maxPrivateLoanLTV = 90;
    [SerializeField]
    private float _defaultEquitySplit = 0.6f;
    [SerializeField]
    private int _maxEquityShares = 100;
#pragma warning restore 0649

    public static BusinessManager Instance { get; private set; }
    public float defaultEquitySplit => _defaultEquitySplit;
    public int maxEquityShares => _maxEquityShares;
    public int maxPrivateLoanLTV => _maxPrivateLoanLTV;
    public int maxBusinessLoanLTV => _maxBusinessLoanLTV;

    private void Awake()
    {
        Instance = this;
    }

    public void Initialize(System.Random random)
    {
    }

    private int calculateValueFactor(
        System.Random random, int price, Vector2Int range, int increment)
    {
        return calculateValue(
            random, price / 1000 * range.x, price / 1000 * range.y, increment);
    }

    private int calculatePrice(
        System.Random random, Vector2Int range, int increment)
    {
        return calculateValue(random, range.x, range.y, increment);
    }

    private int calculateValue(
        System.Random random, int min, int max, int increment)
    {
        int n1 = min / increment;
        int n2 = max / increment;
        return random.Next(n1, n2 + 1) * increment;
    }

    private AbstractBuyInvestmentAction GetInvestmentAction(
        Player player,
        BusinessProfile[] templates,
        System.Random random,
        ActionCallback callback)
    {
        BusinessProfile profile = templates[random.Next(templates.Length)];
        int price = calculatePrice(
            random, profile.priceRange, profile.priceIncrement);
        int minIncome = calculateValueFactor(
            random, price, profile.minIncomeRange, profile.incomeIncrement);
        int maxIncome = calculateValueFactor(
            random, price, profile.maxIncomeRange, profile.incomeIncrement);
        int actualIncome = calculateValue(
            random, minIncome, maxIncome, profile.incomeIncrement);
        int franchiseFee = 0;
        if (profile.franchise)
        {
            franchiseFee = calculateValueFactor(
                random, price, profile.franchiseFeeRange, profile.priceIncrement);
        }
        string description = profile.descriptions[random.Next(profile.descriptions.Length)];
        Debug.LogFormat("Min income {0} max income {1} actual {2}", minIncome, maxIncome, actualIncome);
        Business business = new Business(
            description,
            price,
            franchiseFee,
            minIncome,
            maxIncome,
            actualIncome,
            0,
            _maxBusinessLoanLTV);
        return new BuyBusinessAction(player, business, callback);
    }

    public AbstractBuyInvestmentAction GetSmallInvestmentAction(
        Player player, System.Random random, ActionCallback callback)
    {
        return GetInvestmentAction(player, _smallInvestments, random, callback);
    }

    public AbstractBuyInvestmentAction GetLargeInvestmentAction(
        Player player, System.Random random, ActionCallback callback)
    {
        return GetInvestmentAction(player, _largeInvestments, random, callback);
    }
}
