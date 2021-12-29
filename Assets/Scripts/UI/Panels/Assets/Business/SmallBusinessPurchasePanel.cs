using Assets;
using PlayerInfo;
using ScriptableObjects;
using System;
using TMPro;
using UI.Panels.Templates;
using UnityEngine;

namespace UI.Panels.Assets
{
    public class SmallBusinessPurchasePanel : AbstractBusinessPurchasePanel
    {
        private SmallBusiness _smallBusiness;
        public SmallBusiness business
        {
            get => _smallBusiness;
            set
            {
                _smallBusiness = value;
                _business = value;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            GetComponent<MessageBox>().confirmMessageHandler = GetConfirmMessage;
        }

        public void SwitchView(bool advanced)
        {
            MessageBox messageBox = GetComponent<MessageBox>();
            messageBox.Destroy();
            UIManager.Instance.ShowSmallBusinessPurchasePanel(
                business,
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
                    "Start a {0} business for {1}? You'll need to prepare a total of {2}.",
                    local.GetBusinessDescription(asset.description),
                    local.GetCurrency(asset.totalCost, true),
                    local.GetCurrency(partialAsset.fundsNeeded));
            }
            return "";
        }
    }
}
