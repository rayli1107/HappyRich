namespace StateMachine
{
    public class MarketEventState : IState
    {
        private StateMachine _stateMachine;

        public MarketEventState(StateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public void EnterState()
        {
            StockManager.Instance.OnTurnStart(GameManager.Instance.Random);
            new Events.Market.MarketEvent(onEventDone).Run();
        }

        public void ExitState()
        {
        }

        public void Update()
        {
        }

        private void onEventDone()
        {
            _stateMachine.ChangeState(_stateMachine.SellPropertyState);
        }

    }
}
