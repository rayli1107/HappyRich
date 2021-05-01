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

    public abstract class AbstractBuyInvestmentAction : AbstractAction
    {
        protected Player player { get; private set; }
        private AbstractRealEstate _asset;
        protected PartialRealEstate partialAsset { get; private set; }

        public AbstractBuyInvestmentAction(
            Player player,
            AbstractRealEstate asset,
            ActionCallback callback) : base(callback)
        {
            this.player = player;
            _asset = asset;
        }

        protected void messageBoxHandler(ButtonType buttonType)
        {
            if (buttonType == ButtonType.OK)
            {
                partialAsset.OnPurchase();
                RunCallback(true);
            }
            else
            {
                partialAsset.OnPurchaseCancel();
                RunCallback(false);
            }
        }

        public abstract void ShowPurchasePanel();

        public override void Start()
        {
            partialAsset = new PartialRealEstate(
                _asset,
                player.GetEquityPartners(),
                RealEstateManager.Instance.defaultEquitySplit,
                RealEstateManager.Instance.maxEquityShares);
            ShowPurchasePanel();
        }
    }

    public class BuyRentalRealEstateAction : AbstractBuyInvestmentAction
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
                _rentalAsset,
                partialAsset,
                messageBoxHandler,
                startTransactionHandler,
                false);
        }

        private void startTransactionHandler(TransactionHandler handler)
        {
            TransactionManager.BuyRentalRealEstate(
                player, partialAsset, _rentalAsset, handler);
        }
    }

    public class BuyDistressedRealEstateAction : AbstractBuyInvestmentAction
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
                _distressedAsset,
                partialAsset,
                messageBoxHandler,
                startTransactionHandler,
                false);
        }

        private void startTransactionHandler(TransactionHandler handler)
        {
            TransactionManager.BuyDistressedRealEstate(
                player, partialAsset, _distressedAsset, handler);
        }
    }
}
