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
            MarketEventManager.Instance.GetMarketEvent()?.Invoke(
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
