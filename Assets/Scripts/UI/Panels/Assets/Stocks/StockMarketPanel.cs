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
        private GrowthStockComponent _prefabGrowthStockPanel;
        [SerializeField]
        private YieldStockComponent _prefabYieldStockPanel;
        [SerializeField]
        private CryptoComponent _prefabCryptoPanel;
#pragma warning restore 0649

        public Player player;

        private void setupStockPanel(StockPanel panel)
        {
            panel.player = player;
            panel.tradeCallback = Refresh;
            panel.gameObject.SetActive(true);
            panel.Refresh();
        }

        public void Refresh()
        {
            if (player == null)
            {
                return;
            }

            StockManager stockManager = StockManager.Instance;

            // Growth Stocks
            _panelGrowthStocks.Clear();
            foreach (GrowthStock stock in stockManager.growthStocks)
            {
                GrowthStockComponent panel = Instantiate(
                    _prefabGrowthStockPanel,
                    _panelGrowthStocks.transform);
                panel.growthStock = stock;
                setupStockPanel(panel);
            }
            _panelGrowthStocks.ActivateIfNonEmpty();

            // Yield Stocks
            _panelYieldStocks.Clear();
            foreach (YieldStock stock in stockManager.yieldStocks)
            {
                YieldStockComponent panel = Instantiate(
                    _prefabYieldStockPanel,
                    _panelYieldStocks.transform);
                panel.yieldStock = stock;
                setupStockPanel(panel);
            }
            _panelYieldStocks.ActivateIfNonEmpty();

            // Crypto
            _panelCryptos.Clear();
            foreach (AbstractCryptoCurrency stock in stockManager.cryptoCurrencies)
            {
                CryptoComponent panel = Instantiate(
                    _prefabCryptoPanel,
                    _panelCryptos.transform);
                panel.crypto = stock;
                setupStockPanel(panel);
            }
            _panelCryptos.ActivateIfNonEmpty();
        }

        private void OnEnable()
        {
            Refresh();
        }
    }
}