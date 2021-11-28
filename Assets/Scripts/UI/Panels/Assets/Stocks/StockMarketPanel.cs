using Assets;
using PlayerInfo;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Panels.Assets
{
    public class StockMarketPanel : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField]
        private StockListPanel _stockListGrowth;
        [SerializeField]
        private StockListPanel _stockListYield;
        [SerializeField]
        private StockListPanel _stockListCrypto;
        [SerializeField]
        private StockPanel _prefabGrowthStockPanel;
        [SerializeField]
        private StockPanel _prefabYieldStockPanel;
        [SerializeField]
        private StockPanel _prefabCryptoPanel;
#pragma warning restore 0649

        public Player player;

        private void registerGrowthStock(
            StockPanel panel,
            GrowthStock stock)
        {
            Debug.LogFormat("Growth Stock {0}", stock.name);
            panel.stock = stock;
            GrowthStockComponent component = panel.GetComponent<GrowthStockComponent>();
            component.growthStock = stock;
            component.player = player;
        }

        private void registerYieldStock(
            StockPanel panel,
            YieldStock stock)
        {
            Debug.LogFormat("Yield Stock {0}", stock.name);
            panel.stock = stock;
            YieldStockComponent component = panel.GetComponent<YieldStockComponent>();
            component.yieldStock = stock;
            component.player = player;
        }

        private void registerCrypto(
            StockPanel panel,
            AbstractCryptoCurrency stock)
        {
            Debug.LogFormat("Crypto {0}", stock.name);
            panel.stock = stock;
            CryptoComponent component = panel.GetComponent<CryptoComponent>();
            component.crypto = stock;
            component.player = player;
        }

        private void AddStockList<StockType>(
            StockListPanel stockListPanel,
            StockPanel prefab,
            List<StockType> stocks,
            Action<StockPanel, StockType> registerFn)
        {
            stockListPanel.gameObject.SetActive(stocks.Count > 0);
            foreach (StockType stock in stocks)
            {
                StockPanel panel = Instantiate(prefab, stockListPanel.transform);
                panel.player = player;
                registerFn(panel, stock);
                panel.gameObject.SetActive(true);
            }
        }
/*

        private void AddGrowthStocks(List<GrowthStock> stocks)
        {
            _stockListGrowth.gameObject.SetActive(stocks.Count > 0);
            foreach (GrowthStock stock in stocks)
            {
                GrowthStockComponent component = Instantiate(
                    _prefabGrowthStockPanel, _stockListGrowth.transform);
                component.player = player;
                component.growthStock = stock;
                StockPanel panel = component.GetComponent<StockPanel>();
                panel.player = player;
                panel.stock = stock;
                component.gameObject.SetActive(true);
            }
        }

        private void AddYieldStocks(List<YieldStock> stocks)
        {
            _stockListYield.gameObject.SetActive(stocks.Count > 0);
            foreach (YieldStock stock in stocks)
            {
                YieldStockComponent component = Instantiate(
                    _prefabYieldStockPanel, _stockListYield.transform);
                component.player = player;
                component.yieldStock = stock;
                StockPanel panel = component.GetComponent<StockPanel>();
                panel.player = player;
                panel.stock = stock;
                component.gameObject.SetActive(true);
            }
        }

        private void AddCryptos(List<AbstractCryptoCurrency> stocks)
        {
            _stockListCrypto.gameObject.SetActive(stocks.Count > 0);
            foreach (AbstractCryptoCurrency stock in stocks)
            {
                CryptoComponent component = Instantiate(
                    _prefabCryptoPanel, _stockListCrypto.transform);
                component.player = player;
                component.crypto = stock;
                StockPanel panel = component.GetComponent<StockPanel>();
                panel.player = player;
                panel.stock = stock;
                component.gameObject.SetActive(true);
            }
        }
        */

        public void Refresh()
        {
            if (player == null)
            {
                return;
            }

            StockManager stockManager = StockManager.Instance;

            AddStockList(
                _stockListGrowth,
                _prefabGrowthStockPanel,
                stockManager.growthStocks,
                registerGrowthStock);

            AddStockList(
                _stockListYield,
                _prefabYieldStockPanel,
                stockManager.yieldStocks,
                registerYieldStock);

            AddStockList(
                _stockListCrypto,
                _prefabCryptoPanel,
                stockManager.cryptoCurrencies,
                registerCrypto);
        }

        private void OnEnable()
        {
            Refresh();
        }
    }
}