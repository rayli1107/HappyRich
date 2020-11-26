namespace StateMachine
{
    public class PlayerInitState : IState
    {
        private StateMachine _stateMachine;

        public PlayerInitState(StateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public void EnterState()
        {
            GameManager.Instance.CreatePlayer();
            UI.UIManager.Instance.UpdatePlayerInfo(GameManager.Instance.player);
        }

        public void ExitState()
        {
        }

        public void Update()
        {
            _stateMachine.ChangeState(_stateMachine.MarketEventState);
        }
    }
}
