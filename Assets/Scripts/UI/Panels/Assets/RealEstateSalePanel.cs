using Assets;
using PlayerInfo;
using ScriptableObjects;
using System;
using TMPro;
using UI.Panels.Templates;
using UnityEngine;

namespace UI.Panels.Assets
{
    public class RealEstateSalePanel : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField]
        protected TextMeshProUGUI _textMessage;
        [SerializeField]
        protected TextMeshProUGUI _textOfferPrice;
        [SerializeField]
        protected TextMeshProUGUI _textCurrentLoanAmount;
        [SerializeField]
        protected TextMeshProUGUI _textTotalGainOnSale;
        [SerializeField]
        protected TextMeshProUGUI _textOwnershipInterest;
#pragma warning restore 0649

        public Player player;
        public AbstractRealEstate asset;
        public PartialRealEstate partialAsset;
        public int initialOffer;
        public int finalOffer;
        public MessageBoxHandler handler;

        private void Awake()
        {
            GetComponent<MessageBox>().messageBoxHandler = messageBoxHandler;
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
                    "A buyer wants to purchase your {0} for {1}. Sell the property?",
                    local.GetRealEstateDescription(asset.description),
                    local.GetCurrency(finalOffer));
            }

            if (_textOfferPrice != null)
            {
                _textOfferPrice.text = local.GetCurrency(finalOffer);
            }

            int loanAmount = asset.combinedLiability.amount;
            if (_textCurrentLoanAmount != null)
            {
                _textCurrentLoanAmount.text = local.GetCurrency(loanAmount, true);
            }

            if (_textTotalGainOnSale != null)
            {
                _textTotalGainOnSale.text = local.GetCurrency(finalOffer - loanAmount);
            }
        }

        public void OnEnable()
        {
            Refresh();
        }

        private void handleAction(ButtonType buttonType, ButtonType actualButtonType)
        {
            if (buttonType == ButtonType.OK)
            {
                handler?.Invoke(actualButtonType);
            }
        }

        private void messageBoxHandler(ButtonType buttonType)
        {
            Localization local = Localization.Instance;
            if (buttonType == ButtonType.OK)
            {
                string message = string.Format(
                    "Sell the {0} for {1}?",
                    local.GetRealEstateDescription(asset.description),
                    local.GetCurrency(finalOffer));
                UIManager.Instance.ShowSimpleMessageBox(
                    message,
                    ButtonChoiceType.OK_CANCEL,
                    (ButtonType t) => handleAction(t, ButtonType.OK));
            }
            else
            {
                string message = string.Format(
                    "Reject the offer for {0}?",
                    local.GetRealEstateDescription(asset.description));
                UIManager.Instance.ShowSimpleMessageBox(
                    message,
                    ButtonChoiceType.OK_CANCEL,
                    (ButtonType t) => handleAction(t, ButtonType.CANCEL));
            }
        }
    }
}
