using Assets;
using TMPro;
using UI.Panels.Templates;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Panels.Assets
{
    public class DebtOfferCallback : IContactSelectCallback, INumberInputCallback
    {
        public RentalRealEstatePurchasePanel parent;
        private InvestmentPartner _partner;
        private int _interestRate;

        public DebtOfferCallback(RentalRealEstatePurchasePanel parent)
        {
            this.parent = parent;
            _interestRate = InterestRateManager.Instance.defaultPrivateLoanRate;
        }

        public void ContactSelect(InvestmentPartner partner)
        {
            _partner = partner;
            Debug.LogFormat("Asset income {0}", parent.asset.income);
            Debug.LogFormat("Interest Rate {0}", _interestRate);

            int maxLoan = Mathf.Min(
                partner.cash,
                parent.asset.income * 100 / _interestRate);
            Debug.LogFormat("Max Loan {0}", maxLoan);
            parent.ShowDebtOfferingPanel(maxLoan, _interestRate, this);
        }

        public void OnNumberInput(int number)
        {
            if (number > 0)
            {
                parent.asset.AddPrivateLoan(
                    new PrivateLoan(_partner, number, _interestRate));
                parent.Refresh();
            }
        }

        public void OnNumberInputCancel()
        {
        }
    }

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
        private TextMeshProUGUI _textPrivateLoanAmount;
        [SerializeField]
        private TextMeshProUGUI _textPrivateLoanPayment;

        [SerializeField]
        private DebtOfferingPanel _prefabDebtOfferingPanel;
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

            if (_textPrivateLoanAmount != null)
            {
                _textPrivateLoanAmount.text = local.GetCurrency(asset.privateLoanAmount, true);
            }

            if (_textPrivateLoanPayment != null)
            {
                _textPrivateLoanPayment.text = local.GetCurrency(asset.privateLoanPayment, true);
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
            int maxLoanAmount, int loanRate, INumberInputCallback callback)
        {
            DebtOfferingPanel panel = Instantiate(
                _prefabDebtOfferingPanel, UIManager.Instance.transform);
            panel.interestRate = loanRate;
            panel.maxLoanAmount = maxLoanAmount;
            panel.callback = callback;
            panel.GetComponent<MessageBox>().messageBoxHandler = panel;
            panel.OnNumberInput(maxLoanAmount);
        }

        public void OnOfferDebtButton()
        {
            DebtOfferCallback callback = new DebtOfferCallback(this);
            UIManager.Instance.ShowContactListPanel(callback, false, true);
        }

        public void OnResetButton()
        {
            asset.ClearPrivateLoans();
            Refresh();
        }
    }
}
