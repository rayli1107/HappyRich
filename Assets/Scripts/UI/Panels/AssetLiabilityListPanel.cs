using Assets;
using System.Collections;
using System.Collections.Generic;
using UI.Panels.Templates;
using UnityEngine;
using UnityEngine.UI;

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
        private ItemValuePanel _panelAssets;
        [SerializeField]
        private ItemValuePanel _panelLiabilities;
        /*
         *[SerializeField]
                private ItemListPanel _panelAssets;
                [SerializeField]
                private ItemListPanel _panelStocks;
                [SerializeField]
                private ItemListPanel _panelOtherAssets;
                [SerializeField]
                private ItemListPanel _panelLiabilities;
                [SerializeField]
                private ItemListPanel _panelOtherLiabilities;
                */

        [SerializeField]
        private ItemValuePanel _prefabItemValuePanel;
#pragma warning restore 0649

        public Player player;

        private void Awake()
        {
        }

        private IEnumerator DelayedRefresh()
        {
            GameObject content = GetComponentInChildren<ScrollRect>().content.gameObject;
            yield return new WaitForSeconds(0.5f);
            content.SetActive(true);
        }

        private int AddItemValueAsCurrency(
            Transform parentTranform,
            int index,
            int tab,
            string label,
            int value,
            bool flip)
        {
            ItemValuePanel panel = Instantiate(_prefabItemValuePanel, parentTranform);
            panel.colorFlip = flip;
            panel.setLabel(label);
            panel.setValueAsCurrency(value);
            panel.setTabCount(tab);
            panel.transform.SetSiblingIndex(index);
            return index + 1;
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

            int currentIndex = _panelAssets.transform.GetSiblingIndex() + 1;

            // Stocks
            if (player.portfolio.stocks.Count > 0)
            {
                foreach (KeyValuePair<string, PurchasedStock> entry in player.portfolio.stocks)
                {
                    totalStocks += entry.Value.getValue();
                }

                currentIndex = AddItemValueAsCurrency(
                    _panelAssets.transform.parent,
                    currentIndex,
                    _panelAssets.tabCount + 1,
                    "Stocks",
                    totalStocks,
                    false);

                foreach (KeyValuePair<string, PurchasedStock> entry in player.portfolio.stocks)
                {
                    PurchasedStock stock = entry.Value;
                    currentIndex = AddItemValueAsCurrency(
                        _panelAssets.transform.parent,
                        currentIndex,
                        _panelAssets.tabCount + 2,
                        stock.stock.name,
                        stock.getValue(),
                        false);
                }
            }

            // Other Assets
            if (player.portfolio.otherAssets.Count > 0)
            {
                foreach (AbstractAsset asset in player.portfolio.otherAssets)
                {
                    totalOtherAssets += asset.getValue();
                    if (asset.liability != null && asset.liability.amount > 0)
                    {
                        liabilities.Add(asset.liability);
                    }
                }

                currentIndex = AddItemValueAsCurrency(
                    _panelAssets.transform.parent,
                    currentIndex,
                    _panelAssets.tabCount + 1,
                    "Other Assets",
                    totalOtherAssets,
                    false);

                foreach (AbstractAsset asset in player.portfolio.otherAssets)
                {
                    currentIndex = AddItemValueAsCurrency(
                        _panelAssets.transform.parent,
                        currentIndex,
                        _panelAssets.tabCount + 2,
                        asset.name,
                        asset.getValue(),
                        false);
                }
            }

            // Other Liabilities
            currentIndex = _panelLiabilities.transform.GetSiblingIndex() + 1;
            foreach (AbstractLiability liability in liabilities)
            {
                totalLiabilities += liability.amount;

                currentIndex = AddItemValueAsCurrency(
                    _panelLiabilities.transform.parent,
                    currentIndex,
                    _panelLiabilities.tabCount + 1,
                    liability.name,
                    liability.amount,
                    true);
            }

            totalAssets = totalStocks + totalOtherAssets;
            int netWorth = player.cash + totalAssets - totalLiabilities;

            _panelAssets.setValueAsCurrency(totalAssets);
            _panelLiabilities.setValueAsCurrency(totalLiabilities);
            _panelNetWorth.setValueAsCurrency(netWorth);
            _panelAvailableCash.setValueAsCurrency(player.cash);
        }

        private void OnEnable()
        {
            RefreshContent();
        }
    }
}
