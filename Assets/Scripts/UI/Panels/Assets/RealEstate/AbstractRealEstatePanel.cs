using Assets;
using PlayerInfo;
using ScriptableObjects;
using System;
using TMPro;
using UI.Panels.Templates;
using UnityEngine;

namespace UI.Panels.Assets
{
    public class AbstractRealEstatePanel : AbstractInvestmentPanel
    {
#pragma warning disable 0649
        [SerializeField]
        protected TextMeshProUGUI _textEstimatedValue;
#pragma warning restore 0649

        protected override void Awake()
        {
            base.Awake();

            MessageBox messageBox = GetComponent<MessageBox>();
            messageBox.confirmMessageHandler = GetConfirmMessage;
            messageBox.messageBoxHandler =
                b => messageBoxHandler(b, messageBox.messageBoxHandler);
        }

        public override void Refresh()
        {
            if (player == null || asset == null)
            {
                return;
            }
            base.Refresh();

            Localization local = Localization.Instance;
            if (_textMessage != null)
            {
                _textMessage.text = string.Format(
                    _messageTemplate,
                    local.GetRealEstateDescription(asset.description));
            }

            if (_textEstimatedValue != null)
            {
                if (player.HasSkill(SkillType.REAL_ESTATE_VALUATION))
                {
                    _textEstimatedValue.text = local.GetCurrencyPlain(
                        ((AbstractRealEstate)asset).template.basePrice);
                }
                else
                {
                    _textEstimatedValue.text = "???";
                }
            }

            if (_securedLoanControlPanel != null)
            {
                EnableSecuredLoanPanel(true);
            }

            bool enableEquityPanel = partialAsset.investorShares > 0;
            bool enableDebtPanel = !enableEquityPanel && asset.privateLoan != null;
            EnableEquityPanel(enableEquityPanel);
            EnablePrivateLoanPanel(enableDebtPanel);
        }

        private string GetConfirmMessage(ButtonType buttonType)
        {
            Localization local = Localization.Instance;
            if (buttonType == ButtonType.OK)
            {
                return string.Format(
                    "Buy the {0} for a total of {1}? You'll need to prepare a " +
                    "total of {2} to purchase the property.",
                    local.GetRealEstateDescription(asset.description),
                    local.GetCurrency(asset.totalCost),
                    local.GetCurrency(partialAsset.fundsNeeded));
            }
            return "";
        }

        private void messageBoxHandler(
            ButtonType buttonType, MessageBoxHandler oldHandler)
        {
            Debug.LogFormat("AbstractRealEstatePanel messageBoxHandler");
            if (buttonType == ButtonType.OK)
            {
                Localization local = Localization.Instance;
                string message = string.Format(
                    "Congratulations! You've purchased {0} property!",
                    local.GetRealEstateDescription(asset.description));
                UIManager.Instance.ShowSimpleMessageBox(
                    message,
                    ButtonChoiceType.OK_ONLY,
                    oldHandler,
                    () => partialAsset.OnDetail(player, null));
            }
            else
            {
                oldHandler?.Invoke(buttonType);
            }

        }
    }
}
