using Assets;
using ScriptableObjects;
using System;
using TMPro;
using UI.Panels.Templates;
using UnityEngine;

namespace UI.Panels.Assets
{
    public class DistressedRealEstatePurchasePanel : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField]
        private TextMeshProUGUI _textMessage;
        [SerializeField]
        private TextMeshProUGUI _textPurchasePrice;
        [SerializeField]
        private TextMeshProUGUI _textRehabPrice;
        [SerializeField]
        private TextMeshProUGUI _textInterestFee;
        [SerializeField]
        private TextMeshProUGUI _textEstimatedValue;
        [SerializeField]
        private TextMeshProUGUI _textFundsNeeded;
        [SerializeField]
        private TextMeshProUGUI _textTotalLTV;
        [SerializeField]
        private TextMeshProUGUI _textOwnershipInterest;
        [SerializeField]
        private TextMeshProUGUI _textPrivateLoanAmount;
        [SerializeField]
        private TextMeshProUGUI _textInvestorAmount;
        [SerializeField]
        private RectTransform _equitySummaryPanel;
        [SerializeField]
        private DebtOfferingPanel _prefabDebtOfferingPanel;
        [SerializeField]
        private EquityOfferingPanel _prefabEquityOfferingPanel;
#pragma warning restore 0649

        public Player player;
        public DistressedRealEstate asset;
        public PartialRealEstate partialAsset;

        private void AdjustNumbers()
        {
            Localization local = Localization.Instance;

            int privateLoanAmount = asset.privateLoanAmount;
            int investorAmount = partialAsset.investorAmount;

            if (_textFundsNeeded != null)
            {
                _textFundsNeeded.text = local.GetCurrency(partialAsset.fundsNeeded, true);
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

            if (_textPrivateLoanAmount != null)
            {
                _textPrivateLoanAmount.text = local.GetCurrency(privateLoanAmount, true);
            }

            if (_textInterestFee != null)
            {
                _textInterestFee.text = local.GetCurrencyPlain(
                    asset.privateLoanDelayedPayment);
            }

            if (_textInvestorAmount != null)
            {
                _textInvestorAmount.text = local.GetCurrency(investorAmount);
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
                _textPurchasePrice.text = local.GetCurrencyPlain(
                    asset.purchasePrice);
            }

            if (_textRehabPrice != null)
            {
                _textRehabPrice.text = local.GetCurrencyPlain(asset.rehabPrice);
            }

            if (_textEstimatedValue != null)
            {
                if (player.HasSkill(SkillType.REAL_ESTATE_VALUATION))
                {
                    _textEstimatedValue.text = local.GetCurrencyPlain(
                        asset.template.basePrice);
                }
                else
                {
                    _textEstimatedValue.text = "???";
                }
            }

            AdjustNumbers();

            if (_equitySummaryPanel != null)
            {
                _equitySummaryPanel.gameObject.SetActive(partialAsset.investorShares > 0);
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
            UIManager.Instance.ShowContactListPanel(
                offerDebtContactSelect, false, true, false);
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
                offerEquityContactSelect, true, false, false);
        }

        public void OnResetButton()
        {
            partialAsset.OnPurchaseCancel();
            Refresh();
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

        public void OnSwitchViewButton(bool advanced)
        {
            MessageBox messageBox = GetComponent<MessageBox>();
            gameObject.SetActive(false);
            messageBox.Destroy();
            UIManager.Instance.ShowDistressedRealEstatePurchasePanel(
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
            int rate = InterestRateManager.Instance.distressedLoanRate;
            int maxLoan = Mathf.Min(partner.cash, asset.purchasePrice);

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
                    new PrivateLoan(partner, number, rate, true));
                AdjustNumbers();
            }
        }


    }
}
