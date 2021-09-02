﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets
{
    public class PurchasedStock : AbstractAsset
    {
        public AbstractStock stock { get; private set; }
        public int count { get; private set; }
        public override int value { get { return count * stock.value; } }


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
    }
    public class AbstractStock
    {
        public string name { get; private set; }
        public int value { get; protected set; }
        public int prevValue { get; private set; }
        public float change
        {
            get
            {
                return (value - prevValue) / (float)prevValue;
            }
        }

        public AbstractStock(string name, int value)
        {
            this.name = name;
            this.value = value;
            prevValue = value;
        }

        public virtual void OnTurnStart(System.Random random)
        {
            prevValue = value;
        }

        public virtual string GetDescription()
        {
            List<string> messages = new List<string>();
            messages.Add(string.Format("Name: {0}", name));
            messages.Add(string.Format("Price: {0}", value));
            messages.Add("Yield 0%");
            return string.Join("\n", messages);
        }
    }

    public class GrowthStock : AbstractStock { 
        public GrowthStock(string name, int initialPrice) : base(name, initialPrice)
        {

        }

        public override void OnTurnStart(System.Random random)
        {
            base.OnTurnStart(random);
            value = Convert.ToInt32(
                value * StockManager.Instance.getGrowthStockGrowth(random));
            value = Math.Max(value, 1);

            Debug.LogFormat("{0} prev {1} cur {2} change {3}",
                name, prevValue, value, change);
        }
    }

    public abstract class AbstractCryptoCurrency : AbstractStock
    {
        private int _turnDelay;
        private int _turn;

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
            if (_turn >= _turnDelay)
            {
                OnTurnStartDelayed(random);
            }
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
            }
            else if (value > 0)
            {
                double multiplier = _multiplierRange.x + random.NextDouble() * (_multiplierRange.y - _multiplierRange.x);
                value = Mathf.FloorToInt(value * (float)multiplier);
            }
        }
    }

}
