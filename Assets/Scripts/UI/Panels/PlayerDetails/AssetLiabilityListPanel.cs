using Actions;
using Assets;
using PlayerInfo;
using System;
using System.Collections.Generic;
using UI.Panels.Templates;
using UnityEngine;

using AssetContext = System.Tuple<
    Assets.AbstractAsset, Assets.AbstractLiability>;

namespace UI.Panels.PlayerDetails
{
    public class AssetLiabilityListPanel : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField]
        private ItemValuePanel _panelNetWorth;
        [SerializeField]
        private ItemValuePanel _panelCash;
        [SerializeField]
        private ItemValueListPanel _panelAssets;
        [SerializeField]
        private ItemValueListPanel _panelLiabilities;
        [SerializeField]
        private bool _showTotalValues = false;
#pragma warning restore 0649

        public Player player;

        private void addAssetLiabilityByType(
            string assetType,
            List<AbstractAsset> assets,
            List<AssetContext> liabilities,
            ref int totalAssetValue,
            ref int totalLiabilityValue)
        {
            Localization local = Localization.Instance;
            int totalAssetValueByType = 0;
            int totalLiabilityValueByType = 0;

            if (liabilities == null)
            {
                liabilities = new List<AssetContext>();
            }

            if (assets != null && assets.Count > 0)
            {
                int tabCount = _panelAssets.firstItemValuePanel.tabCount + 1;
                _panelAssets.AddItem(assetType, tabCount);

                foreach (AbstractAsset asset in assets)
                {
                    totalAssetValueByType += asset.value;
                    if (asset.combinedLiability.amount > 0)
                    {
                        liabilities.Add(new AssetContext(asset, asset.combinedLiability));
                    }

                    ItemValuePanel panel = _panelAssets.AddItemValue(
                        asset.name,
                        tabCount + 1,
                        local.GetCurrency(asset.value));
                    panel.clickAction = () => asset.OnDetail(player, reloadWindow);
                }
            }

            if (liabilities.Count > 0)
            {
                int tabCount = _panelLiabilities.firstItemValuePanel.tabCount + 1;
                _panelLiabilities.AddItem(assetType, tabCount);
                foreach (AssetContext context in liabilities)
                {
                    int amount = context.Item2.amount;
                    totalLiabilityValueByType += amount;
                    ItemValuePanel panel = _panelLiabilities.AddItemValue(
                        context.Item2.longName,
                        tabCount + 1,
                        local.GetCurrency(amount, true));
                    if (context.Item1 != null)
                    {
                        panel.clickAction = () => context.Item1.OnDetail(player, reloadWindow);
                    }
                    else
                    {
                        panel.clickAction = () => context.Item2.OnDetail(player, reloadWindow);
                    }
                }
            }

            totalAssetValue += totalAssetValueByType;
            totalLiabilityValue += totalLiabilityValueByType;
        }

        public void RefreshContent()
        {
            if (player == null)
            {
                return;
            }

            Localization local = Localization.Instance;
            int totalAssets = 0;
            int totalLiabilities = 0;

            // Cash
            _panelCash.SetValue(local.GetCurrency(player.cash));

            // Stocks
            List<AbstractAsset> stocks = new List<AbstractAsset>();
            foreach (KeyValuePair<string, PurchasedStock> entry in player.portfolio.stocks)
            {
                stocks.Add(entry.Value);
            }
            addAssetLiabilityByType(
                "Liquid Assets",
                stocks,
                null,
                ref totalAssets,
                ref totalLiabilities);

            // Real Estate
            addAssetLiabilityByType(
                "Real Estate",
                player.portfolio.properties.ConvertAll(a => (AbstractAsset)a),
                null,
                ref totalAssets,
                ref totalLiabilities);

            // Business
            addAssetLiabilityByType(
                "Business",
                player.portfolio.businesses.ConvertAll(a => (AbstractAsset)a),
                null,
                ref totalAssets,
                ref totalLiabilities);

            // Other Assets & Liabilities
            addAssetLiabilityByType(
                "Other Assets & Liabilities",
                player.portfolio.otherAssets,
                player.portfolio.liabilities.ConvertAll(l => new AssetContext(null, l)),
                ref totalAssets,
                ref totalLiabilities);

            int netWorth = totalAssets - totalLiabilities;

            if (_showTotalValues)
            {
                _panelAssets.firstItemValuePanel.SetValue(
                    local.GetCurrency(totalAssets));
                _panelLiabilities.firstItemValuePanel.SetValue(
                    local.GetCurrency(totalLiabilities, true));
            }
            _panelNetWorth.SetValue(local.GetCurrency(netWorth));
        }

        private void OnEnable()
        {
            RefreshContent();
        }

        private void reloadWindow()
        {
            GetComponent<MessageBox>().Destroy();
            UIManager.Instance.ShowAssetLiabilityStatusPanel();
        }
    }
}
