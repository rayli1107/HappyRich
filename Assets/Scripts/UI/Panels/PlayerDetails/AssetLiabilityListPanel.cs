using Assets;
using System.Collections;
using System.Collections.Generic;
using UI.Panels.Templates;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Panels.PlayerDetails
{
    public class AssetLiabilityListPanel : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField]
        private ItemValuePanel _panelNetWorth;
        [SerializeField]
        private ItemValuePanel _panelAssets;
        [SerializeField]
        private ItemValuePanel _panelLiabilities;
        [SerializeField]
        private ItemValuePanel _prefabItemValuePanel;
        [SerializeField]
        private bool _showTotalValues = false;
#pragma warning restore 0649

        public Player player;

        private void Awake()
        {
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
            panel.setLabel(label);
            if (value != 0)
            {
                panel.setValueAsCurrency(value, flip);
            }
            else
            {
                panel.removeValue();
            }
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

            // Cash
            currentIndex = AddItemValueAsCurrency(
                _panelAssets.transform.parent,
                currentIndex,
                _panelAssets.tabCount + 1,
                "Cash",
                player.cash,
                false);
            totalAssets = player.cash;

            // Stocks
            if (player.portfolio.stocks.Count > 0)
            {
                foreach (KeyValuePair<string, PurchasedStock> entry in player.portfolio.stocks)
                {
                    totalStocks += entry.Value.value;
                }

                currentIndex = AddItemValueAsCurrency(
                    _panelAssets.transform.parent,
                    currentIndex,
                    _panelAssets.tabCount + 1,
                    "Stocks",
                    0,
                    false);

                foreach (KeyValuePair<string, PurchasedStock> entry in player.portfolio.stocks)
                {
                    PurchasedStock stock = entry.Value;
                    currentIndex = AddItemValueAsCurrency(
                        _panelAssets.transform.parent,
                        currentIndex,
                        _panelAssets.tabCount + 2,
                        stock.stock.name,
                        stock.value,
                        false);
                }
            }
            totalAssets += totalStocks;

            // Other Assets
            if (player.portfolio.otherAssets.Count > 0)
            {
                foreach (AbstractAsset asset in player.portfolio.otherAssets)
                {
                    totalOtherAssets += asset.value;
                    foreach (AbstractLiability liability in asset.liabilities) 
                    {
                        if (liability.amount > 0)
                        {
                            liabilities.Add(liability);
                        }
                    }
                }

                currentIndex = AddItemValueAsCurrency(
                    _panelAssets.transform.parent,
                    currentIndex,
                    _panelAssets.tabCount + 1,
                    "Other Assets",
                    0,
                    false);

                foreach (AbstractAsset asset in player.portfolio.otherAssets)
                {
                    currentIndex = AddItemValueAsCurrency(
                        _panelAssets.transform.parent,
                        currentIndex,
                        _panelAssets.tabCount + 2,
                        asset.name,
                        asset.value,
                        false);
                }
            }
            totalAssets += totalOtherAssets;

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

            int netWorth = totalAssets - totalLiabilities;

            if (_showTotalValues)
            {
                _panelAssets.setValueAsCurrency(totalAssets);
                _panelLiabilities.setValueAsCurrency(totalLiabilities, true);
            }
            _panelNetWorth.setValueAsCurrency(netWorth);
        }

        private void OnEnable()
        {
            RefreshContent();
        }
    }
}
