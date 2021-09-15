using Assets;
using PlayerInfo;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Panels.Assets
{
    public class StockMarketPanel : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField]
        private StockListPanel _prefabStockListPanel;
#pragma warning restore 0649

        public Player player;

        private void AddStocks(Transform parentTransform, List<AbstractStock> stocks, string title)
        {
            if (stocks.Count > 0)
            {
                StockListPanel stockListPanel = Instantiate(_prefabStockListPanel, parentTransform);
                stockListPanel.player = player;
                stockListPanel.stocks = stocks;
                stockListPanel.title = title;
                stockListPanel.gameObject.SetActive(true);
                stockListPanel.Refresh();
                stockListPanel.transform.SetSiblingIndex(
                    stockListPanel.transform.GetSiblingIndex() - 1);
            }
        }

        public void Refresh()
        {
            if (player == null)
            {
                return;
            }

            Transform parentTransform = GetComponentInChildren<VerticalLayoutGroup>().transform;

            StockManager stockManager = StockManager.Instance;
            AddStocks(parentTransform, stockManager.growthStocks, "Growth Stocks");
            AddStocks(parentTransform, stockManager.yieldStocks, "Dividend Stocks");
            AddStocks(parentTransform, stockManager.cryptoCurrencies, "Cryptocurrencies");
        }

        private void OnEnable()
        {
            Refresh();
        }
    }
}