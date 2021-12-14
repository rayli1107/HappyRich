using Assets;
using PlayerInfo;
using ScriptableObjects;
using System;
using TMPro;
using UI.Panels.Templates;
using UnityEngine;

namespace UI.Panels.Assets
{
    public class AbstractBusinessPurchasePanel : AbstractInvestmentPanel
    {

        protected VariableIncomeBusiness _business;

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

                /*
                SkillInfo skillInfo = player.GetSkillInfo(SkillType.BUSINESS_OPERATIONS);
                if (skillInfo != null)
                {
                    message += string.Format(
                        _messageBusinessOperationSkill,
                        local.GetSkill(skillInfo));
                }
                */
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
    }
}
