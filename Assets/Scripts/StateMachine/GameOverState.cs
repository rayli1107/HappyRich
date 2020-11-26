namespace StateMachine
{
    public class GameOverState : IState
    {
        private StateMachine _stateMachine;

        public GameOverState(StateMachine stateMachine)
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
        }
    }
}
