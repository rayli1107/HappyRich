using Actions;
using Assets;
using PlayerInfo;
using ScriptableObjects;
using System.Collections.Generic;
using UnityEngine;

using Investment = System.Tuple<InvestmentPartner, int>;
/*
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

    private InvestmentAction GetDistressedRealEstateAction(
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

    private InvestmentAction GetRentalRealEstateAction(
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

    private InvestmentAction GetInvestmentAction(
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

    public InvestmentAction GetSmallInvestmentAction(
        Player player, System.Random random, ActionCallback callback)
    {
        return GetInvestmentAction(player, _smallInvestments, random, callback);
    }

    public InvestmentAction GetLargeInvestmentAction(
        Player player, System.Random random, ActionCallback callback)
    {
        return GetInvestmentAction(player, _largeInvestments, random, callback);
    }
}
*/