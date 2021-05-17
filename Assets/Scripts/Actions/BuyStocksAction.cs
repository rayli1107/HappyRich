using Assets;
using PlayerInfo;

namespace Actions
{
    public class BuyStocksAction : AbstractAction
    {
        private Player _player;
        private AbstractStock _stock;

        public BuyStocksAction(Player player, AbstractStock stock, ActionCallback callback)
            : base(callback)
        {
            _player = player;
            _stock = stock;
        }

        private string confirmMessageHandler(int number)
        {
            int cost = number * _stock.value;

            Localization local = Localization.Instance;
            return string.Format(
                "Purchase {0} share{1} of {2} for {3}?",
                number, number > 1 ? "s" : "", _stock.name,
                local.GetCurrency(cost, true));
        }

        private void startTransactionHandler(int number, TransactionHandler handler)
        {
            TransactionManager.BuyStock(_player, _stock, number, handler);
        }

        public override void Start()
        {
            int max = _player.cash / _stock.value;
            string message = string.Format(
                "How many shares of {0} do you want to buy?",
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
