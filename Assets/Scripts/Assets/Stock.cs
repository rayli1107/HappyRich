using System;
using System.Collections.Generic;

namespace Assets
{
    public class AbstractStock : AbstractAsset
    {
        private float _prevValue;
        public float change
        {
            get
            {
                return (value - _prevValue) / _prevValue;
            }
        }

        public AbstractStock(string name, int price) : base(name, price, null, 0)
        {
            _prevValue = price;
        }

        public virtual void OnTurnStart(Random random)
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

        public override void OnTurnStart(Random random)
        {
            base.OnTurnStart(random);
            value = Convert.ToInt32(
                value * StockManager.Instance.getGrowthStockGrowth(random));
            value = Math.Max(value, 1);
        }
    }
}
