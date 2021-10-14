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

            bool enableEquityPanel = partialAsset.investorShares > 0;
            bool enableDebtPanel = !enableEquityPanel && asset.privateLoan != null;
            bool enableSecuredLoanPanel = !enableEquityPanel && !enableDebtPanel;
            EnableEquityPanel(enableEquityPanel);
            EnablePrivateLoanPanel(enableDebtPanel);
            EnableSecuredLoanPanel(enableSecuredLoanPanel);
        }
    }
}
