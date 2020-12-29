using System.Collections.Generic;
using UI;
using UI.Panels.Templates;

namespace Actions
{
    public class TakePersonalLoan : AbstractAction
    {
        private Player _player;
        private int _amount;
        private TransactionHandler _transactionHandler;

        public TakePersonalLoan(
            Player player,
            int amount,
            TransactionHandler transactionHandler)
        {
            _player = player;
            _amount = amount;
            _transactionHandler = transactionHandler;
        }
        private void messageBoxHandler(ButtonType button)
        {
            bool success = false;
            if (button == ButtonType.OK)
            {
                int loanAmount = _amount - _player.cash;
                _player.portfolio.AddCash(-1 * _player.cash);
                _player.portfolio.AddPersonalLoan(loanAmount);
                success = true;
            }
            _transactionHandler?.Invoke(success);
        }

        public override void Start()
        {
            Localization local = Localization.Instance;
            int loanAmount = _amount - _player.cash;
            int interst = loanAmount * InterestRateManager.Instance.personalLoanRate / 100;
            List<string> message = new List<string>();
            message.Add(string.Format("Take out a personal loan of {0}?",
                local.GetCurrency(loanAmount, true)));
            message.Add(string.Format("You will need to pay an additional annual interest of {0}",
                local.GetCurrency(interst, true)));
            UI.UIManager.Instance.ShowSimpleMessageBox(
                string.Join("\n", message), ButtonChoiceType.OK_CANCEL, messageBoxHandler);
        }
    }
}
