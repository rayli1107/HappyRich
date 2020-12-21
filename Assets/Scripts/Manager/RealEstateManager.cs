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
    private int _defaultMortgageRate = 80;
    [SerializeField]
    private float _defaultEquitySplit = 0.8f;
#pragma warning restore 0649

    public static RealEstateManager Instance { get; private set; }
    public float defaultEquitySplit => _defaultEquitySplit;

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

    private AbstractRealEstate GetInvestment(
        List<RealEstateTemplate> templates, System.Random random)
    {
        RealEstateTemplate template = templates[random.Next(templates.Count)];
        int minPrice = Mathf.FloorToInt(
            template.basePrice * (1 - template.priceVariance) / template.priceIncrement);
        int maxPrice = Mathf.FloorToInt(
            template.basePrice * (1 + template.priceVariance) / template.priceIncrement);
        int price = random.Next(minPrice, maxPrice + 1) * template.priceIncrement;
        int rentRate = random.Next(template.rentalRange.x, template.rentalRange.y + 1);
        int annualIncome = price * rentRate / 100;

        return new RentalRealEstate(
            template, price, price, annualIncome, _defaultMortgageRate);
    }

    public AbstractRealEstate GetSmallInvestment(System.Random random)
    {
        return GetInvestment(_smallInvestments, random);
    }
}
