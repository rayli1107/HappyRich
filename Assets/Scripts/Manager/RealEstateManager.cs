﻿using Actions;
using Assets;
using PlayerInfo;
using ScriptableObjects;
using System.Collections.Generic;
using UnityEngine;

using Investment = System.Tuple<InvestmentPartner, int>;

public class RealEstateTemplate
{
    public RealEstateProfile profile { get; private set; }
    public string description => profile.description;
    public string label => profile.label;
    public int basePrice { get; private set; }
    public float priceVariance => profile.priceVariance;
    public Vector2Int rentalRange => profile.rentalRange;
    public int priceIncrement => profile.priceIncrement;
    public int rentalIncrement => profile.rentalIncrement;
    public bool commercial => profile.commercial;
    public int[] unitCount => profile.unitCount;

    public RealEstateTemplate(RealEstateProfile profile, System.Random random)
    {
        this.profile = profile;
        basePrice = priceIncrement * random.Next(
            profile.basePriceRange.x / priceIncrement,
            profile.basePriceRange.y / priceIncrement + 1);
    }
}

public class RealEstateManager : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField]
    private RealEstateProfile[] _profiles;
    [SerializeField]
    private int _maxMortgageLTV = 75;
    [SerializeField]
    private int _maxPrivateLoanLTV = 90;
    [SerializeField]
    private int _maxDistressedLoanLTV = 100;
    [SerializeField]
    private float _defaultEquitySplit = 0.8f;
    [SerializeField]
    private int _maxEquityShares = 100;
    [SerializeField]
    private Vector2 _distressedPurchasePriceRange = new Vector2(0.5f, 0.7f);
    [SerializeField]
    private Vector2 _distressedRehabPriceRange = new Vector2(0.1f, 0.3f);
    [SerializeField]
    private float _sellChance = 1f;
#pragma warning restore 0649

    public static RealEstateManager Instance { get; private set; }
    public float defaultEquitySplit => _defaultEquitySplit;
    public int maxEquityShares => _maxEquityShares;
    public int maxPrivateLoanLTV => _maxPrivateLoanLTV;
    public float sellChance => _sellChance;


    private List<RealEstateTemplate> _smallInvestments;
    private List<RealEstateTemplate> _largeInvestments;

    private void Awake()
    {
        Instance = this;
    }

    public void Initialize(System.Random random)
    {
        _smallInvestments = new List<RealEstateTemplate>();
        _largeInvestments = new List<RealEstateTemplate>();

        foreach (RealEstateProfile profile in _profiles)
        {
            RealEstateTemplate template = new RealEstateTemplate(profile, random);

            if (profile.smallInvestment)
            {
                _smallInvestments.Add(template);
            }
            else
            {
                _largeInvestments.Add(template);
            }
        }
    }

    private int calculateRent(
        System.Random random, int price, Vector2Int range, int increment)
    {
        int rentMin = price / 1000 * range.x / increment;
        int rentMax = price / 1000 * range.y / increment;
        return random.Next(rentMin, rentMax + 1) * increment;
    }

    private int calculatePrice(
        System.Random random, int price, Vector2 range, int increment)
    {
        int x = Mathf.FloorToInt(price * range.x / increment);
        int y = Mathf.FloorToInt(price * range.y / increment);
        return random.Next(x, y + 1) * increment;
    }

    public int calculateOfferPrice(
        RealEstateTemplate template,
        System.Random random)
    {
        float variance = template.priceVariance / 2;
        Vector2 varianceRange = new Vector2(1 - variance, 1 + variance);
        return calculatePrice(
            random, template.basePrice, varianceRange, template.priceIncrement);
    }

    private AbstractBuyInvestmentAction GetDistressedRealEstateAction(
        Player player,
        RealEstateTemplate template,
        System.Random random,
        ActionCallback callback)
    {
        int appraisalPrice = calculateOfferPrice(template, random);

        int rentBasePrice = template.commercial ? appraisalPrice : template.basePrice;
        int annualIncome = calculateRent(
            random, rentBasePrice, template.rentalRange, template.rentalIncrement);
        int purchasePrice = calculatePrice(
            random, template.basePrice, _distressedPurchasePriceRange, template.priceIncrement);
        int rehabPrice = calculatePrice(
            random, template.basePrice, _distressedRehabPriceRange, template.priceIncrement);

        int unitCount = 1;
        if (template.unitCount.Length > 0)
        {
            unitCount = template.unitCount[random.Next(template.unitCount.Length)];
            appraisalPrice *= unitCount;
            annualIncome *= unitCount;
            purchasePrice *= unitCount;
            rehabPrice *= unitCount;
        }

        DistressedRealEstate asset = new DistressedRealEstate(
            template,
            player.GetDebtPartners(),
            purchasePrice,
            rehabPrice,
            appraisalPrice,
            annualIncome,
            unitCount,
            _maxDistressedLoanLTV);
        return new BuyDistressedRealEstateAction(player, asset, callback);
    }

    private AbstractBuyInvestmentAction GetRentalRealEstateAction(
        Player player,
        RealEstateTemplate template,
        System.Random random,
        ActionCallback callback)
    {
        Vector2 variance = new Vector2(
            1 - template.priceVariance, 1 + template.priceVariance);
        int price = calculatePrice(
            random, template.basePrice, variance, template.priceIncrement);
        int rentBasePrice = template.commercial ? price : template.basePrice;
        int annualIncome = calculateRent(
            random, rentBasePrice, template.rentalRange, template.rentalIncrement);

        int unitCount = 1;
        if (template.unitCount.Length > 0)
        {
            unitCount = template.unitCount[random.Next(template.unitCount.Length)];
            price *= unitCount;
            annualIncome *= unitCount;
        }

        RentalRealEstate asset = new RentalRealEstate(
            template, price, price, annualIncome, _maxMortgageLTV, _maxMortgageLTV, unitCount);
        return new BuyRentalRealEstateAction(player, asset, callback);
    }

    private AbstractBuyInvestmentAction GetInvestmentAction(
        Player player,
        List<RealEstateTemplate> templates,
        System.Random random,
        ActionCallback callback)
    {
        RealEstateTemplate template = templates[random.Next(templates.Count)];
        bool rental = false;
        if (rental)
        {
            return GetRentalRealEstateAction(player, template, random, callback);
        }
        else
        {
            return GetDistressedRealEstateAction(player, template, random, callback);
        }
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

    public RefinancedRealEstate RefinanceDistressedProperty(
        Player player, DistressedRealEstate oldAsset)
    {
        RefinancedRealEstate newAsset = new RefinancedRealEstate(
            oldAsset,
            player.GetDebtPartners(),
            _maxMortgageLTV,
            _maxPrivateLoanLTV);
        return newAsset;
    }

    public List<Investment> CalculateReturnedCapitalForRefinance(
        RefinancedRealEstate asset,
        PartialInvestment partialAsset)
    {
        return calculateReturnedCapital(
            partialAsset,
            asset.originalTotalCost,
            asset.originalLoanAmount,
            asset.combinedLiability.amount);
    }

    public List<Investment> CalculateReturnedCapitalForSale(
        RentalRealEstate asset,
        PartialInvestment partialAsset,
        int price)
    {
        return calculateReturnedCapital(
            partialAsset,
            asset.totalCost,
            asset.combinedLiability.amount,
            price);
    }

    private List<Investment> calculateReturnedCapital(
        PartialInvestment partialAsset,
        int originalTotalCost,
        int oldCapitalAmount,
        int newCapitalAmount)
    {
        int totalReturnedCapital = newCapitalAmount - oldCapitalAmount;
        int capital1 = Mathf.Max(
            Mathf.Min(newCapitalAmount, originalTotalCost) - oldCapitalAmount, 0);
        int capital2 = totalReturnedCapital - capital1;

        List<string> messages = new List<string>()
        {
            "calculateReturnedCapital()",
            string.Format("originalTotalCost: {0}", originalTotalCost),
            string.Format("oldCapitalAmount: {0}", oldCapitalAmount),
            string.Format("newCapitalAmount: {0}", newCapitalAmount),
        };
/*
        Debug.LogFormat("Returned capital {0}", returnedCapital);
        Debug.LogFormat("Downpayment {0}", asset.distressedAsset.downPayment);
        Debug.LogFormat("Capital stack {0} {1}", capital1, capital2);
        */
        List<Investment> returnedCapitalList = new List<Investment>();
        returnedCapitalList.Add(null);
        foreach (Investment investment in partialAsset.investments)
        {
            float investorCapitalEquity = investment.Item2 / (float)partialAsset.maxShares;
            float investorEquity = investment.Item2 * partialAsset.equityPerShare;
/*
            Debug.LogFormat("Investor {0} equity {1} {2}",
                investment.Item1.name,
                investorCapitalEquity,
                investorEquity);
                */
            int investorCapital1 = Mathf.FloorToInt(capital1 * investorCapitalEquity);
            int investorCapital2 = Mathf.FloorToInt(capital2 * investorEquity);
            int investorCapital = investorCapital1 + investorCapital2;
            returnedCapitalList.Add(new Investment(investment.Item1, investorCapital));
            /*
             *Debug.LogFormat(
                            "Investor {0} returned capital {1} {2}",
                            investment.Item1.name,
                            investorCapital1,
                            investorCapital2);
                            */
            totalReturnedCapital -= investorCapital;
            messages.Add(string.Format(
                "Investor {0} equity {1} {2} returned capital {3} {4}",
                investment.Item1.name,
                investorCapitalEquity,
                investorEquity,
                investorCapital1,
                investorCapital2));
        }
        messages.Add(string.Format("Owner returned capital: {0}", totalReturnedCapital));
        Debug.Log(string.Join("\n", messages));
        returnedCapitalList[0] = new Investment(null, totalReturnedCapital);
        return returnedCapitalList;
    }
}
