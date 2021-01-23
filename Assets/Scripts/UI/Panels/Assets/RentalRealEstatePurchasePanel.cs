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
        private RectTransform _debtSummaryPanel;
        [SerializeField]
        private RectTransform _equitySummaryPanel;
        [SerializeField]
        private AssetMortgageControlPanel _mortgageControlPanel;
        [SerializeField]
        private AssetPrivateLoanControlPanel _privateLoanControlPanel;
#pragma warning restore 0649

        private bool checkRaiseDebt()
        {
            int maxltv = RealEstateManager.Instance.maxPrivateLoanLTV;
            int rate = InterestRateManager.Instance.defaultPrivateLoanRate;
            RealEstatePrivateLoan loan = new RealEstatePrivateLoan(
                asset, player.GetDebtPartners(), maxltv, rate, false);
            return loan.maxltv > 0;
        }

        private void Awake()
        {
            if (_mortgageControlPanel != null)
            {
                _mortgageControlPanel.adjustNumberCallback = AdjustNumbers;
                _mortgageControlPanel.checkRaiseDebtCallback = checkRaiseDebt;
                _mortgageControlPanel.checkRaiseEquityCallback = () => false;
            }

            if (_privateLoanControlPanel != null)
            {
                _privateLoanControlPanel.adjustNumberCallback = AdjustNumbers;
                _privateLoanControlPanel.checkRaiseEquityCallback = () => false;
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

            if (_privateLoanControlPanel != null)
            {
                _mortgageControlPanel.player = player;
                _mortgageControlPanel.asset = asset;
                _privateLoanControlPanel.gameObject.SetActive(true);
                _privateLoanControlPanel.Refresh();
            }
            /*
                        if (_debtSummaryPanel != null)
                        {
                            _debtSummaryPanel.gameObject.SetActive(asset.privateLoanAmount > 0);
                        }

                        if (_equitySummaryPanel != null)
                        {
                            _equitySummaryPanel.gameObject.SetActive(partialAsset.investorShares > 0);
                        }
                        */
        }

/*
        public void OnRaiseDebtButton()
        {
            _debtSummaryPanel.gameObject.SetActive(true);
        }

        public void OnRaiseEquityButton()
        {
            _equitySummaryPanel.gameObject.SetActive(true);
        }
        */

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
