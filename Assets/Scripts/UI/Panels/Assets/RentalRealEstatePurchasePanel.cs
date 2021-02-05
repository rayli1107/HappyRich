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
#pragma warning disable 0649
        [SerializeField]
        private AssetMortgageControlPanel _mortgageControlPanel;
        [SerializeField]
        private AssetPrivateLoanControlPanel _privateLoanControlPanel;
        [SerializeField]
        private AssetEquityControlPanel _equityControlPanel;
#pragma warning restore 0649

        private bool checkRaiseDebt()
        {
            int maxltv = RealEstateManager.Instance.maxPrivateLoanLTV;
            int rate = InterestRateManager.Instance.defaultPrivateLoanRate;
            RealEstatePrivateLoan loan = new RealEstatePrivateLoan(
                asset, player.GetDebtPartners(), maxltv, rate, false);
            return loan.maxltv > 0;
        }

        private bool checkRaiseEquity()
        {
            return partialAsset.maxShares > 0;
        }

        private void Awake()
        {
            if (_mortgageControlPanel != null)
            {
                _mortgageControlPanel.adjustNumberCallback = AdjustNumbers;
                _mortgageControlPanel.checkRaiseDebtCallback = checkRaiseDebt;
                _mortgageControlPanel.checkRaiseEquityCallback = checkRaiseEquity;
            }

            if (_privateLoanControlPanel != null)
            {
                _privateLoanControlPanel.adjustNumberCallback = AdjustNumbers;
                _privateLoanControlPanel.checkRaiseEquityCallback = checkRaiseEquity;
            }

            if (_equityControlPanel != null)
            {
                _equityControlPanel.adjustNumberCallback = AdjustNumbers;
            }
        }

        private void EnablePrivateLoanPanel(bool enable)
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

        private void EnableEquityPanel(bool enable)
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

        public override void Refresh()
        {
            base.Refresh();

            if (player == null || asset == null)
            {
                return;
            }

            if (_mortgageControlPanel != null)
            {
                _mortgageControlPanel.player = player;
                _mortgageControlPanel.asset = asset;
                _mortgageControlPanel.gameObject.SetActive(true);
                _mortgageControlPanel.Refresh();
            }

            bool enableEquityPanel = partialAsset.shares > 0;
            bool enableDebtPanel = asset.privateLoan != null;
            Debug.LogFormat("enableEquityPanel {0}", enableEquityPanel);
            Debug.LogFormat("enableDebtPanel {0}", enableDebtPanel);

            if (_equityControlPanel != null)
            {
                EnableEquityPanel(enableEquityPanel);
            }

            if (_privateLoanControlPanel != null && !enableEquityPanel)
            {
                EnablePrivateLoanPanel(enableDebtPanel);
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

        public void OnSwitchViewButton(bool advanced)
        {
            MessageBox messageBox = GetComponent<MessageBox>();
            gameObject.SetActive(false);
            messageBox.Destroy();
            UIManager.Instance.ShowRentalRealEstatePurchasePanel(
                asset, partialAsset, messageBox.messageBoxHandler, advanced);
        }
    }
}
