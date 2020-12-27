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

    public class SmallInvestmentAction : AbstractAction, ITransactionHandler
    {
        private Player _player;
        private AbstractRealEstate _asset;
        private PartialRealEstate _partialAsset;

        public SmallInvestmentAction(Player player) : base(null)
        {
            _player = player;
        }

        private void OnPurchasePanelButtonClick(ButtonType button)
        {
            if (button == ButtonType.OK)
            {
                TransactionManager.BuyRealEstate(_player, _partialAsset, this);
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
                _partialAsset.OnPurchaseCancel();
                GameManager.Instance.StateMachine.OnPlayerActionDone();
                RunCallback(false);
            }
            else
            {
                ShowPurchasePanel();
            }
        }

        public void ShowPurchasePanel()
        {
            UIManager.Instance.ShowRentalRealEstatePurchasePanel(
                (RentalRealEstate)_asset, _partialAsset, OnPurchasePanelButtonClick, false);
        }

        public void ShowCancelConfirmPanel()
        {
            UI.UIManager.Instance.ShowSimpleMessageBox(
                "Are you sure you don't want to purchase the property?",
                ButtonChoiceType.OK_CANCEL,
                OnCancelConfirmButtonClick);
        }

        public override void Start()
        {
            System.Random random = GameManager.Instance.Random;
            RealEstateManager manager = GameManager.Instance.RealEstateManager;
            _asset = manager.GetSmallInvestment(random);
            _partialAsset = new PartialRealEstate(
                _asset,
                RealEstateManager.Instance.defaultEquitySplit,
                RealEstateManager.Instance.defaultEquityPerShare);
            ShowPurchasePanel();
        }

        public void OnTransactionFinish(bool success)
        {
            if (success)
            {
                _partialAsset.OnPurchase();
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
