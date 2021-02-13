using Assets;
using TMPro;
using UI.Panels.Templates;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Panels.Assets
{
    public class RentalRealEstateRefinancePanel : AbstractRealEstatePanel
    {
#pragma warning disable 0649
        [SerializeField]
        protected TextMeshProUGUI _textReturnedCapital;
#pragma warning restore 0649

        public RefinancedRealEstate refinancedAsset;

        protected override void Awake()
        {
            if (_mortgageControlPanel != null)
            {
                _mortgageControlPanel.checkRaiseEquityCallback = () => false;
            }

            if (_privateLoanControlPanel != null)
            {
                _privateLoanControlPanel.checkRaiseEquityCallback = () => false;
            }
        }

        public override void Refresh()
        {
            base.Refresh();

            if (player == null || asset == null)
            {
                return;
            }

            Localization local = Localization.Instance;

            if (_textMessage != null)
            {
                _textMessage.text = string.Format(
                    "You finished repairing the {0} and rented it out. Refinance existing loans?",
                    local.GetRealEstateDescription(asset.description));
            }

            if (_textPurchasePrice != null)
            {
                _textPurchasePrice.text = local.GetCurrencyPlain(
                    asset.value);
            }

            if (_mortgageControlPanel != null)
            {
                EnableMortgagePanel(true);
            }

            bool enableDebtPanel = asset.privateLoan != null;
            if (_privateLoanControlPanel != null)
            {
                EnablePrivateLoanPanel(enableDebtPanel);
            }
        }

        protected override void AdjustNumbers()
        {
            base.AdjustNumbers();

            Localization local = Localization.Instance;
            if (_textReturnedCapital != null)
            {
                int returnedCapital = RealEstateManager.Instance.CalculateReturnedCapital(
                    refinancedAsset, partialAsset);
                _textReturnedCapital.text = local.GetCurrency(returnedCapital);
            }
        }

        public void OnResetRefinanceButton()
        {
            asset.ClearPrivateLoan();
            asset.mortgage.ltv = asset.mortgage.maxltv;
            Refresh();
        }

        public void OnSwitchViewButton(bool advanced)
        {
            MessageBox messageBox = GetComponent<MessageBox>();
            messageBox.Destroy();
            UIManager.Instance.ShowRentalRealEstateRefinancePanel(
                refinancedAsset, partialAsset, messageBox.messageBoxHandler, advanced);
        }
    }
}
