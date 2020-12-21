using Assets;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UI;

namespace UI.Panels.Assets
{
    public class EquityOfferingPanel : MonoBehaviour, IMessageBoxHandler, INumberInputCallback
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
        public INumberInputCallback callback;

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
                "Investment Amount", amountPerShare * maxShares, this);
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

        public void OnButtonClick(ButtonType button)
        {
            if (button == ButtonType.OK)
            {
                if (callback != null)
                {
                    callback.OnNumberInput(_shares);
                }
            }
            else
            {
                if (callback != null)
                {
                    callback.OnNumberInputCancel();
                }
            }
        }

        public void OnNumberInput(int number)
        {
            _shares = Mathf.Min(number / amountPerShare, maxShares);
            Refresh();
        }

        public void OnNumberInputCancel()
        {
        }
    }
}
