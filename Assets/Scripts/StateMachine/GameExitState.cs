namespace StateMachine
{
    public class GameExitState : IState
    {
        private StateMachine _stateMachine;

        public GameExitState(StateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public void EnterState(StateMachineParameter param)
        {
            System.Environment.Exit(1);
        }

        public void ExitState()
        {
        }

        public void Update()
        {
        }
    }
}
