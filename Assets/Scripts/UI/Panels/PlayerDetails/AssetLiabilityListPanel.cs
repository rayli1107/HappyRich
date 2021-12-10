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

        private void showAssetLiabilityDetails(
            AbstractAsset asset, AbstractLiability loan, int fontSizeMax = 32)
        {
            List<string> details =
                asset == null ? loan.GetDetails() : asset.GetDetails();
            SimpleTextMessageBox panel = UIManager.Instance.ShowSimpleMessageBox(
                string.Join("\n", details), ButtonChoiceType.OK_ONLY, null);
            panel.text.fontSizeMax = fontSizeMax;
        }

        private Action getClickAction(
            AbstractAsset asset, AbstractLiability loan)
        {
            if (loan != null && loan.payable && loan.amount > 0)
            {
                return () => LoanPayoffActions.PayAssetLoanPrincipal(
                    player, asset, loan, reloadWindow);
            }
            else
            {
                return () => showAssetLiabilityDetails(asset, loan);
            }
        }

        private int AddItemValueAsCurrency(
            Transform parentTranform,
            int index,
            int tab,
            string label,
            int value,
            bool flip,
            Action clickAction = null)
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
            panel.clickAction = clickAction;
            return index + 1;
        }

        public void RefreshContent()
        {
            if (player == null)
            {
                return;
            }

            List<AssetContext> liabilities = new List<AssetContext>();
            foreach (AbstractLiability liability in player.portfolio.liabilities)
            {
                liabilities.Add(new AssetContext(null, liability));
            }

            int totalAssets = 0;
            int totalBusiness = 0;
            int totalRealEstate = 0;
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

            // Real Estate
            if (player.portfolio.properties.Count > 0)
            {
                foreach (PartialInvestment asset in player.portfolio.properties)
                {
                    totalRealEstate += asset.value;
                    if (asset.combinedLiability.amount > 0)
                    {
                        liabilities.Add(
                            new AssetContext(asset, asset.combinedLiability));
                    }
                }

                currentIndex = AddItemValueAsCurrency(
                    _panelAssets.transform.parent,
                    currentIndex,
                    _panelAssets.tabCount + 1,
                    "Real Estate",
                    0,
                    false);

                foreach (PartialInvestment asset in player.portfolio.properties)
                {
                    currentIndex = AddItemValueAsCurrency(
                        _panelAssets.transform.parent,
                        currentIndex,
                        _panelAssets.tabCount + 2,
                        asset.name,
                        asset.value,
                        false,
                        getClickAction(asset, asset.combinedLiability));
                }
            }
            totalAssets += totalRealEstate;

            // Business
            if (player.portfolio.businesses.Count > 0)
            {
                currentIndex = AddItemValueAsCurrency(
                    _panelAssets.transform.parent,
                    currentIndex,
                    _panelAssets.tabCount + 1,
                    "Business",
                    0,
                    false);

                foreach (PartialInvestment asset in player.portfolio.businesses)
                {
                    totalBusiness += asset.value;
                    if (asset.combinedLiability.amount > 0)
                    {
                        liabilities.Add(
                            new AssetContext(asset, asset.combinedLiability));
                    }

                    currentIndex = AddItemValueAsCurrency(
                        _panelAssets.transform.parent,
                        currentIndex,
                        _panelAssets.tabCount + 2,
                        asset.name,
                        asset.value,
                        false,
                        getClickAction(asset, asset.combinedLiability));
                }
            }
            totalAssets += totalBusiness;

            // Other Assets
            if (player.portfolio.otherAssets.Count > 0)
            {
                foreach (AbstractAsset asset in player.portfolio.otherAssets)
                {
                    totalOtherAssets += asset.value;
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
                    AbstractLiability loan = asset.combinedLiability;
                    currentIndex = AddItemValueAsCurrency(
                        _panelAssets.transform.parent,
                        currentIndex,
                        _panelAssets.tabCount + 2,
                        asset.name,
                        asset.value,
                        false,
                        getClickAction(asset, loan));

                    if (loan.amount > 0)
                    {
                        liabilities.Add(new AssetContext(asset, loan));
                    }
                }
            }
            totalAssets += totalOtherAssets;

            // Other Liabilities
            currentIndex = _panelLiabilities.transform.GetSiblingIndex() + 1;
            foreach (AssetContext assetContext in liabilities)
            {
                totalLiabilities += assetContext.Item2.amount;
                currentIndex = AddItemValueAsCurrency(
                    _panelLiabilities.transform.parent,
                    currentIndex,
                    _panelLiabilities.tabCount + 1,
                    assetContext.Item2.shortName,
                    assetContext.Item2.amount,
                    true,
                    getClickAction(assetContext.Item1, assetContext.Item2));
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

        private void reloadWindow()
        {
            GetComponent<MessageBox>().Destroy();
            UIManager.Instance.ShowAssetLiabilityStatusPanel();
        }
    }
}
