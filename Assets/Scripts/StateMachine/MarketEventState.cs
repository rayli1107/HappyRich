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
            GameManager.Instance.StockManager.OnTurnStart(
                GameManager.Instance.Random);
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
