using System.Collections.Generic;
using UI;
using UI.Panels.Templates;

namespace Actions
{
    public class TakePersonalLoan : IAction, IMessageBoxHandler
    {
        private Player _player;
        private int _amount;
        private ITransactionHandler _transactionHandler;

        public TakePersonalLoan(
            Player player,
            int amount,
            ITransactionHandler transactionHandler)
        {
            _player = player;
            _amount = amount;
            _transactionHandler = transactionHandler;
        }
        public void OnButtonClick(ButtonType button)
        {
            bool success = false;
            if (button == ButtonType.OK)
            {
                int loanAmount = _amount - _player.cash;
                _player.portfolio.AddCash(-1 * _player.cash);
                _player.portfolio.AddPersonalLoan(loanAmount);
                success = true;
            }
            _transactionHandler.OnTransactionFinish(success);
        }

        public void Start()
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
                string.Join("\n", message), ButtonChoiceType.OK_CANCEL, this);
        }
    }
}
