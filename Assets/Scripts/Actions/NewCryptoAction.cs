using Assets;
using PlayerInfo;
using PlayerState;
using UI.Panels.Templates;

namespace Actions
{
    public class NewCryptoAction : AbstractAction
    {
        private Player _player;

        public NewCryptoAction(Player player, ActionCallback callback) : base(callback)
        {
            _player = player;
        }

        public override void Start()
        {
            StockManager manager = StockManager.Instance;
            if (manager.cryptoCurrencies.Count < manager.numCryptoCurrencies)
            {
                System.Tuple<string, AbstractStock> crypto = manager.CreateNewCryptoCurrency(
                    GameManager.Instance.Random);

                string message = string.Format(
                    "A new cryptocurrency {0} has just been launched!",
                    Localization.Instance.GetStockName(crypto.Item2));
                UI.UIManager.Instance.ShowSimpleMessageBox(
                    message, ButtonChoiceType.OK_ONLY, (_) => RunCallback(true));
            }
            else
            {
                RunCallback(true);
            }
        }
    }
}
