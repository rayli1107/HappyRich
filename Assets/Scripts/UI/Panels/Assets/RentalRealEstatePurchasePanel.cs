using Assets;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Panels.Assets
{
    public class RentalRealEstatePurchasePanel : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField]
        private TextMeshProUGUI _textMessage;
        [SerializeField]
        private TextMeshProUGUI _textPurchasePrice;
        [SerializeField]
        private TextMeshProUGUI _textDownPayment;
        [SerializeField]
        private TextMeshProUGUI _textAnnualIncome;
        [SerializeField]
        private TextMeshProUGUI _textMortgage;
        [SerializeField]
        private TextMeshProUGUI _textMortgagePercentage;
        [SerializeField]
        private Slider _sliderMortgage;
        [SerializeField]
        private int _sliderMultiplier = 5;
#pragma warning restore 0649

        public Player player;
        public RentalRealEstate asset;

        private void AdjustNumbers()
        {
            Localization local = Localization.Instance;
            if (_textDownPayment != null)
            {
                _textDownPayment.text = local.GetCurrency(asset.downPayment, true);
            }

            if (_textAnnualIncome != null)
            {
                _textAnnualIncome.text = local.GetCurrency(asset.income);
            }

            if (_textMortgage != null)
            {
                _textMortgage.text = local.GetCurrency(asset.mortgage.amount, true);
            }

            if (_textMortgagePercentage != null)
            {
                _textMortgagePercentage.text = string.Format("{0}%", asset.mortgage.ltv);
            }
        }

        public void Refresh()
        {
            if (player == null || asset == null)
            {
                return;
            }

            Localization local = Localization.Instance;

            if (_textMessage != null)
            {
                _textMessage.text = string.Format(
                    "You found a {0} listed for sale. Purchase the property?",
                    local.GetRealEstateDescription(asset.template.profile));
            }

            if (_textPurchasePrice != null)
            {
                _textPurchasePrice.text = local.GetCurrency(
                    asset.purchasePrice, true);
            }

            AdjustNumbers();

            _sliderMortgage.value = asset.mortgage.ltv / _sliderMultiplier;
        }

        public void OnSliderChange()
        {
            asset.mortgage.ltv = Mathf.RoundToInt(
                _sliderMortgage.value * _sliderMultiplier);
            AdjustNumbers();
        }

        public void OnEnable()
        {
            Refresh();
        }
    }
}
