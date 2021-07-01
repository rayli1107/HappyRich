using Assets;
using UI.Panels.Templates;

namespace StateMachine
{
    public class SellPropertyState : IState
    {
        private StateMachine _stateMachine;

        public SellPropertyState(StateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }

        private void messageBoxHandler(
            ButtonType buttonType, PlayerInfo.Player player, int index, int finalOffer)
        {
            if (buttonType == ButtonType.OK)
            {
                Localization local = Localization.Instance;
                RentalRealEstate asset = player.portfolio.rentalProperties[index].Item2;
                string description = local.GetRealEstateDescription(asset.description);

                TransactionManager.SellProperty(player, index, finalOffer);

                string message = string.Format(
                    "You've successfully sold the {0} proptery.", description);
                UI.UIManager.Instance.ShowSimpleMessageBox(
                    message,
                    ButtonChoiceType.OK_ONLY,
                    (ButtonType _) => onFinish());
            }
            else
            {
                onFinish();
            }
        }

        private void onFinish()
        {
            _stateMachine.ChangeState(_stateMachine.RefinancePropertyState);
        }

        public void EnterState(StateMachineParameter param)
        {
            PlayerInfo.Player player = GameManager.Instance.player;
            PlayerInfo.Portfolio portfolio = player.portfolio;
            if (portfolio.rentalProperties.Count == 0)
            {
                onFinish();
                return;
            }

            System.Random random = GameManager.Instance.Random;
            if (random.NextDouble() < RealEstateManager.Instance.sellChance)
            {
                int index = random.Next(portfolio.rentalProperties.Count);
                RentalRealEstate asset = portfolio.rentalProperties[index].Item2;
                PartialInvestment partialAsset = portfolio.rentalProperties[index].Item1;
                int initialOffer = RealEstateManager.Instance.calculateOfferPrice(
                    asset.template, random);
                int finalOffer = initialOffer;

                MessageBoxHandler handler =
                    (ButtonType t) => messageBoxHandler(t, player, index, finalOffer);
                UI.UIManager.Instance.ShowRealEstateSalePanel(
                    asset, partialAsset, initialOffer, finalOffer, handler, false);
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
