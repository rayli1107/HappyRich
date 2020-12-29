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
    private float _defaultEquitySplit = 0.8f;
    [SerializeField]
    private float _defaultEquityPerShare = 0.01f;
#pragma warning restore 0649

    public static RealEstateManager Instance { get; private set; }
    public float defaultEquitySplit => _defaultEquitySplit;
    public float defaultEquityPerShare => _defaultEquityPerShare;

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

    private AbstractRealEstate GetInvestment(
        List<RealEstateTemplate> templates, System.Random random)
    {
        RealEstateTemplate template = templates[random.Next(templates.Count)];
        int minPrice = Mathf.FloorToInt(
            template.basePrice * (1 - template.priceVariance) / template.priceIncrement);
        int maxPrice = Mathf.FloorToInt(
            template.basePrice * (1 + template.priceVariance) / template.priceIncrement);
        int price = random.Next(minPrice, maxPrice + 1) * template.priceIncrement;
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

        return new RentalRealEstate(
            template, price, price, annualIncome, _maxMortgageLTV, unitCount);
    }

    public AbstractRealEstate GetSmallInvestment(System.Random random)
    {
        return GetInvestment(_smallInvestments, random);
    }

    public AbstractRealEstate GetLargeInvestment(System.Random random)
    {
        return GetInvestment(_largeInvestments, random);
    }
}
