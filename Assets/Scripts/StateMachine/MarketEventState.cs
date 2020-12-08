﻿namespace StateMachine
{
    public class MarketEventState : IState, IEventState
    {
        private StateMachine _stateMachine;

        public MarketEventState(StateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public void EnterState()
        {
            GameManager.Instance.StockManager.OnTurnStart(
                GameManager.Instance.Random);
            new Events.Market.MarketEvent(this).Run();
        }

        public void ExitState()
        {
        }

        public void Update()
        {
        }

        public void OnEventDone()
        {
            _stateMachine.ChangeState(_stateMachine.PlayerActionState);
        }

    }
}