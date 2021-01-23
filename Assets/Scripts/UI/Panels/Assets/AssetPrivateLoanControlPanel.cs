using Assets;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Panels.Assets
{
    public class AssetPrivateLoanControlPanel : AssetLoanControlPanel
    {
#pragma warning disable 0649
        [SerializeField]
        private TextMeshProUGUI _textAmount;
        [SerializeField]
        private TextMeshProUGUI _textRate;
        [SerializeField]
        private TextMeshProUGUI _textPayment;
        [SerializeField]
        private Button _buttonRaiseEquity;
#pragma warning restore 0649

        public Player player;
        public AbstractRealEstate asset;

        public Action adjustNumberCallback;
        public Func<bool> checkRaiseEquityCallback;

        private void enableButton(Button button, bool enable)
        {
            if (button != null)
            {
                button.gameObject.SetActive(enable);
            }
        }

        public void Refresh()
        {
            if (player == null || asset == null || asset.privateLoan == null)
            {
                return;
            }

            minValue = 0;
            maxValue = asset.privateLoan.maxltv;
            value = asset.privateLoan.ltv;

            if (_textRate != null)
            {
                _textRate.text = string.Format("{0}%", asset.privateLoan.interestRate);
            }

            enableButton(_buttonRaiseEquity, checkRaiseEquityCallback.Invoke());

            AdjustNumbers();
        }

        public void OnSliderChange()
        {
            if (asset != null && asset.privateLoan != null)
            {
                asset.privateLoan.ltv = value;
                AdjustNumbers();
            }
        }

        private void AdjustNumbers()
        {
            adjustNumberCallback?.Invoke();

            Localization local = Localization.Instance;

            if (_textAmount != null) {
                _textAmount.text = local.GetCurrency(
                    asset.privateLoan.amount, true);
            }

            if (_textPayment != null)
            {
                _textPayment.text = local.GetCurrency(
                    asset.privateLoan.expense, true);
            }
        }

        private void OnEnable()
        {
            Refresh();
        }
    }
}
