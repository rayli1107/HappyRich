using Assets;
using PlayerInfo;
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

        public InvestmentAction(
            Player player,
            AbstractRealEstate asset,
            ActionCallback callback) : base(callback)
        {
            this.player = player;
            _asset = asset;
        }

        protected abstract void BuyProperty();

        protected void OnPurchasePanelButtonClick(ButtonType button)
        {
            if (button == ButtonType.OK)
            {
                BuyProperty();
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
                player.GetEquityPartners(),
                RealEstateManager.Instance.defaultEquitySplit,
                RealEstateManager.Instance.maxEquityShares);
            ShowPurchasePanel();
        }

        protected void onTransactionFinish(bool success)
        {
            if (success)
            {
                partialAsset.OnPurchase();
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
            Player player,
            RentalRealEstate asset,
            ActionCallback callback) : base(player, asset, callback)
        {
            _rentalAsset = asset;
        }

        public override void ShowPurchasePanel()
        {
            UIManager.Instance.ShowRentalRealEstatePurchasePanel(
                _rentalAsset, partialAsset, OnPurchasePanelButtonClick, false);
        }

        protected override void BuyProperty()
        {
            TransactionManager.BuyRentalRealEstate(
                player, partialAsset, _rentalAsset, onTransactionFinish);
        }
    }

    public class BuyDistressedRealEstateAction : InvestmentAction
    {
        private DistressedRealEstate _distressedAsset;

        public BuyDistressedRealEstateAction(
            Player player,
            DistressedRealEstate asset,
            ActionCallback callback) : base(player, asset, callback)
        {
            _distressedAsset = asset;
        }

        public override void ShowPurchasePanel()
        {
            UIManager.Instance.ShowDistressedRealEstatePurchasePanel(
                _distressedAsset, partialAsset, OnPurchasePanelButtonClick, false);
        }

        protected override void BuyProperty()
        {
            TransactionManager.BuyDistressedRealEstate(
                player, partialAsset, _distressedAsset, onTransactionFinish);
        }
    }
}
