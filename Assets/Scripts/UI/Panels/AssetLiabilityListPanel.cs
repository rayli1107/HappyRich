using Assets;
using System.Collections.Generic;
using TMPro;
using UI.Panels.Assets;
using UI.Panels.Templates;
using UnityEngine;

namespace UI.Panels
{
    public class AssetLiabilityListPanel : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField]
        private ItemValuePanel _panelNetWorth;
        [SerializeField]
        private ItemValuePanel _panelAvailableCash;
        [SerializeField]
        private ItemListPanel _panelStocks;
        [SerializeField]
        private ItemListPanel _panelOtherAssets;
        [SerializeField]
        private ItemListPanel _panelOtherLiabilities;

        [SerializeField]
        private ItemValuePanel _prefabItemValuePanel;
#pragma warning restore 0649

        public Player player;

        private void Awake()
        {
        }

        public void RefreshContent()
        {
            if (player == null)
            {
                return;
            }

            List<AbstractLiability> liabilities = new List<AbstractLiability>();
            liabilities.AddRange(player.portfolio.liabilities);

            int totalAssets = 0;
            int totalLiabilities = 0;
            int totalStocks = 0;
            int totalOtherAssets = 0;

            foreach (KeyValuePair<string, PurchasedStock> entry in player.portfolio.stocks)
            {
                PurchasedStock stock = entry.Value;
                int value = stock.getValue();
                totalStocks += value;

                ItemValuePanel panel = Instantiate(
                    _prefabItemValuePanel, _panelStocks.content.transform);
                panel.setLabel(stock.stock.name);
                panel.setValueAsCurrency(stock.getValue());
            }
            if (_panelStocks.content.transform.childCount == 0)
            {
                _panelStocks.gameObject.SetActive(false);
            }

            foreach (AbstractAsset asset in player.portfolio.otherAssets)
            {
                int value = asset.getValue();
                totalOtherAssets = value;
                if (asset.liability != null && asset.liability.amount > 0)
                {
                    liabilities.Add(asset.liability);
                }

                ItemValuePanel panel = Instantiate(
                    _prefabItemValuePanel, _panelOtherAssets.content.transform);
                panel.setLabel(asset.name);
                panel.setValueAsCurrency(value);
            }
            if (_panelOtherAssets.content.transform.childCount == 0)
            {
                _panelOtherAssets.gameObject.SetActive(false);
            }

            foreach (AbstractLiability liability in liabilities)
            {
                totalLiabilities += liability.amount;

                ItemValuePanel panel = Instantiate(
                    _prefabItemValuePanel, _panelOtherLiabilities.content.transform);
                panel.colorFlip = true;
                panel.setLabel(liability.name);
                panel.setValueAsCurrency(liability.amount);
            }

            if (_panelOtherLiabilities.content.transform.childCount == 0)
            {
                _panelOtherLiabilities.gameObject.SetActive(false);
            }

            totalAssets = totalStocks + totalOtherAssets;
            int netWorth = player.cash + totalAssets - totalLiabilities;

            Localization local = GameManager.Instance.Localization;
            _panelNetWorth.setValueAsCurrency(netWorth);
            _panelAvailableCash.setValueAsCurrency(player.cash);
        }

        private void OnEnable()
        {
            RefreshContent();
        }
    }
}
