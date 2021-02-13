using Assets;
using ScriptableObjects;
using System.Collections.Generic;

using DistressedProperty = System.Tuple<
    Assets.PartialRealEstate, Assets.DistressedRealEstate>;
using RentalProperty = System.Tuple<
    Assets.PartialRealEstate, Assets.RentalRealEstate>;

namespace PlayerInfo
{
    public class Portfolio
    {
        public StudentLoan studentLoan { get; private set; }
        public PersonalLoan personalLoan { get; private set; }
        public Car car { get; private set; }
        public int cash { get; private set; }
        public Dictionary<string, PurchasedStock> stocks { get; private set; }
        public List<RentalProperty> rentalProperties { get; private set; }
        public List<DistressedProperty> distressedProperties { get; private set; }
        public List<PartialRealEstate> properties {
            get
            {
                List<PartialRealEstate> properties = new List<PartialRealEstate>();
                foreach (RentalProperty property in rentalProperties)
                {
                    properties.Add(property.Item1);
                }
                foreach (DistressedProperty property in distressedProperties)
                {
                    properties.Add(property.Item1);
                }
                return properties;
            }
        }

        public List<AbstractAsset> assets
        {
            get
            {
                List<AbstractAsset> assets = new List<AbstractAsset>();
                assets.AddRange(otherAssets);
                foreach (KeyValuePair<string, PurchasedStock> entry in stocks)
                {
                    assets.Add(entry.Value);
                }
                assets.AddRange(properties);
                return assets;
            }
        }

        public List<AbstractAsset> otherAssets
        {
            get
            {
                List<AbstractAsset> otherAssets = new List<AbstractAsset>();
                if (car != null)
                {
                    otherAssets.Add(car);
                }
                return otherAssets;
            }
        }

        public List<AbstractLiability> liabilities
        {
            get
            {
                List<AbstractLiability> ret = new List<AbstractLiability>();
                if (studentLoan != null && studentLoan.amount > 0)
                {
                    ret.Add(studentLoan);
                }
                if (personalLoan != null && personalLoan.amount > 0)
                {
                    ret.Add(personalLoan);
                }
                return ret;
            }
        }


        public Portfolio(Profession profession)
        {
            if (profession.autoLoan > 0)
            {
                car = new Car(profession.autoLoan);
            }

            if (profession.jobCost > 0)
            {
                studentLoan = new StudentLoan(profession.jobCost);
            }

            cash = profession.startingCash;
            stocks = new Dictionary<string, PurchasedStock>();
            rentalProperties = new List<RentalProperty>();
            distressedProperties = new List<DistressedProperty>();
        }

        public void AddPersonalLoan(int amount)
        {
            if (personalLoan == null)
            {
                personalLoan = new PersonalLoan(amount);
            }
            else
            {
                personalLoan.Add(amount);
            }
        }

        public void AddCash(int amount)
        {
            cash += amount;
        }

        public void AddStock(AbstractStock stock, int number)
        {
            PurchasedStock purchasedStock = null;
            if (stocks.TryGetValue(stock.name, out purchasedStock))
            {
                purchasedStock.AddCount(number);
            }
            else
            {
                purchasedStock = new PurchasedStock(stock);
                purchasedStock.AddCount(number);
                stocks.Add(stock.name, purchasedStock);
            }
        }

        public bool TryRemoveStock(AbstractStock stock, int number)
        {
            PurchasedStock purchasedStock = null;
            if (stocks.TryGetValue(stock.name, out purchasedStock))
            {
                if (purchasedStock.count == number)
                {
                    stocks.Remove(stock.name);
                    return true;
                }
                else if (purchasedStock.count > number)
                {
                    return purchasedStock.TryRemoveCount(number);
                }
            }
            return false;
        }
    }
}
