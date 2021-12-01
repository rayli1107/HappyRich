using Actions;
using Assets;
using PlayerInfo;
using ScriptableObjects;
using System;
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
    [SerializeField]
    private int _operationIncomeBonusStartup = 40;
    [SerializeField]
    private int _operationIncomeBonusFranchise = 30;
    [SerializeField]
    private int _maxStartupLoanLTV = 20;
    [SerializeField]
    private int _loanAgentStartupLoanLTV = 50;
    [SerializeField]
    private string[] _startupIdeas;
    [SerializeField]
    private Vector3Int _startupPriceRange = new Vector3Int(2, 10, 500000);
    [SerializeField]
    private Vector2Int _startupDuration = new Vector2Int(4, 7);
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
        System.Random random, int price, Vector2Int range, int increment, int bonus = 0)
    {
        return calculateValue(
            random, price / 1000 * (range.x + bonus), price / 1000 * (range.y + bonus), increment);
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

    private BuyInvestmentContext GetBusinessInvestmentAction(
        Player player,
        BusinessProfile[] templates,
        System.Random random)
    {
        if (templates == null || templates.Length == 0)
        {
            return new BuyInvestmentContext(null, null);
        }

        BusinessProfile profile = templates[random.Next(templates.Length)];
        int price = calculatePrice(
            random, profile.priceRange, profile.priceIncrement);
        int franchiseFee = 0;
        if (profile.franchise)
        {
            franchiseFee = calculateValueFactor(
                random, price, profile.franchiseFeeRange, profile.priceIncrement);
            franchiseFee = Mathf.Min(franchiseFee, profile.priceIncrement);
        }

        int minIncomeBonus = 0;
        if (player.HasSkill(SkillType.BUSINESS_OPERATIONS))
        {
            minIncomeBonus = profile.franchise ?
                _operationIncomeBonusFranchise :
                _operationIncomeBonusStartup;
        }
        int minIncome = calculateValueFactor(
            random, price + franchiseFee, profile.minIncomeRange, profile.incomeIncrement, minIncomeBonus);
        int maxIncome = calculateValueFactor(
            random, price + franchiseFee, profile.maxIncomeRange, profile.incomeIncrement);
        minIncome = Mathf.Min(minIncome, maxIncome);

        int actualIncome = calculateValue(
            random, minIncome, maxIncome, profile.incomeIncrement);
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
        if (profile.franchise)
        {
            return new BuyInvestmentContext(
                business, JoinFranchiseAction.GetBuyAction(player, business));
        }
        else
        {
            return new BuyInvestmentContext(
                business, PurchaseSmallBusinessAction.GetBuyAction(player, business));
        }
    }

    public BuyInvestmentContext GetSmallInvestmentAction(
        Player player, System.Random random)
    {
        return GetBusinessInvestmentAction(player, _smallInvestments, random);
    }

    private BuyInvestmentContext GetStartupInvestmentAction(
        Player player, System.Random random)
    {
        string idea = _startupIdeas[random.Next(_startupIdeas.Length)];
        string label = string.Format("Startup: {0}", idea);
        int cost = random.Next(
            _startupPriceRange.x, _startupPriceRange.y + 1) * _startupPriceRange.z;
        int duration = random.Next(_startupDuration.x, _startupDuration.y + 1);
        int ltv = player.specialists.Exists(
            s => s.specialistType == SpecialistType.LOAN_AGENT) ?
            _loanAgentStartupLoanLTV : _maxStartupLoanLTV;
        Startup startup = new Startup(idea, label, cost, duration, ltv, ltv);
        return new BuyInvestmentContext(
            startup, PurchaseStartupAction.GetBuyAction(player, startup));
    }

    public BuyInvestmentContext GetLargeInvestmentAction(
        Player player, System.Random random)
    {
        return GetStartupInvestmentAction(player, random);
    }
}
