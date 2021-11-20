using Actions;
using System;
using System.Collections.Generic;
using UnityEngine;

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
            UI.UIManager.Instance.ShowTimedTransitionScreen(
                "Market Event", Color.yellow, runMarketEvent);
        }

        private void runMarketEvent()
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
