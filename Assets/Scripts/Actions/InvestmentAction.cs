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

    public class InvestmentAction : AbstractAction
    {
        private Player _player;
        private AbstractRealEstate _asset;
        private PartialRealEstate _partialAsset;

        public InvestmentAction(Player player, AbstractRealEstate asset) : base(null)
        {
            _player = player;
            _asset = asset;
        }

        private void OnPurchasePanelButtonClick(ButtonType button)
        {
            if (button == ButtonType.OK)
            {
                TransactionManager.BuyRealEstate(
                    _player, _partialAsset, onTransactionFinish);
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
            RealEstateManager manager = GameManager.Instance.RealEstateManager;
            _partialAsset = new PartialRealEstate(
                _asset,
                RealEstateManager.Instance.defaultEquitySplit,
                RealEstateManager.Instance.defaultEquityPerShare);
            ShowPurchasePanel();
        }

        private void onTransactionFinish(bool success)
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
        public static InvestmentAction GetSmallInvestmentAction(Player player)
        {
            System.Random random = GameManager.Instance.Random;
            AbstractRealEstate asset = RealEstateManager.Instance.GetSmallInvestment(
                random);
            return new InvestmentAction(player, asset);
        }

        public static InvestmentAction GetLargeInvestmentAction(Player player)
        {
            System.Random random = GameManager.Instance.Random;
            AbstractRealEstate asset = RealEstateManager.Instance.GetLargeInvestment(
                random);
            return new InvestmentAction(player, asset);
        }
    }
}
