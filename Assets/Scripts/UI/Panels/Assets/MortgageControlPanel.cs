using Assets;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Panels.Assets
{
    public class MortgageControlPanel : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField]
        private TextMeshProUGUI _textMortgage;
        [SerializeField]
        private TextMeshProUGUI _textMortgagePercentage;
        [SerializeField]
        private TextMeshProUGUI _textMortgagePayment;
        [SerializeField]
        private Slider _sliderMortgage;
        [SerializeField]
        private RentalRealEstatePurchasePanel _purchasePanel;
        [SerializeField]
        private int _sliderMultiplier = 5;
#pragma warning restore 0649


        private void AdjustNumbers()
        {
            Localization local = Localization.Instance;

            Mortgage mortgage = _purchasePanel.asset.mortgage;

            if (_textMortgage != null)
            {
                _textMortgage.text = local.GetCurrency(mortgage.amount, true);
            }

            if (_textMortgagePercentage != null)
            {
                _textMortgagePercentage.text = string.Format("{0}%", mortgage.ltv);
            }

            if (_textMortgagePayment != null)
            {
                _textMortgagePayment.text = local.GetCurrency(mortgage.expense, true);
            }
        }

        public void Refresh()
        {
            if (_purchasePanel.asset == null)
            {
                return;
            }

            AdjustNumbers();

            _sliderMortgage.value = _purchasePanel.asset.mortgage.ltv / _sliderMultiplier;
        }

        public void OnSliderChange()
        {
            if (_purchasePanel.asset != null)
            {
                _purchasePanel.asset.mortgage.ltv = Mathf.RoundToInt(
                    _sliderMortgage.value * _sliderMultiplier);
                AdjustNumbers();
                _purchasePanel.Refresh();
            }
        }

        public void OnEnable()
        {
            Refresh();
        }
    }
}
