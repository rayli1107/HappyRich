using Assets;
using System;
using TMPro;
using UI.Panels.Templates;
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
        private TextMeshProUGUI _textTotalLTV;
        [SerializeField]
        private TextMeshProUGUI _textOwnershipInterest;
        [SerializeField]
        private TextMeshProUGUI _textMortgage;
        [SerializeField]
        private TextMeshProUGUI _textMortgagePercentage;
        [SerializeField]
        private TextMeshProUGUI _textMortgagePayment;
        [SerializeField]
        private TextMeshProUGUI _textPrivateLoanAmount;
        [SerializeField]
        private TextMeshProUGUI _textPrivateLoanPayment;
        [SerializeField]
        private TextMeshProUGUI _textInvestorAmount;
        [SerializeField]
        private TextMeshProUGUI _textInvestorCashflow;
        [SerializeField]
        private RectTransform _debtSummaryPanel;
        [SerializeField]
        private RectTransform _equitySummaryPanel;
        [SerializeField]
        private DebtOfferingPanel _prefabDebtOfferingPanel;
        [SerializeField]
        private EquityOfferingPanel _prefabEquityOfferingPanel;
        [SerializeField]
        private Slider _sliderMortgage;
        [SerializeField]
        private Button _buttonRaiseDebt;
        [SerializeField]
        private int _sliderMultiplier = 5;
#pragma warning restore 0649

        public Player player;
        public RentalRealEstate asset;
        public PartialRealEstate partialAsset;

        private void AdjustNumbers()
        {
            Localization local = Localization.Instance;

            int privateLoanAmount = asset.privateLoanAmount;
            int investorAmount = partialAsset.investorAmount;

            if (_textDownPayment != null)
            {
                _textDownPayment.text = local.GetCurrency(partialAsset.downPayment, true);
            }

            if (_textAnnualIncome != null)
            {
                _textAnnualIncome.text = local.GetCurrency(partialAsset.income);
            }

            if (_textTotalLTV != null)
            {
                float totalLTV = (float)asset.combinedLiability.amount / asset.value;
                _textTotalLTV.text = local.GetPercentPlain(totalLTV, false);
            }

            if (_textOwnershipInterest != null)
            {
                _textOwnershipInterest.text = local.GetPercentPlain(
                    partialAsset.equity, false);
            }

            if (_textMortgage != null)
            {
                _textMortgage.text = local.GetCurrency(asset.mortgage.amount, true);
            }

            if (_textMortgagePercentage != null)
            {
                _textMortgagePercentage.text = string.Format("{0}%", asset.mortgage.ltv);
            }

            if (_textMortgagePayment != null)
            {
                _textMortgagePayment.text = local.GetCurrency(asset.mortgage.expense, true);
            }

            if (_textPrivateLoanAmount != null)
            {
                _textPrivateLoanAmount.text = local.GetCurrency(privateLoanAmount, true);
            }

            if (_textPrivateLoanPayment != null)
            {
                _textPrivateLoanPayment.text = local.GetCurrency(asset.privateLoanPayment, true);
            }

            if (_textInvestorAmount != null)
            {
                _textInvestorAmount.text = local.GetCurrency(investorAmount);
            }

            if (_textInvestorCashflow != null)
            {
                _textInvestorCashflow.text = local.GetCurrency(partialAsset.investorCashflow);
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
                    local.GetRealEstateDescription(asset.description));
            }

            if (_textPurchasePrice != null)
            {
                _textPurchasePrice.text = local.GetCurrency(
                    asset.purchasePrice, true);
            }

            AdjustNumbers();

            if (_sliderMortgage != null)
            {
                _sliderMortgage.maxValue = asset.mortgage.maxltv / _sliderMultiplier;
                _sliderMortgage.value = asset.mortgage.ltv / _sliderMultiplier;
            }

            _debtSummaryPanel.gameObject.SetActive(asset.privateLoanAmount > 0);
            _equitySummaryPanel.gameObject.SetActive(partialAsset.investorShares > 0);
        }

        public void OnSliderChange()
        {
            if (asset != null)
            {
                asset.mortgage.ltv = Mathf.RoundToInt(
                    _sliderMortgage.value * _sliderMultiplier);
                AdjustNumbers();
            }
        }

        public void OnEnable()
        {
            Refresh();
        }

        public void ShowDebtOfferingPanel(
            int maxLoanAmount,
            int loanRate,
            NumberInputCallback numberInputCallback,
            Action numberCancelCallback)
        {
            DebtOfferingPanel panel = Instantiate(
                _prefabDebtOfferingPanel, UIManager.Instance.transform);
            panel.interestRate = loanRate;
            panel.maxLoanAmount = maxLoanAmount;
            panel.numberInputCallback = numberInputCallback;
            panel.numberCancelCallback = numberCancelCallback;
            panel.GetComponent<MessageBox>().messageBoxHandler = panel.messageBoxHandler;
            panel.OnNumberInput(maxLoanAmount);
        }

        public void ShowEquityOfferingPanel(
            int maxShares,
            NumberInputCallback numberInputCallback,
            Action numberCancelCallback)

        {
            EquityOfferingPanel panel = Instantiate(
                _prefabEquityOfferingPanel, UIManager.Instance.transform);

            panel.amountPerShare = partialAsset.amountPerShare;
            panel.equityPerShare = partialAsset.equityPerShare;
            panel.maxShares = maxShares;
            panel.cashflow = partialAsset.income;
            panel.numberInputCallback = numberInputCallback;
            panel.numberCancelCallback = numberCancelCallback;
            panel.GetComponent<MessageBox>().messageBoxHandler = panel.messageBoxHandler;
            panel.OnNumberInput(maxShares);
        }

        public void OnOfferDebtButton()
        {
            UIManager.Instance.ShowContactListPanel(offerDebtContactSelect, false, true);
        }

        public void OnOfferEquityButton()
        {
            if (partialAsset.downPayment == 0)
            {
                string message = "You've raised the maximum amount needed.";
                UIManager.Instance.ShowSimpleMessageBox(
                    message, ButtonChoiceType.OK_ONLY, null);
                return;
            }

            UIManager.Instance.ShowContactListPanel(offerEquityContactSelect, true, false);
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
            int maxRaise = Mathf.Min(partner.cash, partialAsset.downPayment);
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
                asset.AddPrivateLoan(new PrivateLoan(partner, number, rate));
                AdjustNumbers();
            }
        }


    }
}
