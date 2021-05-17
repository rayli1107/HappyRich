using Assets;
using PlayerInfo;
using UI.Panels.Templates;

namespace Actions
{
    public class SellStocksAction : AbstractAction
    {
        private Player _player;
        private AbstractStock _stock;
        private int _numSold;

        public SellStocksAction(
            Player player, AbstractStock stock, ActionCallback callback)
            : base(callback)
        {
            _player = player;
            _stock = stock;
        }

        private string confirmMessageHandler(int number)
        {
            Localization local = Localization.Instance;
            int cost = number * _stock.value;
            return string.Format(
                "Sell {0} share{1} of {2} for {3}?",
                number, number > 1 ? "s" : "", _stock.name,
                local.GetCurrency(cost));
        }

        private void startTransactionHandler(int number, TransactionHandler handler)
        {
            TransactionManager.SellStock(_player, _stock, number, handler);
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
                message,
                max,
                onNumberInput,
                () => RunCallback(false),
                confirmMessageHandler,
                startTransactionHandler);
        }

        private void onNumberInput(int number)
        {
            UI.UIManager.Instance.UpdatePlayerInfo(_player);
            RunCallback(true);
        }
    }
}
