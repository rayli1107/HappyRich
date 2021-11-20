using System;
using UnityEngine;

namespace StateMachine
{
    public class YearStartState : IState
    {
        private StateMachine _stateMachine;

        public YearStartState(StateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }

        private void transitionCallback(StateMachineParameter param)
        {
            if (param.newYearEnterMarketEventState)
            {
                _stateMachine.ChangeState(_stateMachine.MarketEventState);
            }
            else
            {
                _stateMachine.ChangeState(_stateMachine.StockMarketEventState);
            }
        }

        public void EnterState(StateMachineParameter param)
        {
            UI.UIManager.Instance.ShowTimedTransitionScreen(
                "Start of\nNew Year", Color.white, () => transitionCallback(param));
        }

        public void ExitState()
        {
        }

        public void Update()
        {
        }
    }
}
