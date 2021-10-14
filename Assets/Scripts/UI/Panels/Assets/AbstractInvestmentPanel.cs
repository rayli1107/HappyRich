using Assets;
using PlayerInfo;
using ScriptableObjects;
using System;
using TMPro;
using UI.Panels.Templates;
using UnityEngine;

namespace UI.Panels.Assets
{
    public class AbstractInvestmentPanel : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField]
        protected AssetSecuredLoanControlPanel _securedLoanControlPanel;
        [SerializeField]
        protected AssetPrivateLoanControlPanel _privateLoanControlPanel;
        [SerializeField]
        protected AssetEquityControlPanel _equityControlPanel;

        [SerializeField]
        protected TextMeshProUGUI _textMessage;
        [SerializeField]
        protected TextMeshProUGUI _textPurchasePrice;
        [SerializeField]
        protected TextMeshProUGUI _textDownPayment;
        [SerializeField]
        protected TextMeshProUGUI _textAnnualIncome;
        [SerializeField]
        protected TextMeshProUGUI _textTotalLTV;
        [SerializeField]
        protected TextMeshProUGUI _textOwnershipInterest;
#pragma warning restore 0649

        public Player player;
        public AbstractInvestment asset;
        public PartialInvestment partialAsset;

        protected virtual int _privateLoanRate =>
            InterestRateManager.Instance.defaultPrivateLoanRate;
        protected virtual bool _privateLoanDelayed => false;

        protected string _messageTemplate { get; private set; }

        protected virtual void Awake()
        {
            if (_securedLoanControlPanel != null)
            {
                _securedLoanControlPanel.adjustNumberCallback = AdjustNumbers;
                _securedLoanControlPanel.checkRaiseDebtCallback = checkRaiseDebt;
                _securedLoanControlPanel.checkRaiseEquityCallback = checkRaiseEquity;
                _securedLoanControlPanel.onRaiseDebtCallback = OnRaiseDebtButton;
                _securedLoanControlPanel.onRaiseEquityCallback = OnRaiseEquityButton;
            }

            if (_privateLoanControlPanel != null)
            {
                _privateLoanControlPanel.adjustNumberCallback = AdjustNumbers;
                _privateLoanControlPanel.checkRaiseEquityCallback = checkRaiseEquity;
                _privateLoanControlPanel.onRaiseEquityCallback = OnRaiseEquityButton;
                _privateLoanControlPanel.onCancelCallback = OnCancelDebtButton;
                _privateLoanControlPanel.checkEnableCancelCallback = checkEnableCancelForPrivateLoanPanel;
            }

            if (_equityControlPanel != null)
            {
                _equityControlPanel.adjustNumberCallback = AdjustNumbers;
                _equityControlPanel.onCancelCallback = OnCancelEquityButton;
            }

            if (_textMessage != null)
            {
                _messageTemplate = _textMessage.text;
            }
        }

        private bool checkRaiseDebt()
        {
            int maxltv = RealEstateManager.Instance.maxPrivateLoanLTV;
            int rate = InterestRateManager.Instance.defaultPrivateLoanRate;
            PrivateLoan loan = new PrivateLoan(
                asset, player.GetDebtPartners(), maxltv, rate, false);
            return loan.maxltv > 0;
        }

        private bool checkRaiseEquity()
        {
            return partialAsset.maxShares > 0;
        }

        private bool checkEnableCancelForPrivateLoanPanel()
        {
            return asset.primaryLoan != null;
        }

        protected virtual void AdjustNumbers()
        {
            Localization local = Localization.Instance;

            int privateLoanAmount = asset.privateLoan == null ? 0 : asset.privateLoan.amount;
            int investorAmount = partialAsset.investorCapital;

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

            if (partialAsset.investorShares == 0)
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

            if (_textPurchasePrice != null)
            {
                _textPurchasePrice.text = local.GetCurrencyPlain(
                    asset.originalPrice);
            }

            AdjustNumbers();
        }

        public void OnEnable()
        {
            Refresh();
        }

        protected void EnableSecuredLoanPanel(bool enable)
        {
            if (_securedLoanControlPanel != null)
            {
                if (enable)
                {
                    _securedLoanControlPanel.player = player;
                    _securedLoanControlPanel.asset = asset;
                    _securedLoanControlPanel.gameObject.SetActive(true);
                    _securedLoanControlPanel.Refresh();
                }
                else
                {
                    _securedLoanControlPanel.gameObject.SetActive(false);
                }
            }
        }

        protected void EnablePrivateLoanPanel(bool enable)
        {
            if (_privateLoanControlPanel != null)
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
        }

        protected void EnableEquityPanel(bool enable)
        {
            if (_equityControlPanel != null)
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

        public void OnResetButton()
        {
            partialAsset.OnPurchaseCancel();
            if (asset.primaryLoan != null)
            {
                asset.primaryLoan.ltv = asset.primaryLoan.maxltv;
            }
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
    }
}
