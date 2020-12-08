using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets
{
    public class PurchasedStock : AbstractAsset
    {
        public AbstractStock stock { get; private set; }
        public int count { get; private set; }

        public PurchasedStock(AbstractStock stock) : base(stock.name, 0, null, 0)
        {
            this.stock = stock;
            count = 0;
        }

        public void AddCount(int amount)
        {
            count += amount;
            count = Math.Max(count, 0);
        }

        public override int getValue()
        {
            return count * stock.value;
        }

        public override int getIncome()
        {
            return 0;
        }
    }
    public class AbstractStock
    {
        public string name { get; private set; }
        public int value { get; protected set; }
        protected int _prevValue { get; private set; }
        public float change
        {
            get
            {
                return (value - _prevValue) / (float)_prevValue;
            }
        }

        public AbstractStock(string name, int value)
        {
            this.name = name;
            this.value = value;
            _prevValue = value;
        }

        public virtual void OnTurnStart(System.Random random)
        {
            _prevValue = value;
        }

        public virtual string GetDescription()
        {
            return "";
        }
    }

    public class GrowthStock : AbstractStock { 
        public GrowthStock(string name, int initialPrice) : base(name, initialPrice)
        {

        }

        public override string GetDescription()
        {
            List<string> messages = new List<string>();
            messages.Add(string.Format("Name: {0}", name));
            messages.Add(string.Format("Price: {0}", value));
            messages.Add("Yield 0%");
            return string.Join("\n", messages);
        }

        public override void OnTurnStart(System.Random random)
        {
            base.OnTurnStart(random);
            value = Convert.ToInt32(
                value * StockManager.Instance.getGrowthStockGrowth(random));
            value = Math.Max(value, 1);

            Debug.LogFormat("{0} prev {1} cur {2} change {3}",
                name, _prevValue, value, change);
        }
    }
}
