using Assets;
using PlayerInfo;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

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
        [SerializeField]
        private Button _buttonCancel;
#pragma warning restore 0649

        public Player player;
        public AbstractInvestment asset;
        public PartialInvestment partialAsset;

        public Action adjustNumberCallback;
        public UnityAction onCancelCallback;

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

            if (_buttonCancel != null)
            {
                _buttonCancel.onClick.RemoveAllListeners();
                _buttonCancel.onClick.AddListener(onCancelCallback);
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
                int incomeLow = partialAsset.investorCashflowRange.x;
                int incomeHigh = partialAsset.investorCashflowRange.y;
                if (incomeLow == incomeHigh)
                {
                    _textInvestorCashflow.text = local.GetCurrency(incomeLow);
                }
                else
                {
                    _textInvestorCashflow.text = string.Format(
                        "{0} ~ {1}",
                        local.GetCurrency(incomeLow),
                        local.GetCurrency(incomeHigh));
                }
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
