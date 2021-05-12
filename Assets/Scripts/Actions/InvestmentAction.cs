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
        private AbstractInvestment _asset;
        protected PartialInvestment partialAsset { get; private set; }

        public AbstractBuyInvestmentAction(
            Player player,
            AbstractInvestment asset,
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
            partialAsset = new PartialInvestment(
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

    public class PurchaseStartupBusinessAction : AbstractBuyInvestmentAction
    {
        private Business _business;

        public PurchaseStartupBusinessAction(
            Player player,
            Business asset,
            ActionCallback callback) : base(player, asset, callback)
        {
            _business = asset;
        }

        public override void ShowPurchasePanel()
        {
            UIManager.Instance.ShowStartupBusinessPurchasePanel(
                _business,
                partialAsset,
                messageBoxHandler,
                startTransactionHandler,
                false);
        }

        private void nameInputCallback(TransactionHandler handler, string name)
        {
            _business.SetName(name);

            Localization local = Localization.Instance;
            string message = string.Format(
                "After stabilizing business operations, {0} started generating " +
                "a total revenue of {1}",
                local.GetBusinessDescription(name),
                local.GetCurrency(_business.totalIncome));
            UIManager.Instance.ShowSimpleMessageBox(
                message, ButtonChoiceType.OK_ONLY, (_) => handler?.Invoke(true));
        }

        private string confirmMessageCallback(string name)
        {
            Localization local = Localization.Instance;
            return string.Format(
                "Name your business {0}?",
                local.GetBusinessDescription(name));
        }

        private void startNameInput(TransactionHandler handler, bool transactionSuccess)
        {
            if (!transactionSuccess)
            {
                handler?.Invoke(false);
                return;
            }

            Localization local = Localization.Instance;
            string message = string.Format(
                "Choose a name for your {0} business.",
                local.GetBusinessDescription(_business.description));
            UIManager.Instance.ShowSimpleTextPrompt(
                message,
                (string n) => nameInputCallback(handler, n),
                confirmMessageCallback,
                false,
                true);
        }

        private void startTransactionHandler(TransactionHandler handler)
        {
            TransactionManager.BuyBusiness(
                player,
                partialAsset,
                _business,
                (bool b) => startNameInput(handler, b));
        }
    }

    public class JoinFranchiseAction : AbstractBuyInvestmentAction
    {
        private Business _business;

        public JoinFranchiseAction(
            Player player,
            Business asset,
            ActionCallback callback) : base(player, asset, callback)
        {
            _business = asset;
        }

        public override void ShowPurchasePanel()
        {
            UIManager.Instance.ShowFranchiseJoinPanel(
                _business,
                partialAsset,
                messageBoxHandler,
                startTransactionHandler,
                false);
        }

        private void transactionHandler(TransactionHandler handler, bool transactionSuccess)
        {
            if (!transactionSuccess)
            {
                handler?.Invoke(false);
                return;
            }

            Localization local = Localization.Instance;
            string message = string.Format(
                "After stabilizing business operations, your {0} store " +
                "started generating a total revenue of {1}",
                local.GetBusinessDescription(_business.description),
                local.GetCurrency(_business.totalIncome));
            UIManager.Instance.ShowSimpleMessageBox(
                message, ButtonChoiceType.OK_ONLY, (_) => handler?.Invoke(true));
        }

        private void startTransactionHandler(TransactionHandler handler)
        {
            TransactionManager.BuyBusiness(
                player,
                partialAsset,
                _business,
                (bool b) => transactionHandler(handler, b));
        }
    }
}
