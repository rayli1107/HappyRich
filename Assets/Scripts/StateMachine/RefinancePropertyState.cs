using Assets;
using PlayerInfo;
using UI.Panels.Templates;
using UnityEngine;

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
            Player player,
            int index,
            PartialInvestment partialAsset,
            RefinancedRealEstate refinancedAsset) {
            TransactionManager.RefinanceProperty(
                GameManager.Instance.player,
                partialAsset,
                refinancedAsset);

            string message = string.Format(
                "You've successfully refinanced the {0} property.",
                Localization.Instance.GetRealEstateDescription(
                    refinancedAsset.description));
            UI.UIManager.Instance.ShowSimpleMessageBox(
                message,
                ButtonChoiceType.OK_ONLY,
                _ => processDistressedProperty(index + 1),
                () => partialAsset.OnDetail(player, null));
        }

        private void processDistressedProperty(int index)
        {
            Player player = GameManager.Instance.player;
            Portfolio portfolio = player.portfolio;
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
                (_) => handleRefinancePanelResult(player, index, partialAsset, refinancedAsset),
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
