using Assets;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Panels.Assets
{
    public class RentalRealEstateRefinancePanel : AbstractRealEstatePanel
    {
#pragma warning disable 0649
        [SerializeField]
        private AssetMortgageControlPanel _mortgageControlPanel;
#pragma warning restore 0649

        public RentalRealEstate rentalAsset;

        private void Awake()
        {
            _mortgageControlPanel.player = player;
            _mortgageControlPanel.asset = asset;
            _mortgageControlPanel.adjustNumberCallback = AdjustNumbers;
            _mortgageControlPanel.checkRaiseDebtCallback = () => false;
            _mortgageControlPanel.checkRaiseEquityCallback = () => false;
        }

        public override void Refresh()
        {
            if (player == null || asset == null)
            {
                return;
            }

            base.Refresh();

            _mortgageControlPanel.gameObject.SetActive(true);
            _mortgageControlPanel.Refresh();
        }


        /*
        public void OnOfferDebtButton()
        {
            UIManager.Instance.ShowContactListPanel(
                offerDebtContactSelect, false, true, true);
        }

        public void OnOfferEquityButton()
        {
            if (partialAsset.fundsNeeded == 0)
            {
                string message = "You've raised the maximum amount needed.";
                UIManager.Instance.ShowSimpleMessageBox(
                    message, ButtonChoiceType.OK_ONLY, null);
                return;
            }

            UIManager.Instance.ShowContactListPanel(
                offerEquityContactSelect, true, true, false);
        }

        public void OnResetButton()
        {
            partialAsset.OnPurchaseCancel();
            Refresh();
        }

        public void OnRaiseDebtButton()
        {
            _debtSummaryPanel.gameObject.SetActive(true);
        }

        public void OnRaiseEquityButton()
        {
            _equitySummaryPanel.gameObject.SetActive(true);
        }

        public void OnCancelEquityButton()
        {
            partialAsset.ClearInvestors();
            Refresh();
        }

        public void OnCancelDebtButton()
        {
            asset.ClearPrivateLoans();
            Refresh();
        }

        public void OnSwitchViewButton(bool advanced)
        {
            MessageBox messageBox = GetComponent<MessageBox>();
            gameObject.SetActive(false);
            messageBox.Destroy();
            UIManager.Instance.ShowRentalRealEstatePurchasePanel(
                asset, partialAsset, messageBox.messageBoxHandler, advanced);
        }

        private void offerEquityContactSelect(InvestmentPartner partner)
        {
            int maxRaise = Mathf.Min(partner.cash, partialAsset.fundsNeeded);
            int maxShares = maxRaise / partialAsset.amountPerShare;

            if (maxShares == 0)
            {
                string message = "You've raised the maximum amount needed.";
                UIManager.Instance.ShowSimpleMessageBox(
                    message, ButtonChoiceType.OK_ONLY, null);
                return;
            }

            NumberInputCallback callback = (int n) => offerEquityNumberInput(partner, n);
            ShowEquityOfferingPanel(maxShares, callback, null);
        }

        private void offerEquityNumberInput(InvestmentPartner partner, int number)
        {
            if (number > 0)
            {
                partialAsset.AddInvestor(partner, number);
                AdjustNumbers();
            }
        }

        private void offerDebtContactSelect(InvestmentPartner partner)
        {
            int rate = InterestRateManager.Instance.defaultPrivateLoanRate;
            int maxLoan = Mathf.Min(
                partner.cash,
                asset.income * 100 / rate,
                asset.downPayment);

            if (maxLoan <= 0)
            {
                string message = "You've raised the maximum amount needed.";
                UIManager.Instance.ShowSimpleMessageBox(
                    message, ButtonChoiceType.OK_ONLY, null);
                return;
            }

            NumberInputCallback callback = (int n) => offerDebtNumberInput(partner, rate, n);
            ShowDebtOfferingPanel(maxLoan, rate, callback, null);
        }

        private void offerDebtNumberInput(
            InvestmentPartner partner, int rate, int number)
        {
            if (number > 0)
            {
                asset.AddPrivateLoan(
                    new PrivateLoan(partner, number, rate, false));
                AdjustNumbers();
            }
        }
*/

    }
}
