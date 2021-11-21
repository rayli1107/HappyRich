using Assets;
using PlayerInfo;
using System;
using TMPro;
using UI.Panels.Templates;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Panels
{
    public class RaisePersonalFundsMessageBox : ModalObject
    {
#pragma warning disable 0649
        [SerializeField]
        private TextMeshProUGUI _textMessage;
        [SerializeField]
        private Button _buttonPersonalLoan;
        [SerializeField]
        private Button _buttonLiquidateAssets;
#pragma warning restore 0649

        public Player player;
        public int amount;
        public Action<bool> callback;

        private int getMaxLoanAmount()
        {
            return new Snapshot(player).availablePersonalLoanAmount;
        }

        private void raiseSuccessfulHandler()
        {
            Destroy();
            callback?.Invoke(true);
        }

        private void raiseFailedHandler()
        {
            Destroy();
            callback?.Invoke(false);
        }

        public void Refresh()
        {
            Localization local = Localization.Instance;
            if (player == null)
            {
                return;
            }
            
            if (player.portfolio.cash >= amount)
            {
                UIManager.Instance.ShowSimpleMessageBox(
                    "You've managed to raise enough money to pay off the required amount.",
                    ButtonChoiceType.OK_ONLY,
                    _ => raiseSuccessfulHandler());
                return;
            }

            int maxLoanAmount = getMaxLoanAmount();
            int liquidAsset = 0;
            foreach (AbstractAsset asset in player.portfolio.liquidAssets)
            {
                liquidAsset += asset.value;
            }

            if (player.portfolio.cash + maxLoanAmount + liquidAsset < amount)
            {
                string message = string.Format(
                    "Unfortunately you don't have a way to come with the {0} needed. " +
                    "You only have {1} in available cash and {2} in liquid assets, and " +
                    "the maximum amount of personal you can take out is {3}.",
                    local.GetCurrency(amount),
                    local.GetCurrency(player.portfolio.cash),
                    local.GetCurrency(liquidAsset),
                    local.GetCurrency(maxLoanAmount));
                UIManager.Instance.ShowSimpleMessageBox(
                    message, ButtonChoiceType.OK_ONLY, _ => raiseFailedHandler());
                return;
            }

            _textMessage.text = string.Format(
                "You need to come up with {0} but you only have {1} in available cash. " +
                "You also have {2} in liquid assets, and the maximum of amount of " +
                "personal loan you can take out is {3}. How do you want to proceed?",
                local.GetCurrency(amount),
                local.GetCurrency(player.portfolio.cash),
                local.GetCurrency(liquidAsset),
                local.GetCurrency(maxLoanAmount));
            _buttonPersonalLoan.gameObject.SetActive(maxLoanAmount > 0);
            _buttonLiquidateAssets.gameObject.SetActive(liquidAsset > 0);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Refresh();
        }

        private void personalLoanHandler(int loanAmount, ButtonType button)
        {
            if (button == ButtonType.OK)
            {
                player.portfolio.AddPersonalLoan(loanAmount);
                player.portfolio.AddCash(loanAmount);
            }
            Refresh();
        }

        public void OnTakePersonalLoan()
        {
            int loanAmount = Mathf.Min(
                amount - player.portfolio.cash, getMaxLoanAmount());
            int interest = loanAmount * InterestRateManager.Instance.personalLoanRate / 100;
            Localization local = Localization.Instance;
            string message = string.Format(
                "Take out a personal loan of {0}? You will need to pay an additional " +
                "annual interest of {1}.",
                local.GetCurrency(loanAmount),
                local.GetCurrency(interest, true));
            UIManager.Instance.ShowSimpleMessageBox(
                message,
                ButtonChoiceType.OK_CANCEL,
                b => personalLoanHandler(loanAmount, b));
        }

        public void OnLiquidateAssets()
        {
            UIManager.Instance.ShowStockMarketPanel(_ => Refresh());
        }

        public void OnCancel()
        {
            raiseFailedHandler();
        }
    }
}
