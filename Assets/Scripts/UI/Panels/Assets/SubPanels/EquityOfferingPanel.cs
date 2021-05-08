using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UI.Panels.Templates;

namespace UI.Panels.Assets
{
    public class EquityOfferingPanel : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField]
        private TextMeshProUGUI _textEquity;
        [SerializeField]
        private TextMeshProUGUI _textInvestmentAmount;
        [SerializeField]
        private TextMeshProUGUI _textInvestorCashflow;
        [SerializeField]
        private Slider _sliderInvestment;
#pragma warning restore 0649

        public int amountPerShare;
        public float equityPerShare;
        public int maxShares;
        public int cashflow;
        public NumberInputCallback numberInputCallback;
        public Action numberCancelCallback;

        private int _shares;

        private void AdjustNumbers()
        {
            Localization local = Localization.Instance;

            float equity = _shares * equityPerShare;

            if (_textEquity != null)
            {
                _textEquity.text = local.GetPercent(equity);
            }

            if (_textInvestmentAmount != null)
            {
                int amount = amountPerShare * _shares;
                _textInvestmentAmount.text = local.GetCurrency(amount);
            }

            if (_textInvestorCashflow != null)
            {
                int investorCashflow = Mathf.FloorToInt(cashflow * equity);
                _textInvestorCashflow.text = local.GetCurrency(investorCashflow);
            }
        }

        public void AdjustSlider()
        {
            _sliderInvestment.maxValue = maxShares;
            _sliderInvestment.value = _shares;
        }

        public void Refresh()
        {
            AdjustNumbers();
            AdjustSlider();
        }

        public void OnNumberInputButton()
        {
            UIManager.Instance.ShowNumberInputPanel(
                "Investment Amount", amountPerShare * maxShares, OnNumberInput, null);
        }

        public void OnSliderChange()
        {
            _shares = Mathf.RoundToInt(_sliderInvestment.value);
            AdjustNumbers();
        }

        public void OnEnable()
        {
            Refresh();
        }

        public void messageBoxHandler(ButtonType button)
        {
            if (button == ButtonType.OK)
            {
                numberInputCallback?.Invoke(_shares);
            }
            else
            {
                numberCancelCallback?.Invoke();
            }
        }

        public void OnNumberInput(int number)
        {
            _shares = Mathf.Min(number / amountPerShare, maxShares);
            Refresh();
        }
    }
}
