using Assets;
using PlayerInfo;
using ScriptableObjects;
using System;
using TMPro;
using UI.Panels.Templates;
using UnityEngine;

namespace UI.Panels.Assets
{
    public class StartupPurchasePanel : AbstractInvestmentPanel
    {
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
            if (_textMessage != null)
            {
                string message = string.Format(
                    _messageTemplate,
                    local.GetBusinessDescription(asset.description));
                _textMessage.text = message;
            }

            if (_securedLoanControlPanel != null)
            {
                EnableSecuredLoanPanel(true);
            }

            bool enableEquityPanel = partialAsset.investorShares > 0;
            bool enableDebtPanel = asset.privateLoan != null;

            if (_equityControlPanel != null)
            {
                EnableEquityPanel(enableEquityPanel);
            }

            if (_privateLoanControlPanel != null && !enableEquityPanel)
            {
                EnablePrivateLoanPanel(enableDebtPanel);
            }
        }

        public void SwitchView(bool advanced)
        {
            MessageBox messageBox = GetComponent<MessageBox>();
            messageBox.Destroy();
            UIManager.Instance.ShowStartupPurchasePanel(
                (Startup)asset,
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
                    "Start the {0} business for {1}? You'll need to prepare a total of {2}.",
                    local.GetBusinessDescription(asset.description),
                    local.GetCurrency(asset.totalCost, true),
                    local.GetCurrency(partialAsset.fundsNeeded));
            }
            return "";
        }
    }
}
