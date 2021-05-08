using System;
using TMPro;
using UI.Panels.Templates;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Panels.Assets
{
    public class DebtOfferingPanel : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField]
        private TextMeshProUGUI _textInterestRate;
        [SerializeField]
        private TextMeshProUGUI _textLoanAmount;
        [SerializeField]
        private TextMeshProUGUI _textAnnualInterest;
        [SerializeField]
        private Slider _sliderLoan;
#pragma warning restore 0649

        public int interestRate;
        public int maxLoanAmount;
        public NumberInputCallback numberInputCallback;
        public Action numberCancelCallback;

        private int _loanAmount;

        private void AdjustNumbers()
        {
            Localization local = Localization.Instance;

            if (_textInterestRate != null)
            {
                _textInterestRate.text = string.Format("{0}%", interestRate);
            }

            if (_textLoanAmount != null)
            {
                _textLoanAmount.text = local.GetCurrency(_loanAmount);
            }

            if (_textAnnualInterest != null)
            {
                _textAnnualInterest.text = local.GetCurrency(
                    _loanAmount * interestRate / 100, true);
            }
        }

        public void AdjustSlider()
        {
            _sliderLoan.value = (100f * _loanAmount) / maxLoanAmount;
        }

        public void Refresh()
        {
            AdjustNumbers();
            AdjustSlider();
        }

        public void OnNumberInputButton()
        {
            UIManager.Instance.ShowNumberInputPanel(
                "Loan Amount", maxLoanAmount, OnNumberInput, null);
        }

        public void OnSliderChange()
        {
            _loanAmount = Mathf.FloorToInt(maxLoanAmount * _sliderLoan.value / 100);
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
                numberInputCallback?.Invoke(_loanAmount);
            }
            else
            {
                numberCancelCallback?.Invoke();
            }
        }

        public void OnNumberInput(int number)
        {
            _loanAmount = number;
            Refresh();
        }
    }
}
