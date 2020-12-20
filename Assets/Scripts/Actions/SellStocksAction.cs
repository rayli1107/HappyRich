using Assets;
using UI;
using UI.Panels.Templates;

namespace Actions
{
    public class SellStocksAction : AbstractAction, INumberInputCallback, IMessageBoxHandler, ITransactionHandler
    {
        private Player _player;
        private AbstractStock _stock;
        private int _numSold;

        public SellStocksAction(
            Player player, AbstractStock stock, IActionCallback callback)
            : base(callback)
        {
            _player = player;
            _stock = stock;
        }

        public override void Start()
        {
            PurchasedStock purchasedStock;

            int max = 0;
            if (_player.portfolio.stocks.TryGetValue(_stock.name, out purchasedStock))
            {
                max = purchasedStock.count;
            }
            if (max == 0)
            {
                RunCallback(false);
                return;
            }

            string message = string.Format(
                "How many shares of {0} do you want to sell?",
                _stock.name);
            UI.UIManager.Instance.ShowNumberInputPanel(
                message, max, this);
        }

        public void OnNumberInput(int number)
        {
            int cost = number * _stock.value;
            _numSold = number;

            Localization local = Localization.Instance;
            string message = string.Format(
                "Sell {0} share{1} of {2} for {3}?",
                number, number > 1 ? "s" : "", _stock.name,
                local.GetCurrency(cost));
            UI.UIManager.Instance.ShowSimpleMessageBox(
                message, ButtonChoiceType.OK_CANCEL, this);
        }

        public void OnNumberInputCancel()
        {
            RunCallback(false);
        }

        public void OnButtonClick(ButtonType button)
        {
            if (button == ButtonType.OK)
            {
                TransactionManager.SellStock(_player, _stock, _numSold, this);
            }
            else
            {
                OnTransactionFinish(false);
            }
        }

        public void OnTransactionFinish(bool success)
        {
            if (success)
            {
                UI.UIManager.Instance.UpdatePlayerInfo(_player);
            }
            RunCallback(success);
        }
    }
}
