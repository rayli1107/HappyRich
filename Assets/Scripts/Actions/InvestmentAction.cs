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



    public abstract class InvestmentAction : AbstractAction
    {
        protected Player player { get; private set; }
        private AbstractRealEstate _asset;
        protected PartialRealEstate partialAsset { get; private set; }

        public InvestmentAction(Player player, AbstractRealEstate asset) : base(null)
        {
            this.player = player;
            _asset = asset;
        }

        protected void OnPurchasePanelButtonClick(ButtonType button)
        {
            if (button == ButtonType.OK)
            {
                TransactionManager.BuyRealEstate(
                    player, partialAsset, onTransactionFinish);
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
                partialAsset.OnPurchaseCancel();
                GameManager.Instance.StateMachine.OnPlayerActionDone();
                RunCallback(false);
            }
            else
            {
                ShowPurchasePanel();
            }
        }

        public abstract void ShowPurchasePanel();

        public void ShowCancelConfirmPanel()
        {
            UI.UIManager.Instance.ShowSimpleMessageBox(
                "Are you sure you don't want to purchase the property?",
                ButtonChoiceType.OK_CANCEL,
                OnCancelConfirmButtonClick);
        }

        public override void Start()
        {
            partialAsset = new PartialRealEstate(
                _asset,
                RealEstateManager.Instance.defaultEquitySplit,
                RealEstateManager.Instance.defaultEquityPerShare);
            ShowPurchasePanel();
        }

        private void onTransactionFinish(bool success)
        {
            if (success)
            {
                partialAsset.OnPurchase();
                UIManager.Instance.UpdatePlayerInfo(player);
                GameManager.Instance.StateMachine.OnPlayerActionDone();
                RunCallback(true);
            }
            else
            {
                ShowPurchasePanel();
            }
        }
    }

    public class BuyRentalRealEstateAction : InvestmentAction
    {
        private RentalRealEstate _rentalAsset;

        public BuyRentalRealEstateAction(
            Player player, RentalRealEstate asset) : base(player, asset)
        {
            _rentalAsset = asset;
        }

        public override void ShowPurchasePanel()
        {
            UIManager.Instance.ShowRentalRealEstatePurchasePanel(
                _rentalAsset, partialAsset, OnPurchasePanelButtonClick, false);
        }
    }

    public class BuyDistressedRealEstateAction : InvestmentAction
    {
        private DistressedRealEstate _distressedAsset;

        public BuyDistressedRealEstateAction(
            Player player, DistressedRealEstate asset) : base(player, asset)
        {
            _distressedAsset = asset;
        }

        public override void ShowPurchasePanel()
        {
            UIManager.Instance.ShowDistressedRealEstatePurchasePanel(
                _distressedAsset, partialAsset, OnPurchasePanelButtonClick, false);
        }
    }
}
