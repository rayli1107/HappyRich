﻿using Assets;
using Transaction;
using UI.Panels;
using UI.Panels.Templates;
using UnityEngine;

namespace Actions
{
    public class BuyStocksAction
        : AbstractAction, INumberInputCallback, ITransactionHandler, IMessageBoxHandler
    {
        private Player _player;
        private AbstractStock _stock;
        private int _numPrurchased;

        public BuyStocksAction(
            Player player, AbstractStock stock, IActionCallback callback)
            : base(callback)
        {
            _player = player;
            _stock = stock;
        }

        public override void Start()
        {
            int max = _player.cash / _stock.value;
            string message = string.Format(
                "How many shares of {0} do you want to buy?",
                _stock.name);
            UI.UIManager.Instance.ShowNumberInputPanel(
                message, max, this);
        }

        public void OnNumberInput(int number)
        {
            int cost = number * _stock.value;
            _numPrurchased = number;

            Localization local = Localization.Instance;
            string message = string.Format(
                "Purchase {0} share{1} of {2} for {3}?",
                number, number > 1 ? "s" : "", _stock.name,
                local.GetCurrency(cost, true));
            UI.UIManager.Instance.ShowSimpleMessageBox(
                message, ButtonChoiceType.OK_CANCEL, this);
        }

        public void OnNumberInputCancel()
        {
            RunCallback(false);
        }

        public void OnButtonClick(MessageBox msgBox, ButtonType button)
        {
            if (button == ButtonType.OK)
            {
                int cost = _numPrurchased * _stock.value;
                GameManager.Instance.TryDebit(_player, cost, this);
            }
            else
            {
                RunCallback(false);
            }
        }

        public void OnTransactionSuccess()
        {
            _player.portfolio.AddStock(_stock, _numPrurchased);
            UI.UIManager.Instance.UpdatePlayerInfo(_player);
            RunCallback(true);
        }

        public void OnTransactionFailure()
        {
            RunCallback(false);
        }
    }
}
