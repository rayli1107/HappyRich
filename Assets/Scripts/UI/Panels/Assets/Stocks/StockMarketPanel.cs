using Assets;
using PlayerInfo;
using System;
using System.Collections.Generic;
using UI.Panels.Templates;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Panels.Assets
{
    public class StockMarketPanel : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField]
        private ItemValueListPanel _panelGrowthStocks;
        [SerializeField]
        private ItemValueListPanel _panelYieldStocks;
        [SerializeField]
        private ItemValueListPanel _panelCryptos;
        [SerializeField]
        private StockPanel _prefabStockPanel;
#pragma warning restore 0649

        public Player player;

/*
 * private void registerGrowthStock(
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
*/
        private void AddStockList(
            ItemValueListPanel stockListPanel,
            List<AbstractStock> stocks)
//            Action<StockPanel, StockType> registerFn)
        {
            stockListPanel.Clear();
            stockListPanel.gameObject.SetActive(stocks.Count > 0);
            foreach (AbstractStock stock in stocks)
            {
                StockPanel panel = Instantiate(_prefabStockPanel, stockListPanel.transform);
                panel.player = player;
                panel.stock = stock;
//                registerFn(panel, stock);
                panel.gameObject.SetActive(true);
                panel.buttonTrade.onClick.AddListener(
                    new UnityEngine.Events.UnityAction(
                        () => stock.OnDetail(Refresh)));
                panel.Refresh();
            }

            stockListPanel.gameObject.SetActive(stockListPanel.itemCount > 0);
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
                _panelGrowthStocks,
                stockManager.growthStocks.ConvertAll(s => (AbstractStock)s));

            AddStockList(
                _panelYieldStocks,
                stockManager.yieldStocks.ConvertAll(s => (AbstractStock)s));

            AddStockList(
                _panelCryptos,
                stockManager.cryptoCurrencies.ConvertAll(s => (AbstractStock)s));
        }

        private void OnEnable()
        {
            Refresh();
        }
    }
}