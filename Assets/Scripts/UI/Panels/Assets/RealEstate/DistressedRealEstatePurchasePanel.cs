using Assets;
using ScriptableObjects;
using System;
using TMPro;
using UI.Panels.Templates;
using UnityEngine;

namespace UI.Panels.Assets
{
    public class DistressedRealEstatePurchasePanel : AbstractRealEstatePanel
    {
#pragma warning disable 0649
        [SerializeField]
        private TextMeshProUGUI _textRehabPrice;
        [SerializeField]
        private TextMeshProUGUI _textInterestFee;
        [SerializeField]
        private RectTransform _equitySummaryPanel;
#pragma warning restore 0649

        public DistressedRealEstate distressedAsset;

        protected override int _privateLoanRate =>
            InterestRateManager.Instance.distressedLoanRate;
        protected override bool _privateLoanDelayed => true;

        protected override void Awake()
        {
            base.Awake();
            GetComponent<MessageBox>().confirmMessageHandler = GetConfirmMessage;
        }

        public override void Refresh()
        {
            base.Refresh();

            if (player == null || asset == null)
            {
                return;
            }

            Localization local = Localization.Instance;
            if (_textRehabPrice != null)
            {
                _textRehabPrice.text = local.GetCurrencyPlain(distressedAsset.rehabPrice);
            }
        }

        protected override void AdjustNumbers()
        {
            base.AdjustNumbers();

            Localization local = Localization.Instance;
            if (_textInterestFee != null)
            {
                _textInterestFee.text = local.GetCurrency(asset.delayedInterest, true);
            }
/*
            Debug.LogFormat("Original Price {0}", distressedAsset.originalPrice);
            Debug.LogFormat("Rehab Price {0}", distressedAsset.rehabPrice);
            Debug.LogFormat("Value {0}", distressedAsset.value);
            Debug.LogFormat("Liability {0}", distressedAsset.combinedLiability.amount);
            Debug.LogFormat("Down Payment {0}", distressedAsset.downPayment);
            Debug.LogFormat("Funds Needed {0}", partialAsset.fundsNeeded);
            */
        }

        public void OnSwitchViewButton(bool advanced)
        {
            MessageBox messageBox = GetComponent<MessageBox>();
            messageBox.Destroy();
            UIManager.Instance.ShowDistressedRealEstatePurchasePanel(
                distressedAsset,
                partialAsset,
                messageBox.messageBoxHandler,
                messageBox.startTransactionHandler,
                advanced);
        }

        private string GetConfirmMessage(ButtonType buttonType)
        {
            Localization local = Localization.Instance;
            if (buttonType == ButtonType.OK)
            {
                return string.Format(
                    "Buy the {0} for a total of {1}?",
                    local.GetRealEstateDescription(asset.description),
                    local.GetCurrency(asset.totalCost, true));
            }
            return "";
        }
    }
}
