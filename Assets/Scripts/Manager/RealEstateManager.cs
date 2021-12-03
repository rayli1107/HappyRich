using Actions;
using Assets;
using Events.Market;
using PlayerInfo;
using ScriptableObjects;
using System;
using System.Collections.Generic;
using UnityEngine;

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
    [SerializeField]
    private int _loanAgentRentalMortgageLTV = 80;
    [SerializeField]
    private int _loanAgentDistressedMortgageLTV = 20;
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

    private int getRentalMortgageLTV(Player player)
    {
        foreach (SpecialistInfo info in player.specialists)
        {
            if (info.specialistType == SpecialistType.LOAN_AGENT)
            {
                return _loanAgentRentalMortgageLTV;
            }
        }
        return _maxMortgageLTV;
    }

    private int getDistressedMortgageLTV(Player player)
    {
        foreach (SpecialistInfo info in player.specialists)
        {
            if (info.specialistType == SpecialistType.LOAN_AGENT)
            {
                return _loanAgentDistressedMortgageLTV;
            }
        }
        return 0;
    }

    private BuyInvestmentContext GetDistressedRealEstateAction(
        Player player,
        RealEstateTemplate template,
        System.Random random)
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

        int maxMortgageLtv = getDistressedMortgageLTV(player);
        DistressedRealEstate asset = new DistressedRealEstate(
            template,
            player.GetDebtPartners(),
            purchasePrice,
            rehabPrice,
            appraisalPrice,
            annualIncome,
            unitCount,
            maxMortgageLtv,
            _maxDistressedLoanLTV);
        return new BuyInvestmentContext(
            asset, BuyDistressedRealEstateAction.GetBuyAction(player, asset));
    }

    private BuyInvestmentContext GetRentalRealEstateAction(
        Player player,
        RealEstateTemplate template,
        System.Random random)
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

        int ltv = getRentalMortgageLTV(player);
        RentalRealEstate asset = new RentalRealEstate(
            template, price, price, annualIncome, ltv, ltv, unitCount);
        return new BuyInvestmentContext(
            asset, BuyRentalRealEstateAction.GetBuyAction(player, asset));
    }

    private BuyInvestmentContext GetInvestmentAction(
        Player player,
        List<RealEstateTemplate> templates,
        System.Random random)
    {
        RealEstateTemplate template = templates[random.Next(templates.Count)];
        if (random.Next(2) == 0)
        {
            return GetRentalRealEstateAction(player, template, random);
        }
        else
        {
            return GetDistressedRealEstateAction(player, template, random);
        }
    }

    public BuyInvestmentContext GetSmallInvestmentAction(
        Player player, System.Random random)
    {
        return GetInvestmentAction(player, _smallInvestments, random);
    }

    public BuyInvestmentContext GetLargeInvestmentAction(
        Player player, System.Random random)
    {
        return GetInvestmentAction(player, _largeInvestments, random);
    }

    public RefinancedRealEstate RefinanceDistressedProperty(
        Player player, DistressedRealEstate oldAsset)
    {
        int ltv = getRentalMortgageLTV(player);
        RefinancedRealEstate newAsset = new RefinancedRealEstate(
            oldAsset,
            player.GetDebtPartners(),
            ltv,
            _maxPrivateLoanLTV);
        return newAsset;
    }

    public Action<Action> GetMarketEvent(Player player, System.Random random)
    {
        if (player.portfolio.rentalProperties.Count == 0)
        {
            return null;
        }

        int index = random.Next(player.portfolio.rentalProperties.Count);
        RentalRealEstate asset = player.portfolio.rentalProperties[index].Item2;
        int offer = calculateOfferPrice(asset.template, random);
        return SellRealEstateEvent.GetEvent(player, index, offer);
    }
}
