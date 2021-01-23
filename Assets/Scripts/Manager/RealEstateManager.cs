using Actions;
using Assets;
using ScriptableObjects;
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
    private float _defaultEquityPerShare = 0.01f;
    [SerializeField]
    private Vector2 _distressedPurchasePriceRange = new Vector2(0.5f, 0.7f);
    [SerializeField]
    private Vector2 _distressedRehabPriceRange = new Vector2(0.1f, 0.3f);
#pragma warning restore 0649

    public static RealEstateManager Instance { get; private set; }
    public float defaultEquitySplit => _defaultEquitySplit;
    public float defaultEquityPerShare => _defaultEquityPerShare;
    public int maxPrivateLoanLTV => _maxPrivateLoanLTV;

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

    private InvestmentAction GetDistressedRealEstateAction(
        Player player, RealEstateTemplate template, System.Random random)
    {
        float variance = template.priceVariance / 2;
        Vector2 varianceRange = new Vector2(1 - variance, 1 + variance);
        int appraisalPrice = calculatePrice(
            random, template.basePrice, varianceRange, template.priceIncrement);

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
        return new BuyDistressedRealEstateAction(player, asset);
    }

    private InvestmentAction GetRentalRealEstateAction(
        Player player, RealEstateTemplate template, System.Random random)
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
        return new BuyRentalRealEstateAction(player, asset);
    }

    private InvestmentAction GetInvestmentAction(
        Player player, List<RealEstateTemplate> templates, System.Random random)
    {
        RealEstateTemplate template = templates[random.Next(templates.Count)];
//        return GetDistressedRealEstateAction(player, template, random);
        return GetRentalRealEstateAction(player, template, random);
    }

    public InvestmentAction GetSmallInvestmentAction(
        Player player, System.Random random)
    {
        return GetInvestmentAction(player, _smallInvestments, random);
    }

    public InvestmentAction GetLargeInvestmentAction(
        Player player, System.Random random)
    {
        return GetInvestmentAction(player, _largeInvestments, random);
    }

    public RentalRealEstate RefinanceExistingProperty(
        Player player, DistressedRealEstate oldAsset)
    {
        RentalRealEstate newAsset = new RefinancedRealEstate(
            oldAsset,
            player.GetDebtPartners(),
            _maxMortgageLTV,
            _maxPrivateLoanLTV);
        return newAsset;
    }
}
