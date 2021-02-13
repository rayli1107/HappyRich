using Assets;
using PlayerInfo;
using ScriptableObjects;
using System;
using TMPro;
using UI.Panels.Templates;
using UnityEngine;

namespace UI.Panels.Assets
{
    public class AbstractRealEstatePanel : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField]
        protected AssetMortgageControlPanel _mortgageControlPanel;
        [SerializeField]
        protected AssetPrivateLoanControlPanel _privateLoanControlPanel;
        [SerializeField]
        protected AssetEquityControlPanel _equityControlPanel;

        [SerializeField]
        protected bool _selectContact;

        [SerializeField]
        protected TextMeshProUGUI _textMessage;
        [SerializeField]
        protected TextMeshProUGUI _textPurchasePrice;
        [SerializeField]
        protected TextMeshProUGUI _textEstimatedValue;
        [SerializeField]
        protected TextMeshProUGUI _textDownPayment;
        [SerializeField]
        protected TextMeshProUGUI _textAnnualIncome;
        [SerializeField]
        protected TextMeshProUGUI _textTotalLTV;
        [SerializeField]
        protected TextMeshProUGUI _textOwnershipInterest;
        [SerializeField]
        protected TextMeshProUGUI _textMortgage;
        [SerializeField]
        protected TextMeshProUGUI _textMortgagePercentage;
        [SerializeField]
        protected TextMeshProUGUI _textMortgagePayment;
        [SerializeField]
        protected TextMeshProUGUI _textPrivateLoanAmount;
        [SerializeField]
        protected TextMeshProUGUI _textPrivateLoanPayment;
        [SerializeField]
        protected TextMeshProUGUI _textInvestorAmount;
        [SerializeField]
        protected TextMeshProUGUI _textInvestorCashflow;
        [SerializeField]
        protected DebtOfferingPanel _prefabDebtOfferingPanel;
        [SerializeField]
        protected EquityOfferingPanel _prefabEquityOfferingPanel;
#pragma warning restore 0649

        public Player player;
        public AbstractRealEstate asset;
        public PartialRealEstate partialAsset;

        protected virtual int _privateLoanRate =>
            InterestRateManager.Instance.defaultPrivateLoanRate;
        protected virtual bool _privateLoanDelayed => false;

        protected virtual void Awake()
        {
            if (_mortgageControlPanel != null)
            {
                _mortgageControlPanel.adjustNumberCallback = AdjustNumbers;
                _mortgageControlPanel.checkRaiseDebtCallback = checkRaiseDebt;
                _mortgageControlPanel.checkRaiseEquityCallback = checkRaiseEquity;
            }

            if (_privateLoanControlPanel != null)
            {
                _privateLoanControlPanel.adjustNumberCallback = AdjustNumbers;
                _privateLoanControlPanel.checkRaiseEquityCallback = checkRaiseEquity;
            }

            if (_equityControlPanel != null)
            {
                _equityControlPanel.adjustNumberCallback = AdjustNumbers;
            }
        }

        private bool checkRaiseDebt()
        {
            int maxltv = RealEstateManager.Instance.maxPrivateLoanLTV;
            int rate = InterestRateManager.Instance.defaultPrivateLoanRate;
            RealEstatePrivateLoan loan = new RealEstatePrivateLoan(
                asset, player.GetDebtPartners(), maxltv, rate, false);
            return loan.maxltv > 0;
        }

        private bool checkRaiseEquity()
        {
            return partialAsset.maxShares > 0;
        }

        protected virtual void AdjustNumbers()
        {
            Localization local = Localization.Instance;

            int privateLoanAmount = asset.privateLoan == null ? 0 : asset.privateLoan.amount;
            int investorAmount = partialAsset.investorAmount;

            if (_textDownPayment != null)
            {
                _textDownPayment.text = local.GetCurrency(partialAsset.fundsNeeded, true);
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
                _textPrivateLoanPayment.text = local.GetCurrency(
                    asset.privateLoan == null ? 0 : asset.privateLoan.expense, true);
            }

            if (_textInvestorAmount != null)
            {
                _textInvestorAmount.text = local.GetCurrency(investorAmount);
            }

            if (_textInvestorCashflow != null)
            {
                _textInvestorCashflow.text = local.GetCurrency(partialAsset.investorCashflow);
            }

            if (partialAsset.shares == 0)
            {
                partialAsset.Reset();
            }
        }

        public virtual void Refresh()
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
                    asset.originalPrice);
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

        protected void EnableMortgagePanel(bool enable)
        {
            if (enable)
            {
                _mortgageControlPanel.player = player;
                _mortgageControlPanel.asset = asset;
                _mortgageControlPanel.gameObject.SetActive(true);
                _mortgageControlPanel.Refresh();
            }
            else
            {
                _mortgageControlPanel.gameObject.SetActive(false);
            }
        }

        protected void EnablePrivateLoanPanel(bool enable)
        {
            if (enable)
            {
                _privateLoanControlPanel.player = player;
                _privateLoanControlPanel.asset = asset;
                _privateLoanControlPanel.gameObject.SetActive(true);
                _privateLoanControlPanel.Refresh();
            }
            else
            {
                _privateLoanControlPanel.gameObject.SetActive(false);
            }
        }

        protected void EnableEquityPanel(bool enable)
        {
            if (enable)
            {
                _equityControlPanel.player = player;
                _equityControlPanel.asset = asset;
                _equityControlPanel.partialAsset = partialAsset;
                _equityControlPanel.gameObject.SetActive(true);
                _equityControlPanel.Refresh();
            }
            else
            {
                _equityControlPanel.gameObject.SetActive(false);
            }
        }

        public void OnRaiseDebtButton()
        {
            asset.AddPrivateLoan(
                player.GetDebtPartners(),
                RealEstateManager.Instance.maxPrivateLoanLTV,
                InterestRateManager.Instance.defaultPrivateLoanRate,
                false);
            EnablePrivateLoanPanel(true);
        }

        public void OnRaiseEquityButton()
        {
            partialAsset.Reset();
            EnableEquityPanel(true);
        }



        /*
                public void OnOfferDebtButton()
                {
                    List<InvestmentPartner> partners = new List<InvestmentPartner>();
                    partners.AddRange(player.GetDebtPartners());
                    partners.RemoveAll((InvestmentPartner p) => p.cash <= 0);
                    int maxLoanAmount = 0;
                    foreach (InvestmentPartner partner in partners)
                    {
                        Debug.LogFormat("{0} - {1}", partner.name, partner.cash);
                        maxLoanAmount += partner.cash;
                    }
                    Debug.LogFormat("Total - {0}", maxLoanAmount);
                    Debug.LogFormat("Max Private Loan Amount - {0}", asset.privateLoan?.amount);
                    Debug.LogFormat("Combined Liability Amount - {0}", asset.combinedLiability.amount);

                    maxLoanAmount = Mathf.Min(
                        maxLoanAmount,
                        asset.privateLoan?.amount - asset.combinedLiability.amount);

                    if (maxLoanAmount == 0)
                    {
                        string message = "You've raised the maximum allowed amount of debt.";
                        UIManager.Instance.ShowSimpleMessageBox(
                            message, ButtonChoiceType.OK_ONLY, null);
                        return;
                    }

                    if (_selectContact)
                    {
                        ContactSelectCallback callback =
                            (InvestmentPartner p) => offerDebtContactSelect(p, maxLoanAmount);
                        UIManager.Instance.ShowContactListPanel(partners, callback);
                    }
                    else
                    {
                        NumberInputCallback callback =
                            (int n) => offerDebtNumberInput(partners, _privateLoanRate, n);
                        ShowDebtOfferingPanel(
                            maxLoanAmount, _privateLoanRate, callback, null);
                    }
                }

                public void OnOfferEquityButton()
                {
                    int sharesNeeded = partialAsset.fundsNeeded / partialAsset.amountPerShare;
                    if (sharesNeeded == 0)
                    {
                        string message = "You've raised the maximum amount needed.";
                        UIManager.Instance.ShowSimpleMessageBox(
                            message, ButtonChoiceType.OK_ONLY, null);
                        return;
                    }

                    List<InvestmentPartner> partners = new List<InvestmentPartner>();
                    partners.AddRange(player.GetPartners(false, false, true));
                    partners.AddRange(player.GetPartners(false, true, false));
                    partners.RemoveAll(
                        (InvestmentPartner p) => p.cash < partialAsset.amountPerShare);
                    int sharesAvailable = 0;
                    foreach (InvestmentPartner partner in partners)
                    {
                        sharesAvailable += partner.cash / partialAsset.amountPerShare;
                    }

                    if (sharesAvailable == 0)
                    {
                        string message = "You don't have any investors with available funds.";
                        UIManager.Instance.ShowSimpleMessageBox(
                            message, ButtonChoiceType.OK_ONLY, null);
                        return;
                    }

                    int maxShares = Mathf.Min(sharesAvailable, sharesNeeded);

                    if (_selectContact)
                    {
                        UIManager.Instance.ShowContactListPanel(
                            partners, offerEquityContactSelect);
                    }
                    else
                    {
                        NumberInputCallback callback =
                            (int n) => offerEquityNumberInput(partners, n);
                        ShowEquityOfferingPanel(maxShares, callback, null);
                    }
                }
        */
        public void OnResetButton()
        {
            partialAsset.OnPurchaseCancel();
            asset.mortgage.ltv = asset.mortgage.maxltv;
            Refresh();
        }

        public void OnCancelEquityButton()
        {
            partialAsset.Reset();
            Refresh();
        }

        public void OnCancelDebtButton()
        {
            asset.ClearPrivateLoan();
            Refresh();
        }

        /*

        private void offerEquityContactSelect(InvestmentPartner partner)
        {
            int maxRaise = Mathf.Min(partner.cash, partialAsset.fundsNeeded);
            int maxShares = maxRaise / partialAsset.amountPerShare;

            NumberInputCallback callback =
                (int n) => offerEquityPartnerNumberInput(partner, n);
            ShowEquityOfferingPanel(maxShares, callback, null);
        }

        private void offerEquityNumberInput(
            List<InvestmentPartner> partners, int totalShares)
        {
            foreach (InvestmentPartner partner in partners)
            {
                int investorShares = partner.cash / partialAsset.amountPerShare;
                int shares = Mathf.Min(totalShares, investorShares);
                if (shares > 0)
                {
                    partialAsset.AddInvestor(partner, shares);
                }
                totalShares -= shares;
            }
            AdjustNumbers();
        }

        private void offerEquityPartnerNumberInput(
            InvestmentPartner partner, int number)
        {
            if (number > 0)
            {
                partialAsset.AddInvestor(partner, number);
                AdjustNumbers();
            }
        }

        private void offerDebtContactSelect(
            InvestmentPartner partner,
            int maxLoanAmount)
        {
            maxLoanAmount = Mathf.Min(maxLoanAmount, partner.cash);

            if (maxLoanAmount <= 0)
            {
                string message = "You've raised the maximum amount needed.";
                UIManager.Instance.ShowSimpleMessageBox(
                    message, ButtonChoiceType.OK_ONLY, null);
                return;
            }

            NumberInputCallback callback =
                (int n) => offerDebtPartnerNumberInput(partner, _privateLoanRate, n);
            ShowDebtOfferingPanel(
                maxLoanAmount, _privateLoanRate, callback, null);
        }

        private void offerDebtNumberInput(
            List<InvestmentPartner> partners, int rate, int loanAmount)
        {
            foreach (InvestmentPartner partner in partners)
            {
                int partnerLoanAmount = Mathf.Min(loanAmount, partner.cash);
                if (partnerLoanAmount > 0)
                {
                    PrivateLoan loan = new PrivateLoan(
                        partner, partnerLoanAmount, rate, _privateLoanDelayed);
                    asset.AddPrivateLoan(loan);
                }
                loanAmount -= partnerLoanAmount;
            }
            AdjustNumbers();
        }

        private void offerDebtPartnerNumberInput(
            InvestmentPartner partner, int rate, int loanAmount)
        {
            if (loanAmount > 0)
            {
                PrivateLoan loan = new PrivateLoan(
                    partner, loanAmount, rate, _privateLoanDelayed);
                asset.AddPrivateLoan(loan);
                AdjustNumbers();
            }
        }
        */
    }
}
