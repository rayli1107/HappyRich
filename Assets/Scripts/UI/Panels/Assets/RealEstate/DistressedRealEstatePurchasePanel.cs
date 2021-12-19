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
#pragma warning restore 0649

        public DistressedRealEstate distressedAsset;

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
    }
}
