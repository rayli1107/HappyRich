using Assets;
using System.Collections.Generic;
using TMPro;
using UI.Panels.Templates;
using UnityEngine;

using Investment = System.Tuple<InvestmentPartner, int>;

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
            base.Awake();
            GetComponent<MessageBox>().confirmMessageHandler = GetConfirmMessage;

            if (_securedLoanControlPanel != null)
            {
                _securedLoanControlPanel.checkRaiseEquityCallback = () => false;
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

            if (_textPurchasePrice != null)
            {
                _textPurchasePrice.text = local.GetCurrencyPlain(
                    asset.value);
            }

            if (_securedLoanControlPanel != null)
            {
                EnableSecuredLoanPanel(true);
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
/*                Debug.LogFormat("Returned Capital {0}",
                    refinancedAsset.returnedCapital);
                    */
                List<Investment> returnedCapitalList =
                    RealEstateManager.Instance.CalculateReturnedCapitalForRefinance(
                        refinancedAsset, partialAsset);
                _textReturnedCapital.text = local.GetCurrency(
                    returnedCapitalList[0].Item2);
            }

            int actualIncome = refinancedAsset.income;
            int investorCashflow = Mathf.FloorToInt(
                partialAsset.investorEquity * actualIncome);
            int ownerIncome = actualIncome - investorCashflow;

            if (_textAnnualIncome != null)
            {
                _textAnnualIncome.text = local.GetCurrency(ownerIncome);
            }

        }

        public void OnResetRefinanceButton()
        {
            asset.ClearPrivateLoan();
            asset.primaryLoan.ltv = asset.primaryLoan.maxltv;
            Refresh();
        }

        public void OnSwitchViewButton(bool advanced)
        {
            MessageBox messageBox = GetComponent<MessageBox>();
            messageBox.Destroy();
            UIManager.Instance.ShowRentalRealEstateRefinancePanel(
                refinancedAsset, partialAsset, messageBox.messageBoxHandler, advanced);
        }

        private string GetConfirmMessage(ButtonType buttonType)
        {
            Localization local = Localization.Instance;
            if (buttonType == ButtonType.OK)
            {
                return string.Format(
                    "Refinance the {0} for a new loan of {1}?",
                    local.GetRealEstateDescription(asset.description),
                    local.GetCurrency(refinancedAsset.combinedLiability.amount));
            }
            return null;
        }
    }
}
