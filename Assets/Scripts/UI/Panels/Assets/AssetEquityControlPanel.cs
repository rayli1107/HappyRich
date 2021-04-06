using Assets;
using PlayerInfo;
using System;
using TMPro;
using UnityEngine;

namespace UI.Panels.Assets
{
    public class AssetEquityControlPanel : AssetLoanControlPanel
    {
#pragma warning disable 0649
        [SerializeField]
        private TextMeshProUGUI _textAmountRaised;
        [SerializeField]
        private TextMeshProUGUI _textShares;
        [SerializeField]
        private TextMeshProUGUI _textAmountPerShare;
        [SerializeField]
        private TextMeshProUGUI _textInvestorCashflow;
#pragma warning restore 0649

        public Player player;
        public AbstractRealEstate asset;
        public PartialRealEstate partialAsset;

        public Action adjustNumberCallback;

        public void Refresh()
        {
            if (player == null || asset == null || partialAsset == null)
            {
                return;
            }

            minValue = 0;
            maxValue = partialAsset.totalShares;
            value = partialAsset.investorShares;

            if (_textAmountPerShare != null)
            {
                Localization local = Localization.Instance;
                _textAmountPerShare.text = local.GetCurrency(
                    partialAsset.capitalPerShare);
            }

            AdjustNumbers();
        }

        public void OnSliderChange()
        {
            if (partialAsset != null)
            {
                partialAsset.investorShares = value;
                AdjustNumbers();
            }
        }

        private void AdjustNumbers()
        {
            adjustNumberCallback?.Invoke();

            Localization local = Localization.Instance;

            if (_textShares != null)
            {
                _textShares.text = partialAsset.investorShares.ToString();
            }

            if (_textInvestorCashflow != null)
            {
                _textInvestorCashflow.text = local.GetCurrency(partialAsset.investorCashflow);
            }

            if (_textAmountRaised != null)
            {
                _textAmountRaised.text = local.GetCurrency(
                    partialAsset.investorShares * partialAsset.capitalPerShare);
            }
        }

        private void OnEnable()
        {
            Refresh();
        }
    }
}
