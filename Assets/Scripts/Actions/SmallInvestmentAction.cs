using Assets;
using UI;
using UI.Panels.Templates;

namespace Actions
{
    public enum InvestmentState
    {
        kStart,
        kPurchasing,
        kCancelling
    }

    public class SmallInvestmentAction : AbstractAction, IMessageBoxHandler, ITransactionHandler
    {
        private Player _player;
        private InvestmentState _state;
        private AbstractRealEstate _asset;
        private PartialRealEstate _partialAsset;

        public SmallInvestmentAction(Player player) : base(null)
        {
            _player = player;
            _state = InvestmentState.kStart;
        }

        private void OnPurchasePanelButtonClick(ButtonType button)
        {
            if (button == ButtonType.OK)
            {
                TransactionManager.BuyRealEstate(_player, _asset, this);
            }
            else
            {
                ShowCancelConfirmPanel();
            }
        }

        private void OnCancelConfirmButtonClick(ButtonType button)
        {
            if (button == ButtonType.OK)
            {
                _asset.OnPurchaseCancel();
                GameManager.Instance.StateMachine.OnPlayerActionDone();
                RunCallback(false);
            }
            else
            {
                ShowPurchasePanel();
            }
        }

        public void OnButtonClick(ButtonType button)
        {
            switch (_state)
            {
                case InvestmentState.kPurchasing:
                    OnPurchasePanelButtonClick(button);
                    break;
                case InvestmentState.kCancelling:
                    OnCancelConfirmButtonClick(button);
                    break;
                default:
                    break;
            }
        }

        public void ShowPurchasePanel()
        {
            _state = InvestmentState.kPurchasing;
            UIManager.Instance.ShowRentalRealEstatePurchasePanel(
                (RentalRealEstate)_asset, _partialAsset, this);
        }

        public void ShowCancelConfirmPanel()
        {
            _state = InvestmentState.kCancelling;
            UI.UIManager.Instance.ShowSimpleMessageBox(
                "Are you sure you don't want to purchase the property?",
                ButtonChoiceType.OK_CANCEL,
                this);
        }

        public override void Start()
        {
            System.Random random = GameManager.Instance.Random;
            RealEstateManager manager = GameManager.Instance.RealEstateManager;
            _asset = manager.GetSmallInvestment(random);
            _partialAsset = new PartialRealEstate(
                _asset, RealEstateManager.Instance.defaultEquitySplit);
            ShowPurchasePanel();
        }

        public void OnTransactionFinish(bool success)
        {
            if (success)
            {
                _asset.OnPurchase();
                UIManager.Instance.UpdatePlayerInfo(_player);
                GameManager.Instance.StateMachine.OnPlayerActionDone();
                RunCallback(true);
            }
            else
            {
                ShowPurchasePanel();
            }
        }
    }
}
