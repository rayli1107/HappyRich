using Assets;
using ScriptableObjects;
using System;
using System.Collections.Generic;
using TMPro;
using UI.Panels.Templates;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Panels.Assets
{
    public class RentalRealEstatePurchasePanel : AbstractRealEstatePanel
    {
        protected override void Awake()
        {
            base.Awake();
            GetComponent<MessageBox>().confirmMessageHandler = GetConfirmMessage;
        }

        public void OnSwitchViewButton(bool advanced)
        {
            MessageBox messageBox = GetComponent<MessageBox>();
            messageBox.Destroy();
            UIManager.Instance.ShowRentalRealEstatePurchasePanel(
                (AbstractRealEstate)asset,
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
                    "Buy the {0} for {1}?",
                    local.GetRealEstateDescription(asset.description),
                    local.GetCurrency(asset.totalCost, true));
            }
            return "";
        }
    }
}
