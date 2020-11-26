namespace StateMachine
{
    public class VictoryState : IState
    {
        private StateMachine _stateMachine;

        public VictoryState(StateMachine stateMachine)
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
