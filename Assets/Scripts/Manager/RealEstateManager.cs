using Actions;
using Assets;
using Events.Market;
using PlayerInfo;
using ScriptableObjects;
using System;
using System.Collections.Generic;
using UI.Panels.Assets;
using UnityEngine;
public partial class GameInstanceData
{
    public List<RealEstateTemplate> realEstateTemplatesSmall;
    public List<RealEstateTemplate> realEstateTemplatesLarge;
}

[Serializable]
public class RealEstateTemplate
{
    [SerializeField]
    private string _description;
    public string description => _description;

    [SerializeField]
    private string _label;
    public string label => _label;

    [SerializeField]
    private float _priceVariance;
    public float priceVariance => _priceVariance;

    [SerializeField]
    private Vector2Int _rentalRange;
    public Vector2Int rentalRange => _rentalRange;

    [SerializeField]
    private int _priceIncrement;
    public int priceIncrement => _priceIncrement;

    [SerializeField]
    private int _rentalIncrement;
    public int rentalIncrement => _rentalIncrement;

    [SerializeField]
    private bool _commercial;
    public bool commercial => _commercial;

    [SerializeField]
    private int[] _unitCount;
    public int[] unitCount => _unitCount;

    [SerializeField]
    private int _basePrice;
    public int basePrice => _basePrice;

    public void Initialize(RealEstateProfile profile, System.Random random)
    {
        _description = profile.description;
        _label = profile.label;
        _priceVariance = profile.priceVariance;
        _rentalRange = profile.rentalRange;
        _priceIncrement = profile.priceIncrement;
        _rentalIncrement = profile.rentalIncrement;
        _commercial = profile.commercial;
        _unitCount = profile.unitCount;
        _basePrice = priceIncrement * random.Next(
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

    public Dictionary<string, RealEstateTemplate> realEstateTemplates { get; private set; }



    public PersistentGameData PersistentGameData =>
        GameSaveLoadManager.Instance.persistentGameData;
    public GameInstanceData GameData => PersistentGameData.gameInstanceData;

    private void Awake()
    {
        Instance = this;
    }

    public void Initialize(System.Random random)
    {
        if (GameData.realEstateTemplatesSmall == null ||
            GameData.realEstateTemplatesLarge == null)
        {
            GameData.realEstateTemplatesSmall = new List<RealEstateTemplate>();
            GameData.realEstateTemplatesLarge = new List<RealEstateTemplate>();

            foreach (RealEstateProfile profile in _profiles)
            {
                RealEstateTemplate template = new RealEstateTemplate();
                template.Initialize(profile, random);

                if (profile.smallInvestment)
                {
                    GameData.realEstateTemplatesSmall.Add(template);
                }
                else
                {
                    GameData.realEstateTemplatesLarge.Add(template);
                }
            }
        }

        realEstateTemplates = new Dictionary<string, RealEstateTemplate>();
        foreach (RealEstateTemplate template in GameData.realEstateTemplatesSmall)
        {
            Debug.Assert(!realEstateTemplates.ContainsKey(template.label));
            realEstateTemplates[template.label] = template;
        }

        foreach (RealEstateTemplate template in GameData.realEstateTemplatesLarge)
        {
            Debug.Assert(!realEstateTemplates.ContainsKey(template.label));
            realEstateTemplates[template.label] = template;
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
        System.Random random, int price, Vector2 range, int increment, int? maxPrice = null)
    {
        int x = Mathf.FloorToInt(price * range.x / increment);
        int y = Mathf.FloorToInt(price * range.y);
        if (maxPrice.HasValue)
        {
            y = Mathf.Min(y, maxPrice.Value);
        }
        y /= increment;

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

    private AvailableInvestmentContext GetDistressedRealEstateAction(
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
        rehabPrice = Mathf.Max(rehabPrice, template.priceIncrement);

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
        RealEstateData data = new RealEstateData();
        data.Initialize(
            template.label, purchasePrice, appraisalPrice, annualIncome, unitCount, rehabPrice, appraisalPrice);

        DistressedRealEstate asset = new DistressedRealEstate(
            template,
            data,
            player.GetDebtPartners(),
            maxMortgageLtv,
            _maxDistressedLoanLTV);
        return new AvailableInvestmentContext(
            asset,
            BuyDistressedRealEstateAction.GetBuyAction(player, asset));
    }

    private AvailableInvestmentContext GetRentalRealEstateAction(
        Player player,
        RealEstateTemplate template,
        System.Random random,
        int? maxPrice = null)
    {
        Vector2 variance = new Vector2(
            1 - template.priceVariance, 1 + template.priceVariance);
        int price = calculatePrice(
            random, template.basePrice, variance, template.priceIncrement, maxPrice);
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

        RealEstateData data = new RealEstateData();
        data.Initialize(template.label, price, price, annualIncome, unitCount);

        RentalRealEstate asset = new RentalRealEstate(template, data, ltv);
        return new AvailableInvestmentContext(
            asset,
            BuyRentalRealEstateAction.GetBuyAction(player, asset));
    }

    private AvailableInvestmentContext GetInvestmentAction(
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

    public AvailableInvestmentContext GetSmallRentalAction(
        Player player,
        System.Random random,
        int? maxPrice = null)
    {
        int index = random.Next(GameData.realEstateTemplatesSmall.Count);
        RealEstateTemplate template = GameData.realEstateTemplatesSmall[index];
        return GetRentalRealEstateAction(player, template, random, maxPrice);
    }

    public AvailableInvestmentContext GetSmallInvestmentAction(
        Player player, System.Random random)
    {
        return GetInvestmentAction(player, GameData.realEstateTemplatesSmall, random);
    }

    public AvailableInvestmentContext GetLargeInvestmentAction(
        Player player, System.Random random)
    {
         return GetInvestmentAction(player, GameData.realEstateTemplatesLarge, random);
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
        int offer = calculateOfferPrice(asset.template, random) * asset.unitCount;
        return SellRealEstateEvent.GetEvent(player, index, offer);
    }
}
