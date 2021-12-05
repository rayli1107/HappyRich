using Assets;
using PlayerInfo;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI.Panels.Assets
{
    public class AssetSecuredLoanControlPanel : AssetLoanControlPanel
    {
#pragma warning disable 0649
        [SerializeField]
        private TextMeshProUGUI _textAmount;
        [SerializeField]
        private TextMeshProUGUI _textRate;
        [SerializeField]
        private TextMeshProUGUI _textPayment;
        [SerializeField]
        private Button _buttonRaiseDebt;
        [SerializeField]
        private Button _buttonRaiseEquity;
#pragma warning restore 0649

        public Player player;
        public AbstractInvestment asset;
        public Action adjustNumberCallback;
        public Func<bool> checkRaiseDebtCallback;
        public Func<bool> checkRaiseEquityCallback;
        public UnityAction onRaiseDebtCallback;
        public UnityAction onRaiseEquityCallback;

        private void enableButton(Button button, bool enable)
        {
            if (button != null)
            {
                button.gameObject.SetActive(enable);
            }
        }

        public void Refresh()
        {
            if (player == null || asset == null || asset.primaryLoan == null)
            {
                return;
            }

            minValue = asset.primaryLoan.minltv;
            maxValue = asset.primaryLoan.maxltv;
            value = asset.primaryLoan.ltv;

            if (_textRate != null)
            {
                _textRate.text = string.Format("{0}%", asset.primaryLoan.interestRate);
            }

            if (_buttonRaiseDebt != null)
            {
                _buttonRaiseDebt.onClick.RemoveAllListeners();
                _buttonRaiseDebt.onClick.AddListener(onRaiseDebtCallback);
            }

            if (_buttonRaiseEquity != null)
            {
                _buttonRaiseEquity.onClick.RemoveAllListeners();
                _buttonRaiseEquity.onClick.AddListener(onRaiseEquityCallback);
            }


            enableButton(_buttonRaiseDebt, checkRaiseDebtCallback.Invoke());
            AdjustNumbers();
        }

        public void OnSliderChange()
        {
            if (asset != null)
            {
                asset.primaryLoan.ltv = value;
                AdjustNumbers();
            }
        }

        private void AdjustNumbers()
        {
            adjustNumberCallback?.Invoke();

            Localization local = Localization.Instance;

            if (_textAmount != null) {
                _textAmount.text = local.GetCurrency(asset.primaryLoan.amount, true);
            }

            if (_textPayment != null)
            {
                _textPayment.text = local.GetCurrency(asset.primaryLoan.expense, true);
            }

            bool debtButtonActive =
                _buttonRaiseDebt != null && _buttonRaiseDebt.gameObject.activeInHierarchy;
            enableButton(_buttonRaiseEquity,
                !debtButtonActive && checkRaiseEquityCallback.Invoke());
        }

        private void OnEnable()
        {
            Refresh();
        }
    }
}
