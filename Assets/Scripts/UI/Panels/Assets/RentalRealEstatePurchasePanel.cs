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

        public override void Refresh()
        {
            base.Refresh();

            if (player == null || asset == null)
            {
                return;
            }

            if (_mortgageControlPanel != null)
            {
                EnableMortgagePanel(true);
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

        public void OnSwitchViewButton(bool advanced)
        {
            MessageBox messageBox = GetComponent<MessageBox>();
            messageBox.Destroy();
            UIManager.Instance.ShowRentalRealEstatePurchasePanel(
                asset, partialAsset, messageBox.messageBoxHandler, advanced);
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
            else
            {
                return string.Format(
                    "Pass on buying the {0}?",
                    local.GetRealEstateDescription(asset.description));
            }
        }
    }
}
