using Assets;
using PlayerInfo;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace UI.Panels.Assets
{
    public class StockListPanel : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField]
        private StockPanel _prefabStockPanel;
        [SerializeField]
        private Transform _content;
        [SerializeField]
        private TextMeshProUGUI _title;
#pragma warning restore 0649

        public Player player;
        public List<AbstractStock> stocks;
        public string title { get { return _title.text; } set { _title.text = value; }}

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
            Debug.LogFormat("Stocks count {0}", stocks != null ? stocks.Count : 0);
            if (player != null && stocks != null && stocks.Count > 0)
            {
                SetupStockPanel(stocks);
            }
        }

        private void OnEnable()
        {
            Refresh();
        }
    }
}