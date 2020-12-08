using Assets;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Panels.Assets
{
    public class StockMarketPanel : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField]
        private StockPanel _prefabStockPanel;
        [SerializeField]
        private GameObject _growthStockPanel;
        [SerializeField]
        private GameObject _yieldStockPanel;
#pragma warning restore 0649

        public Player player;

        private void SetupStockPanel(GameObject panelObject, List<AbstractStock> stocks)
        {
            panelObject.SetActive(true);
            foreach (AbstractStock stock in stocks)
            {
                StockPanel childPanel = Instantiate(
                    _prefabStockPanel, panelObject.transform);
                childPanel.player = player;
                childPanel.stock = stock;
                childPanel.gameObject.SetActive(false);
                childPanel.gameObject.SetActive(true);
            }
        }

        private void OnEnable()
        {
            StockManager stockManager = StockManager.Instance;
            if (stockManager.growthStocks.Count > 0)
            {
                SetupStockPanel(_growthStockPanel, stockManager.growthStocks);
            }
        }
    }
}