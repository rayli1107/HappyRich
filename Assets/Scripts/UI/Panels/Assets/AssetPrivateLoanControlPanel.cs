using Assets;
using PlayerInfo;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Panels.Assets
{
    public class AssetPrivateLoanControlPanel : AssetLoanControlPanel
    {
#pragma warning disable 0649
        [SerializeField]
        private TextMeshProUGUI _textAmount;
        [SerializeField]
        private TextMeshProUGUI _textRate;
        [SerializeField]
        private TextMeshProUGUI _textPayment;
        [SerializeField]
        private Button _buttonRaiseEquity;
#pragma warning restore 0649

        public Player player;
        public AbstractRealEstate asset;

        public Action adjustNumberCallback;
        public Func<bool> checkRaiseEquityCallback;

        public void Refresh()
        {
            Debug.LogFormat("Player {0}", player != null);
            Debug.LogFormat("Asset {0}", asset != null);
            if (asset != null)
            {
                Debug.LogFormat("Asset.PrivateLoan {0}", asset.privateLoan != null);
            }
            if (player == null || asset == null || asset.privateLoan == null)
            {
                return;
            }

            minValue = 0;
            maxValue = asset.privateLoan.maxltv;
            value = asset.privateLoan.ltv;

            if (_textRate != null)
            {
                _textRate.text = string.Format("{0}%", asset.privateLoan.interestRate);
            }

            AdjustNumbers();
        }

        public void OnSliderChange()
        {
            if (asset != null && asset.privateLoan != null)
            {
                asset.privateLoan.ltv = value;
                AdjustNumbers();
            }
        }

        private void AdjustNumbers()
        {
            adjustNumberCallback?.Invoke();

            Localization local = Localization.Instance;

            if (_textAmount != null) {
                _textAmount.text = local.GetCurrency(
                    asset.privateLoan.amount, true);
            }

            if (_textPayment != null)
            {
                _textPayment.text = local.GetCurrency(
                    asset.privateLoan.expense, true);
            }

            if (_buttonRaiseEquity != null)
            {
                _buttonRaiseEquity.gameObject.SetActive(
                    checkRaiseEquityCallback.Invoke());
            }
        }

        private void OnEnable()
        {
            Refresh();
        }
    }
}
