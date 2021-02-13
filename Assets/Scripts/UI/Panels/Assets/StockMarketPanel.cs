using Assets;
using PlayerInfo;
using System.Collections.Generic;
using UnityEngine;

namespace UI.Panels.Assets
{
    public class StockMarketPanel : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField]
        private StockPanel _prefabStockPanel;
        [SerializeField]
        private Transform _content;
#pragma warning restore 0649

        public Player player;

        private void SetupStockPanel(List<AbstractStock> stocks)
        {
            foreach (AbstractStock stock in stocks)
            {
                StockPanel childPanel = Instantiate(
                    _prefabStockPanel, _content);
                childPanel.player = player;
                childPanel.stock = stock;
                childPanel.Refresh();
            }
        }

        public void Refresh()
        {
            if (player == null)
            {
                return;
            }

            StockManager stockManager = StockManager.Instance;
            if (stockManager.growthStocks.Count > 0)
            {
                SetupStockPanel(stockManager.growthStocks);
            }
        }

        private void OnEnable()
        {
            Refresh();
        }
    }
}