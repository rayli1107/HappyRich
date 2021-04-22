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

        private void Awake()
        {
            GetComponent<MessageBox>().confirmMessageHandler = GetConfirmMessage;
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

        private string GetConfirmMessage(ButtonType buttonType)
        {
            Localization local = Localization.Instance;
            if (buttonType == ButtonType.OK)
            {
                return string.Format(
                    "Sell the {0} for {1}?",
                    local.GetRealEstateDescription(asset.description),
                    local.GetCurrency(finalOffer));
            }
            else
            {
                return string.Format(
                        "Reject the offer for {0}?",
                        local.GetRealEstateDescription(asset.description));
            }
        }
    }
}
