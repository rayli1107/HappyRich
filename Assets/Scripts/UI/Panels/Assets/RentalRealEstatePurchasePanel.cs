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
#pragma warning restore 0649

        private void Awake()
        {
            if (_mortgageControlPanel != null)
            {
                _mortgageControlPanel.adjustNumberCallback = AdjustNumbers;
                _mortgageControlPanel.checkRaiseDebtCallback = () => false;
                _mortgageControlPanel.checkRaiseEquityCallback = () => false;
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
