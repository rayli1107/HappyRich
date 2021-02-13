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

            bool enableEquityPanel = partialAsset.shares > 0;
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
    }
}
