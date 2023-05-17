using Assets;
using ScriptableObjects;
using System.Collections.Generic;
using UnityEngine;
using DistressedProperty = System.Tuple<
    Assets.PartialInvestment, Assets.DistressedRealEstate>;
using RentalProperty = System.Tuple<
    Assets.PartialInvestment, Assets.RentalRealEstate>;
using BusinessEntity = System.Tuple<
    Assets.PartialInvestment, Assets.AbstractInvestment>;
using StartupEntity = System.Tuple<
    Assets.PartialInvestment, Assets.Startup>;
using AssetTypeEntity = System.Tuple<
    string, System.Collections.Generic.List<Assets.AbstractAsset>>;
using System;

public partial class GameInstanceData
{
    public PlayerInfo.PortfolioData portfolioData;
}


namespace PlayerInfo
{
    [Serializable]
    public class InvestmentContextData
    {
        [SerializeField]
        public PartialInvestmentData partialInvestmentData;

        [SerializeField]
        public RealEstateData realEstateData;
    }

    [Serializable]
    public class PortfolioData
    {
        public int cash;
        public bool hasHealthInsurance;

        // Custom Serialization
        public int studentLoan;
        public int personalLoan;
        public int carValue;
        public int autoLoan;

        public List<PurchasedStockData> stocks;
        public List<InvestmentContextData> rentalProperties;
        public List<InvestmentContextData> distressedProperties;

        /*
        public List<RentalProperty> rentalProperties { get; private set; }
        public List<DistressedProperty> distressedProperties { get; private set; }
        public List<BusinessEntity> businessEntities { get; private set; }
        public List<StartupEntity> startupEntities { get; private set; }
        public List<AbstractTimedInvestment> timedInvestments { get; private set; }
        public List<LuxuryItem> luxuryItems { get; private set; }
        */


        public void Initialize(Profession profession)
        {
            cash = profession.startingCash;
            hasHealthInsurance = false;

            studentLoan = profession.jobCost;
            personalLoan = 0;
            carValue = profession.autoLoan;
            autoLoan = profession.autoLoan;

            stocks = new List<PurchasedStockData>();
            rentalProperties = new List<InvestmentContextData>();
            distressedProperties = new List<InvestmentContextData>();
        }
    }

    public class Portfolio
    {
        private PortfolioData _data;

        public int cash
        {
            get => _data.cash;
            set { _data.cash = value; }
        }

        public bool hasHealthInsurance
        {
            get => _data.hasHealthInsurance;
            set { _data.hasHealthInsurance = value; }
        }

        public StudentLoan studentLoan { get; private set; }
        public PersonalLoan personalLoan { get; private set; }
        public Car car { get; private set; }
        public AutoLoan autoLoan { get; private set; }

        public Dictionary<string, PurchasedStock> stocks { get; private set; }
        public List<RentalProperty> rentalProperties { get; private set; }
        public List<DistressedProperty> distressedProperties { get; private set; }
        public List<BusinessEntity> businessEntities { get; private set; }
        public List<StartupEntity> startupEntities { get; private set; }
        public List<AbstractTimedInvestment> timedInvestments { get; private set; }
        public List<LuxuryItem> luxuryItems { get; private set; }

        public void SaveData()
        {
            _data.studentLoan = studentLoan != null ? studentLoan.amount : 0;
            _data.personalLoan = personalLoan != null ? personalLoan.amount : 0;
            _data.carValue = car != null ? car.value : 0;
            _data.autoLoan = autoLoan != null ? autoLoan.amount : 0;

            _data.stocks.Clear();
            foreach (PurchasedStock stock in stocks.Values)
            {
                _data.stocks.Add(stock.SaveToStockData());
            }

            _data.rentalProperties.Clear();
            foreach (RentalProperty rentalProperty in rentalProperties)
            {
                InvestmentContextData investmentData = new InvestmentContextData();
                investmentData.partialInvestmentData = rentalProperty.Item1.data;
                investmentData.realEstateData = rentalProperty.Item2.realEstateData;
                _data.rentalProperties.Add(investmentData);
            }

            _data.distressedProperties.Clear();
            foreach (DistressedProperty distressedProperty in distressedProperties)
            {
                InvestmentContextData investmentData = new InvestmentContextData();
                investmentData.partialInvestmentData = distressedProperty.Item1.data;
                investmentData.realEstateData = distressedProperty.Item2.realEstateData;
                _data.distressedProperties.Add(investmentData);
            }
        }

        public void LoadData()
        {
            studentLoan = _data.studentLoan > 0 ? new StudentLoan(_data.studentLoan) : null;
            personalLoan = _data.personalLoan > 0 ? new PersonalLoan(_data.personalLoan) : null;
            car = _data.carValue > 0 ? new Car(_data.carValue) : null;
            autoLoan = _data.autoLoan > 0 ? new AutoLoan(_data.autoLoan) : null;

            stocks.Clear();
            foreach (PurchasedStockData data in _data.stocks)
            {
                stocks[data.name] = new PurchasedStock(
                    StockManager.Instance.GetStockByName(data.name, true), data.count);
            }
        }


        public List<PartialInvestment> properties {
            get
            {
                List<PartialInvestment> properties = new List<PartialInvestment>();
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
        public List<PartialInvestment> businesses
        {
            get
            {
                List<PartialInvestment> businesses = new List<PartialInvestment>();
                foreach (BusinessEntity entity in businessEntities)
                {
                    businesses.Add(entity.Item1);
                }
                foreach (StartupEntity entity in startupEntities)
                {
                    businesses.Add(entity.Item1);
                }
                return businesses;
            }
        }

/*        public List<PurchasedStock> purchasedStocks
        {
            get
            {
                List<PurchasedStock> purchasedStocks = new List<PurchasedStock>();
                stocks.AddRange(stocks
            }

        }
        */
        public List<AbstractAsset> liquidAssets
        {
            get
            {
                List<AbstractAsset> assets = new List<AbstractAsset>();
                foreach (KeyValuePair<string, PurchasedStock> entry in stocks)
                {
                    assets.Add(entry.Value);
                }
                return assets;
            }
        }


        public List<AbstractAsset> managedAssets
        {
            get
            {
                List<AbstractAsset> assets = new List<AbstractAsset>();
                assets.AddRange(properties);
                assets.AddRange(businesses);
                return assets;
            }
        }
/*
        public List<AbstractAsset> assets
        {
            get
            {
                List<AbstractAsset> assets = new List<AbstractAsset>();
                assets.AddRange(otherAssets);
                assets.AddRange(liquidAssets);
                assets.AddRange(properties);
                assets.AddRange(businesses);
                assets.AddRange(timedInvestments);
                return assets;
            }
        }
*/
        public List<AssetTypeEntity> assetsByType 
        {
            get
            {
                List<AssetTypeEntity> results = new List<AssetTypeEntity>()
                {
                    new AssetTypeEntity(
                        "Real Estate", properties.ConvertAll(a => (AbstractAsset)a)),
                    new AssetTypeEntity(
                        "Business", businesses.ConvertAll(a => (AbstractAsset)a)),
                    new AssetTypeEntity(
                        "Liquid Assets", liquidAssets.ConvertAll(a => (AbstractAsset)a)),
                    new AssetTypeEntity(
                        "Timed Investments", timedInvestments.ConvertAll(a => (AbstractAsset)a)),
                    new AssetTypeEntity(
                        "Other Assets", otherAssets.ConvertAll(a => (AbstractAsset)a)),
                };
                return results;
            }
        }

        public List<AbstractAsset> otherAssets
        {
            get
            {
                List<AbstractAsset> otherAssets = new List<AbstractAsset>();
                otherAssets.AddRange(luxuryItems);
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
                if (autoLoan != null && autoLoan.amount > 0)
                {
                    ret.Add(autoLoan);
                }
                return ret;
            }
        }


        public Portfolio(PortfolioData data)
        {
            _data = data;
            stocks = new Dictionary<string, PurchasedStock>();
            rentalProperties = new List<RentalProperty>();
            distressedProperties = new List<DistressedProperty>();
            businessEntities = new List<BusinessEntity>();
            startupEntities = new List<StartupEntity>();
            timedInvestments = new List<AbstractTimedInvestment>();
            luxuryItems = new List<LuxuryItem>();
            hasHealthInsurance = false;
        }

        public void AddPersonalLoan(int amount)
        {
            EventLogManager.Instance.LogFormat(
                "Add Personal Loan: {0}",
                Localization.Instance.GetCurrency(amount, true));
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
            EventLogManager.Instance.LogFormat(
                "Add Cash: {0}",
                Localization.Instance.GetCurrency(amount));
            _data.cash += amount;
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

        public List<StartupEntity> RemoveExitedStartups()
        {
            List<StartupEntity> exitedStartups = startupEntities.FindAll(
                e => e.Item2.exited);
            startupEntities = startupEntities.FindAll(e => !e.Item2.exited);
            return exitedStartups;
        }

        public void OnTurnStart(System.Random random)
        {
            List<string> stocksDelisted = new List<string>();
            foreach (string name in stocks.Keys) 
            {
                AbstractStock stock = StockManager.Instance.GetStockByName(name);
                if (stock == null || stock.value == 0)
                {
                    stocksDelisted.Add(name);
                }
            }
            stocksDelisted.ForEach(s => stocks.Remove(s));

//            assets.ForEach(a => a.OnTurnStart(random));
        }

        public bool hasHighRiskInvestments
        {
            get
            {
                if (distressedProperties.Count > 0 ||
                    startupEntities.Count > 0 ||
                    timedInvestments.Count > 0)
                {
                    return true;
                }
                foreach (KeyValuePair<string, PurchasedStock> kv in stocks)
                {
                    if (kv.Value.stock is AbstractCryptoCurrency)
                    {
                        return true;
                    }
                }
                return false;
            }
        }
    }
}
