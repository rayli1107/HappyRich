using Actions;
using Assets;
using System;
using System.Collections.Generic;
using UI.Panels.Templates;
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
    private Vector2 _growthStockGrowth = new Vector2(0.9f, 1.2f);
    [SerializeField]
    private Vector2 _growthStockVariance = new Vector2(0.8f, 1.2f);
    [SerializeField]
    private int _growthStockMinPeriod = 3;
    [SerializeField]
    private float _growthStockUpdateChance = 0.5f;
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
    [SerializeField]
    private Vector2Int[] _yieldStockYields;
    [SerializeField]
    private float _tipThreshold = 0.15f;
#pragma warning restore 0649

    public static StockManager Instance;

    private Dictionary<string, AbstractStock> _stocks;
    public List<GrowthStock> growthStocks { get; private set; }
    public List<YieldStock> yieldStocks { get; private set; }
    public List<AbstractCryptoCurrency> cryptoCurrencies { get; private set; }
    public int numCryptoCurrencies => _numCryptoCurrencies;
    public int growthStockMinPeriod => _growthStockMinPeriod;
    public float growthStockUpdateChance => _growthStockUpdateChance;
    public float tipThreshold => _tipThreshold;
    public bool stockEvaluated { get; private set; }

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
        growthStocks = new List<GrowthStock>();
        yieldStocks = new List<YieldStock>();
        cryptoCurrencies = new List<AbstractCryptoCurrency>();
        stockEvaluated = false;

        for (int i = 0; i < _numGrowthStock; ++i) {
            string name = generateStockName(random);
            int value = random.Next(_initialPriceMin, _initialPriceMax + 1);
            GrowthStock stock = new GrowthStock(name, value);
            growthStocks.Add(stock);
            _stocks.Add(name, stock);
        }

        for (int i = 0; i < _yieldStockYields.Length; ++i)
        {
            string name = generateStockName(random);
            int value = random.Next(_initialPriceMin, _initialPriceMax + 1);
            YieldStock stock = new YieldStock(name, value, _yieldStockYields[i]);
            yieldStocks.Add(stock);
            _stocks.Add(name, stock);
        }
    }

    public void EvaluateStocks()
    {
        stockEvaluated = true;
    }

    public Tuple<string, AbstractStock> CreateNewCryptoCurrency(System.Random random)
    {
        string name = generateStockName(random);
        int delay = random.Next(_cryptoTurnDelayRange.y - _cryptoTurnDelayRange.x + 1) + _cryptoTurnDelayRange.x;
        AbstractCryptoCurrency crypto;
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
        stockEvaluated = false;
        foreach (KeyValuePair<string, AbstractStock> entry in _stocks)
        {
            entry.Value.OnTurnStart(random);
        }
    }

    private float generateFromRange(System.Random random, float min, float max)
    {
        return min + (float)random.NextDouble() * (max - min);
    }

    private float generateFromRange(System.Random random, Vector2 range)
    {
        return generateFromRange(random, range.x, range.y);
    }


    public float getGrowthStockGrowth(System.Random random)
    {
        return generateFromRange(random, _growthStockGrowth);
    }

    public float getGrowthStockVariance(System.Random random)
    {
        return generateFromRange(random, _growthStockVariance);
    }

    private void getMarketEventNewCrypto(System.Random random, Action callback)
    {
        System.Tuple<string, AbstractStock> crypto = CreateNewCryptoCurrency(random);
        string stockName = Localization.Instance.GetStockName(crypto.Item2);
        EventLogManager.Instance.LogFormat(
            "Market Event - New Crypto {0}", stockName);
        string message = string.Format(
            "A new cryptocurrency {0} has just been launched!", stockName);
        UI.UIManager.Instance.ShowSimpleMessageBox(
            message, ButtonChoiceType.OK_ONLY, (_) => callback?.Invoke());
    }

    public Action<Action> GetMarketEvent(System.Random random)
    {
        List<Action<Action>> actions = new List<Action<Action>>();
        if (cryptoCurrencies.Count < numCryptoCurrencies)
        {
            return cb => getMarketEventNewCrypto(random, cb);
        }
        return null;
    }

    public AbstractStock GetStockByName(string name)
    {
        AbstractStock result;
        if (_stocks.TryGetValue(name, out result))
        {
            return result;
        }
        return null;
    }
}
