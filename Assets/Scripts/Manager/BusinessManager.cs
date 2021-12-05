﻿using Actions;
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
        if (profile.franchise)
        {
            Franchise franchise = new Franchise(
                description,
                price,
                franchiseFee,
                minIncome,
                maxIncome,
                actualIncome,
                0,
                _maxBusinessLoanLTV);
            return new BuyInvestmentContext(
                franchise, JoinFranchiseAction.GetBuyAction(player, franchise));
        }
        else
        {
            SmallBusiness business = new SmallBusiness(
                description,
                price,
                minIncome,
                maxIncome,
                actualIncome,
                0,
                _maxBusinessLoanLTV);
            return new BuyInvestmentContext(
                business, PurchaseSmallBusinessAction.GetBuyAction(player, business));
        }
    }

    public BuyInvestmentContext GetSmallInvestmentAction(
        Player player, System.Random random)
    {
        return GetBusinessInvestmentAction(player, _smallInvestments, random);
    }
}
