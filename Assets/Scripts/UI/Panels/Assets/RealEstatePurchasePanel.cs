using Assets;
using TMPro;
using UnityEngine;

namespace UI.Panels.Assets
{
    public class RealEstatePurchasePanel : MonoBehaviour
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
#pragma warning restore 0649

        public Player player;
        public AbstractRealEstate asset;

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

            if (_textDownPayment != null)
            {
                int loanAmount = 0;
                foreach (AbstractLiability liability in asset.liabilities)
                {
                    loanAmount += liability.amount;
                }
                _textDownPayment.text = local.GetCurrency(
                    asset.purchasePrice - loanAmount, true);
            }

            if (_textAnnualIncome != null)
            {
                _textAnnualIncome.text = local.GetCurrency(asset.income);
            }


        }
        public void OnEnable()
        {
            Refresh();
        }
    }
}
