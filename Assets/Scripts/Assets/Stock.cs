using System;
using System.Collections.Generic;
using PlayerInfo;
using UnityEngine;

namespace Assets
{
    public class PurchasedStock : AbstractAsset
    {
        public AbstractStock stock { get; private set; }
        public int count { get; private set; }
        public override int value { get { return count * stock.value; } }

        public override int expectedIncome => Mathf.FloorToInt(
            count * stock.value * stock.expectedYield / 100f);
        public override int totalIncome => Mathf.FloorToInt(
            count * stock.value * stock.currentYield / 100f);

        public override string name => stock.longName;

        public PurchasedStock(AbstractStock stock) : base(stock.name, 0, 0)
        {
            this.stock = stock;
            count = 0;
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
    }

    public abstract class AbstractStock
    {
        public string name { get; private set; }
        public abstract string longName { get; }

        public int value { get; protected set; }
        public int prevValue { get; private set; }
        private int _turnCount;
        public float change
        {
            get
            {
                return _turnCount >= 2 ? (value - prevValue) / (float)prevValue : 0f;
            }
        }

        public virtual int expectedYield => 0;
        public virtual int currentYield => 0;

        public AbstractStock(string name, int value)
        {
            this.name = name;
            this.value = value;
            prevValue = value;
            _turnCount = 0;
        }

        public virtual void OnTurnStart(System.Random random)
        {
            ++_turnCount;
            prevValue = value;
        }

        public abstract void OnDetail(Action callback);
        /*
        public virtual string GetDescription()
        {
            List<string> messages = new List<string>();
            messages.Add(string.Format("Name: {0}", name));
            messages.Add(string.Format("Price: {0}", value));
            if (yieldRange.y == yieldRange.x)
            {
                messages.Add(string.Format("Yield {0}%", yieldRange.x));
            }
            else
            {
                messages.Add(string.Format("Yield {0}% - {1}%", yieldRange.x, yieldRange.y));
            }
            return string.Join("\n", messages);
        }
        */
    }

    public class GrowthStock : AbstractStock {
        public override string longName => string.Format("Growth Stock - {0}", name);
        private int _currentPeriodTurn;
        public float basePrice { get; private set; }
        public float variance => (value - basePrice) / basePrice;


        public GrowthStock(string name, int initialPrice) : base(name, initialPrice)
        {
            _currentPeriodTurn = 0;
            basePrice = initialPrice;
        }

        public override void OnTurnStart(System.Random random)
        {
            base.OnTurnStart(random);
            ++_currentPeriodTurn;
            if (_currentPeriodTurn > StockManager.Instance.growthStockMinPeriod &&
                random.NextDouble() < StockManager.Instance.growthStockUpdateChance)
            {
                basePrice = basePrice * StockManager.Instance.getGrowthStockGrowth(random);
                _currentPeriodTurn = 0;
            }

            float newBase = (value + basePrice) / 2;
            value = Mathf.RoundToInt(newBase * StockManager.Instance.getGrowthStockVariance(random));
            value = Math.Max(value, 1);

            Debug.LogFormat("{0} prev {1} cur {2} change {3} variance {4}",
                name, prevValue, value, change, variance);
        }

        public override void OnDetail(Action callback)
        {
            UI.UIManager.Instance.ShowGrowthStockPanel(this, callback);
        }
    }

    public class YieldStock : AbstractStock
    {
        public override string longName => string.Format("Dividend Stock - {0}", name);
        public Vector2Int yieldRange { get; private set; }
        public override int expectedYield => yieldRange.x;
        public override int currentYield => _currentYield;
        private int _currentYield;

        public YieldStock(string name, int initialPrice, Vector2Int yieldRange)
            : base(name, initialPrice)
        {
            this.yieldRange = yieldRange;
            _currentYield = 0;
        }

        public override void OnTurnStart(System.Random random)
        {
            base.OnTurnStart(random);
            _currentYield = random.Next(yieldRange.x, yieldRange.y + 1);
        }

        public override void OnDetail(Action callback)
        {
            UI.UIManager.Instance.ShowYieldStockPanel(this, callback);
        }

    }

    public abstract class AbstractCryptoCurrency : AbstractStock
    {
        public override string longName => string.Format("Cryptocurrency - {0}", name);
        private int _turnDelay;
        private int _turn;

        public bool tookOff => _turn >= _turnDelay;

        public AbstractCryptoCurrency(string name, int initialPrice, int turnDelay) : base(name, initialPrice)
        {
            _turn = 0;
            _turnDelay = turnDelay;
        }

        public abstract void OnTurnStartDelayed(System.Random random);

        public override void OnTurnStart(System.Random random)
        {
            base.OnTurnStart(random);
            ++_turn;
            if (tookOff)
            {
                OnTurnStartDelayed(random);
            }
        }
        public override void OnDetail(Action callback)
        {
            UI.UIManager.Instance.ShowCryptoPanel(this, callback);
        }
    }

    public class FailedCryptoCurrency : AbstractCryptoCurrency
    {
        public FailedCryptoCurrency(string name, int initialPrice, int turnDelay) :
            base(name, initialPrice, turnDelay)
        {
        }

        public override void OnTurnStartDelayed(System.Random random)
        {
            value = 0;
        }
    }

    public class SuccessfulCryptoCurrency : AbstractCryptoCurrency
    {
        private bool _first;
        private Vector2Int _initialRange;
        private Vector2 _multiplierRange;

        public SuccessfulCryptoCurrency(string name, int initialPrice, int turnDelay, Vector2Int initialRange, Vector2 multiplierRange) :
            base(name, initialPrice, turnDelay)
        {
            _first = true;
            _initialRange = initialRange;
            _multiplierRange = multiplierRange;
        }

        public override void OnTurnStartDelayed(System.Random random)
        {
            if (_first)
            {
                value *= _initialRange.x + random.Next(_initialRange.y - _initialRange.x + 1);
                _first = false;
            }
            else if (value > 0)
            {
                double multiplier = _multiplierRange.x + random.NextDouble() * (_multiplierRange.y - _multiplierRange.x);
                value = Mathf.RoundToInt(value * (float)multiplier);
            }
        }
    }

}
