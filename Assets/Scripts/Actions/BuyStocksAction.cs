using Assets;
using PlayerInfo;
using UI.Panels.Templates;

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

        private string confirmMessageHandler(ButtonType buttonType, int number)
        {
            if (buttonType == ButtonType.OK)
            {
                int cost = number * _stock.value;

                Localization local = Localization.Instance;
                return string.Format(
                    "Purchase {0} share{1} of {2} for {3}?",
                    number, number > 1 ? "s" : "", _stock.name,
                    local.GetCurrency(cost, true));
            }
            else
            {
                return null;
            }
        }

        private void startTransactionHandler(TransactionHandler handler, int number)
        {
            TransactionManager.BuyStock(_player, _stock, number, handler);
        }

        public override void Start()
        {
            int max = _player.cash / _stock.value;
            string message = string.Format(
                "How many shares of {0} do you want to buy?\nMax: {1}",
                _stock.name,
                max);
            UI.UIManager.Instance.ShowNumberInputPanel(
                message,
                max,
                messageBoxHandler,
                confirmMessageHandler,
                startTransactionHandler);
        }

        private void messageBoxHandler(ButtonType buttonType, int number)
        {
            UI.UIManager.Instance.UpdatePlayerInfo(_player);
            RunCallback(buttonType == ButtonType.OK);
        }
    }
}
