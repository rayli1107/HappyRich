using System;
using System.Collections.Generic;
using PlayerInfo;
using UnityEngine;

namespace Assets
{
    [Serializable]
    public struct PurchasedStockData
    {
        public int count;
        public string name;
    }

    public class PurchasedStock : AbstractAsset
    {
        public AbstractStock stock { get; private set; }
        public int count { get; private set; }
        public override int value { get { return count * stock.value; } }

        public override string name => stock.longName;

        public override Vector2Int totalIncomeRange => new Vector2Int(
            Mathf.FloorToInt(count * stock.value * stock.yieldRange.x / 100f),
            Mathf.FloorToInt(count * stock.value * stock.yieldRange.y / 100f));

        public PurchasedStock(AbstractStock stock, int count=0) : base(stock.name, 0, Vector2Int.zero)
        {
            this.stock = stock;
            this.count = count;
        }

        public void AddCount(int amount)
        {
            count += amount;
            count = Math.Max(count, 0);
        }

        public bool TryRemoveCount(int amount)
        {
            if (count >= amount)
            {
                count -= amount;
                return true;
            }

            return false;
        }

        public override void OnDetail(Player player, Action callback)
        {
            stock.OnDetail(callback);
        }

        public override int GetActualIncome(System.Random random)
        {
            int yield = random.Next(stock.yieldRange.x, stock.yieldRange.y + 1);
            return Mathf.FloorToInt(count * stock.value * yield / 100f);
        }

        public PurchasedStockData SaveToStockData()
        {
            PurchasedStockData data;
            data.name = stock.name;
            data.count = count;
            return data;
        }
    }

    [Serializable]
    public class StockData
    {
        public enum StockType
        {
            GROWTH_STOCK,
            YIELD_STOCK,
            SUCCESSFUL_CRYPTO,
            FAILED_CRYPTO
        }

        public StockType stockType;
        public string name;
        public int value;
        public int prevValue;
        public Vector2Int yieldRange;
        public int turnCount;

        // Growth Stock Info
        public int currentPeriodTurn;
        public float basePrice;

        // Crypto
        public int cryptoTurnDelay;

        // Successful Crypto
        public bool cryptoFirstBoost;
        public Vector2Int cryptoInitialRange;
        public Vector2 cryptoMultiplierRange;

        public StockData(StockType stockType, string name, int value, Vector2Int yieldRange)
        {
            this.stockType = stockType;
            this.name = name;
            this.value = value;
            this.yieldRange = yieldRange;
            prevValue = value;
            turnCount = 0;
        }
    }

    public abstract class AbstractStock
    {
        protected StockData _stockData;
        public string name => _stockData.name;
        public abstract string longName { get; }

        public int value => _stockData.value;
        public int prevValue => _stockData.prevValue;
        public float change
        {
            get
            {
                return _stockData.turnCount >= 2 ? (value - prevValue) / (float)prevValue : 0f;
            }
        }
        public Vector2Int yieldRange => _stockData.yieldRange;

        public AbstractStock(StockData stockData)
        {
            _stockData = stockData;
        }

        public virtual void OnTurnStart(System.Random random)
        {
            ++_stockData.turnCount;
            _stockData.prevValue = value;
        }

        public abstract void OnDetail(Action callback);
    }

    public class GrowthStock : AbstractStock {
        public override string longName => string.Format("Growth Stock - {0}", name);
        public float basePrice => _stockData.basePrice;
        public float variance => (value - basePrice) / basePrice;


        public GrowthStock(StockData stockData) : base(stockData)
        {
        }

        public override void OnTurnStart(System.Random random)
        {
            base.OnTurnStart(random);
            ++_stockData.currentPeriodTurn;
            if (_stockData.currentPeriodTurn > StockManager.Instance.growthStockMinPeriod &&
                random.NextDouble() < StockManager.Instance.growthStockUpdateChance)
            {
                _stockData.basePrice = basePrice * StockManager.Instance.getGrowthStockGrowth(random);
                _stockData.currentPeriodTurn = 0;
            }

            float newBase = (value + basePrice) / 2;
            _stockData.value = Mathf.RoundToInt(newBase * StockManager.Instance.getGrowthStockVariance(random));
            _stockData.value = Math.Max(value, 1);

            Debug.LogFormat("{0} prev {1} cur {2} change {3} variance {4}",
                name, prevValue, value, change, variance);
        }

        public override void OnDetail(Action callback)
        {
            UI.UIManager.Instance.ShowGrowthStockPanel(this, callback);
        }

        public static StockData CreateStockData(string name, int initialPrice)
        {
            StockData stockData = new StockData(
                StockData.StockType.GROWTH_STOCK, name, initialPrice, Vector2Int.zero);
            stockData.currentPeriodTurn = 0;
            stockData.basePrice = initialPrice;
            return stockData;
        }
    }

    public class YieldStock : AbstractStock
    {
        public override string longName => string.Format("Dividend Stock - {0}", name);

        public YieldStock(StockData stockData) : base(stockData)
        {
        }

        public override void OnDetail(Action callback)
        {
            UI.UIManager.Instance.ShowYieldStockPanel(this, callback);
        }

        public static StockData CreateStockData(string name, int initialPrice, Vector2Int range)
        {
            return new StockData(
                StockData.StockType.YIELD_STOCK, name, initialPrice, range);
        }
    }

    public abstract class AbstractCryptoCurrency : AbstractStock
    {
        public override string longName => string.Format("Cryptocurrency - {0}", name);

        public bool tookOff => _stockData.turnCount >= _stockData.cryptoTurnDelay;

        public AbstractCryptoCurrency(StockData stockData) : base(stockData)
        {
        }

        public abstract void OnTurnStartDelayed(System.Random random);

        public override void OnTurnStart(System.Random random)
        {
            base.OnTurnStart(random);
            if (tookOff)
            {
                OnTurnStartDelayed(random);
            }
        }

        public override void OnDetail(Action callback)
        {
            UI.UIManager.Instance.ShowCryptoPanel(this, callback);
        }

        public static AbstractCryptoCurrency CreateStockFromData(StockData stockData)
        {
            switch (stockData.stockType)
            {
                case StockData.StockType.SUCCESSFUL_CRYPTO:
                    return new SuccessfulCryptoCurrency(stockData);
                case StockData.StockType.FAILED_CRYPTO:
                    return new FailedCryptoCurrency(stockData);
                default:
                    return null;
            }
        }
    }

    public class FailedCryptoCurrency : AbstractCryptoCurrency
    {
        public FailedCryptoCurrency(StockData stockData) : base(stockData)
        {
        }

        public override void OnTurnStartDelayed(System.Random random)
        {
            _stockData.value = 0;
        }

        public static StockData CreateStockData(string name, int initialPrice, int turnDelay)
        {
            StockData stockData = new StockData(
                StockData.StockType.FAILED_CRYPTO, name, initialPrice, Vector2Int.zero);
            stockData.cryptoTurnDelay = turnDelay;
            return stockData;
        }
    }

    public class SuccessfulCryptoCurrency : AbstractCryptoCurrency
    {
        public SuccessfulCryptoCurrency(StockData stockData) : base(stockData)
        {
        }

        public override void OnTurnStartDelayed(System.Random random)
        {
            if (_stockData.cryptoFirstBoost)
            {
                _stockData.value *=
                    _stockData.cryptoInitialRange.x +
                    random.Next(_stockData.cryptoInitialRange.y - _stockData.cryptoInitialRange.x + 1);
                _stockData.cryptoFirstBoost = false;
            }
            else if (value > 0)
            {
                double multiplier =
                    _stockData.cryptoMultiplierRange.x +
                    random.NextDouble() * (_stockData.cryptoMultiplierRange.y - _stockData.cryptoMultiplierRange.x);
                _stockData.value = Mathf.RoundToInt(_stockData.value * (float)multiplier);
            }
        }

        public static StockData CreateStockData(
            string name,
            int initialPrice,
            int turnDelay,
            Vector2Int initialRange,
            Vector2 multiplierRange)
        {
            StockData stockData = new StockData(
                StockData.StockType.SUCCESSFUL_CRYPTO, name, initialPrice, Vector2Int.zero);
            stockData.cryptoTurnDelay = turnDelay;
            stockData.cryptoFirstBoost = true;
            stockData.cryptoInitialRange = initialRange;
            stockData.cryptoMultiplierRange = multiplierRange;
            return stockData;
        }
    }

}
