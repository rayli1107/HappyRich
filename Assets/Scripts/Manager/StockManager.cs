using Assets;
using System;
using System.Collections.Generic;
using UnityEngine;

public class StockManager : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField]
    private int _initialPriceMin = 20;
    [SerializeField]
    private int _initialPriceMax = 100;
    [SerializeField]
    private int _numGrowthStock;
    [SerializeField]
    private int _numCryptoCurrencies;
    [SerializeField]
    private float _growthStockRangeMin = 0.9f;
    [SerializeField]
    private float _growthStockRangeMax = 1.2f;
    [SerializeField]
    private float _cryptoSuccessProbability = 0.5f;
    [SerializeField]
    private int _cryptoInitialValue = 1;
    [SerializeField]
    private Vector2Int _cryptoTurnDelayRange = new Vector2Int(5, 8);
    [SerializeField]
    private Vector2Int _cryptoInitialMultiplier = new Vector2Int(5, 10);
    [SerializeField]
    private Vector2 _cryptoGrowthMultiplier = new Vector2(0.7f, 1.3f);

#pragma warning restore 0649

    public static StockManager Instance;

    private Dictionary<string, AbstractStock> _stocks;
    public List<AbstractStock> growthStocks { get; private set; }
    public List<AbstractStock> cryptoCurrencies { get; private set; }
    public int numCryptoCurrencies => _numCryptoCurrencies;

    private void Awake()
    {
        Instance = this;
    }

    private char generateChar(System.Random random)
    {
        return Convert.ToChar(Convert.ToInt32('A') + random.Next(26));
    }

    private string generateStockName(System.Random random)
    {
        while (true)
        {
            char[] chars = new char[] {
                generateChar(random), generateChar(random), generateChar(random) };
            string name = new string(chars);
            if (!_stocks.ContainsKey(name))
            {
                return name;
            }
        }
    }

    public void Initialize(System.Random random)
    {
        _stocks = new Dictionary<string, AbstractStock>();
        growthStocks = new List<AbstractStock>();
        cryptoCurrencies = new List<AbstractStock>();

        for (int i = 0; i < _numGrowthStock; ++i) {
            string name = generateStockName(random);
            int value = random.Next(_initialPriceMin, _initialPriceMax + 1);
            GrowthStock stock = new GrowthStock(name, value);
            growthStocks.Add(stock);
            _stocks.Add(name, stock);
        }
    }

    public Tuple<string, AbstractStock> CreateNewCryptoCurrency(System.Random random)
    {
        string name = generateStockName(random);
        int delay = random.Next(_cryptoTurnDelayRange.y - _cryptoTurnDelayRange.x + 1) + _cryptoTurnDelayRange.x;
        AbstractStock crypto;
        if (random.NextDouble() < _cryptoSuccessProbability)
        {
            crypto = new SuccessfulCryptoCurrency(
                name, _cryptoInitialValue, delay, _cryptoInitialMultiplier, _cryptoGrowthMultiplier);
        }
        else
        {
            crypto = new FailedCryptoCurrency(name, _cryptoInitialValue, delay);
        }
        cryptoCurrencies.Add(crypto);
        _stocks.Add(name, crypto);
        return new Tuple<string, AbstractStock>(name, crypto);
    }

    public void OnTurnStart(System.Random random)
    {
        foreach (KeyValuePair<string, AbstractStock> entry in _stocks)
        {
            entry.Value.OnTurnStart(random);
        }
    }

    private float generateFromRange(System.Random random, float min, float max)
    {
        return min + (float)random.NextDouble() * (max - min);
    }

    public float getGrowthStockGrowth(System.Random random)
    {
        return generateFromRange(random, _growthStockRangeMin, _growthStockRangeMax);
    }
}
