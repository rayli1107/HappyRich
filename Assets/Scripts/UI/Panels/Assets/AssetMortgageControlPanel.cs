﻿using Assets;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Panels.Assets
{
    public class AssetMortgageControlPanel : AssetLoanControlPanel
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
        public AbstractRealEstate asset;
        public Action adjustNumberCallback;
        public Func<bool> checkRaiseDebtCallback;
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
            if (player == null || asset == null)
            {
                return;
            }

            minValue = 0;
            maxValue = asset.mortgage.maxltv;
            value = asset.mortgage.ltv;

            if (_textRate != null)
            {
                _textRate.text = string.Format("{0}%", asset.mortgage.interestRate);
            }

            if (checkRaiseDebtCallback.Invoke())
            {
                enableButton(_buttonRaiseDebt, true);
                enableButton(_buttonRaiseEquity, false);
            }
            else
            {
                enableButton(_buttonRaiseDebt, false);
                enableButton(_buttonRaiseEquity, checkRaiseEquityCallback.Invoke());
            }

            AdjustNumbers();
        }

        public void OnSliderChange()
        {
            if (asset != null)
            {
                asset.mortgage.ltv = value;
                AdjustNumbers();

                Debug.LogFormat(
                    "LTV {0} Amount {1} Expense {2}",
                    asset.mortgage.ltv,
                    asset.mortgage.amount,
                    asset.mortgage.expense);
            }
        }

        private void AdjustNumbers()
        {
            adjustNumberCallback?.Invoke();

            Localization local = Localization.Instance;

            if (_textAmount != null) {
                _textAmount.text = local.GetCurrency(asset.mortgage.amount, true);
            }

            if (_textPayment != null)
            {
                _textPayment.text = local.GetCurrency(asset.mortgage.expense, true);
            }
        }

        private void OnEnable()
        {
            Refresh();
        }
    }
}
