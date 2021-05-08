using Assets;
using PlayerInfo;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
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
        [SerializeField]
        private Button _buttonCancel;
        [SerializeField]
        private bool _disableCancel = false;
#pragma warning restore 0649

        public Player player;
        public AbstractInvestment asset;

        public Action adjustNumberCallback;
        public Func<bool> checkRaiseEquityCallback;
        public UnityAction onRaiseEquityCallback;
        public UnityAction onCancelCallback;

        public void Refresh()
        {
            if (player == null || asset == null || asset.privateLoan == null)
            {
                return;
            }

            minValue = asset.privateLoan.minltv;
            maxValue = asset.privateLoan.maxltv;
            value = asset.privateLoan.ltv;

            if (_textRate != null)
            {
                _textRate.text = string.Format("{0}%", asset.privateLoan.interestRate);
            }

            if (!_disableCancel && _buttonCancel != null)
            {
                _buttonCancel.gameObject.SetActive(minValue == 0);
            }

            if (_buttonRaiseEquity != null)
            {
                _buttonRaiseEquity.onClick.RemoveAllListeners();
                _buttonRaiseEquity.onClick.AddListener(onRaiseEquityCallback);
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

            if (_buttonRaiseEquity != null)
            {
                _buttonRaiseEquity.gameObject.SetActive(
                    checkRaiseEquityCallback.Invoke());
            }
        }

        private void OnEnable()
        {
            Refresh();
        }
    }
}
