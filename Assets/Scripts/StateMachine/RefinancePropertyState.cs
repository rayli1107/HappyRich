using Assets;

namespace StateMachine
{
    public class RefinancePropertyState : IState
    {
        private StateMachine _stateMachine;

        public RefinancePropertyState(StateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }

        private void handleRefinancePanelResult(
            int index,
            PartialInvestment partialAsset,
            RefinancedRealEstate refinancedAsset) {
            TransactionManager.RefinanceProperty(
                GameManager.Instance.player,
                partialAsset,
                refinancedAsset);
            processDistressedProperty(index + 1);
        }

        private void processDistressedProperty(int index)
        {
            PlayerInfo.Player player = GameManager.Instance.player;
            PlayerInfo.Portfolio portfolio = player.portfolio;
            if (index >= portfolio.distressedProperties.Count)
            {
                portfolio.distressedProperties.Clear();
                _stateMachine.ChangeState(_stateMachine.PlayerActionState);
                return;
            }

            PartialInvestment partialAsset = portfolio.distressedProperties[index].Item1;
            DistressedRealEstate distressedAsset = portfolio.distressedProperties[index].Item2;
            RefinancedRealEstate refinancedAsset =
                RealEstateManager.Instance.RefinanceDistressedProperty(player, distressedAsset);
            UI.UIManager.Instance.ShowRentalRealEstateRefinancePanel(
                refinancedAsset,
                partialAsset,
                (_) => handleRefinancePanelResult(index, partialAsset, refinancedAsset),
                false);
        }

        public void EnterState(StateMachineParameter param)
        {
            processDistressedProperty(0);
        }

        public void ExitState()
        {
        }

        public void Update()
        {
        }
    }
}
