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
#pragma warning disable 0649
        [SerializeField]
        private TextMeshProUGUI _textIncomeRange;
#pragma warning restore 0649

        protected AbstractBusiness _business;

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


        private int calculateIncome(int income)
        {
            int loanPayment = asset.combinedLiability.expense;
            income -= loanPayment;
            int investorCashflow = Mathf.FloorToInt(partialAsset.investorEquity * income);
            return income - investorCashflow;
        }
    
        protected override void AdjustNumbers()
        {
            base.AdjustNumbers();

            Localization local = Localization.Instance;

            if (_textIncomeRange != null)
            {
                int minIncome = calculateIncome(_business.minIncome);
                int maxIncome = calculateIncome(_business.maxIncome);
                _textIncomeRange.text = string.Format(
                    "{0} - {1}",
                    local.GetCurrency(minIncome),
                    local.GetCurrency(maxIncome));
            }
        }
    }
}
