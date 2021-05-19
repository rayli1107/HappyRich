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
            new Events.Market.MarketEvent(GameManager.Instance.player, onEventDone).Run();
        }

        public void ExitState()
        {
        }

        public void Update()
        {
        }

        private void onEventDone()
        {
            _stateMachine.ChangeState(_stateMachine.StockMarketEventState);
        }

    }
}
