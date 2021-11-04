using Actions;
using System;
using System.Collections.Generic;

namespace StateMachine
{
    public class MarketEventState : IState
    {
        private StateMachine _stateMachine;

        public MarketEventState(StateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public void EnterState(StateMachineParameter param)
        {
            List<Action<Action>> actions = new List<Action<Action>>();
            actions.AddRange(StockManager.Instance.GetMarketEventActions(GameManager.Instance.Random));
            actions.AddRange(
                RiskyInvestmentManager.Instance.GetMarketEventActions(
                    GameManager.Instance.player, GameManager.Instance.Random));
            actions.AddRange(
                InvestmentPartnerManager.Instance.GetMarketEventActions(
                    GameManager.Instance.player, GameManager.Instance.Random));
            CompositeActions.GetAndAction(actions)?.Invoke(
                () => _stateMachine.ChangeState(_stateMachine.StockMarketEventState));
        }

        public void ExitState()
        {
        }

        public void Update()
        {
        }
    }
}
