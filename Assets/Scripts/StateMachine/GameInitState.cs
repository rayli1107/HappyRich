namespace StateMachine
{
    public class GameInitState : IState
    {
        private StateMachine _stateMachine;

        public GameInitState(StateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public void EnterState()
        {
        }

        public void ExitState()
        {
        }

        public void Update()
        {
            if (UI.UIManager.Instance != null &&
                UI.UIManager.Instance.ready)
            {
                _stateMachine.ChangeState(_stateMachine.PlayerInitState);
            }
        }
    }
}
