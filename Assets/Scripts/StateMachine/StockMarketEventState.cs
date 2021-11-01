using Assets;
using System.Collections.Generic;

namespace StateMachine
{
    public class StockMarketEventState : IState
    {
        private StateMachine _stateMachine;

        public StockMarketEventState(StateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public void EnterState(StateMachineParameter param)
        {
            StockManager.Instance.OnTurnStart(GameManager.Instance.Random);
            checkCryptoDelisted(0);
        }

        private void checkCryptoDelisted(int index)
        {
            List<AbstractCryptoCurrency> cryptos = StockManager.Instance.cryptoCurrencies;
            if (index >= cryptos.Count)
            {
                _stateMachine.ChangeState(_stateMachine.SellPropertyState);
                return;
            }

            AbstractStock crypto = cryptos[index];
            if (crypto.prevValue > 0 && crypto.value == 0)
            {
                cryptos.RemoveAt(index);
                Localization local = Localization.Instance;
                string message = string.Format(
                    "The cryptocurrency {0} has been delisted from the market.",
                    local.GetStockName(crypto));
                UI.UIManager.Instance.ShowSimpleMessageBox(
                    message,
                    UI.Panels.Templates.ButtonChoiceType.OK_ONLY,
                    (_) => checkCryptoDelisted(index));
            }
            else
            {
                checkCryptoDelisted(index + 1);
            }
        }

        public void ExitState()
        {
        }

        public void Update()
        {
        }
    }
}
