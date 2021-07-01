namespace StateMachine
{
    public class StockMarketEventState : IState
    {
        private StateMachine _stateMachine;

        public StockMarketEventState(StateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public void EnterState(StateMachineParameter param)
        {
            StockManager.Instance.OnTurnStart(GameManager.Instance.Random);
            _stateMachine.ChangeState(_stateMachine.SellPropertyState);
        }

        public void ExitState()
        {
        }

        public void Update()
        {
        }
    }
}
