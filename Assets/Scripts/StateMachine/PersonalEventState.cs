namespace StateMachine
{
    public class PersonalEventState : IState
    {
        private StateMachine _stateMachine;

        public PersonalEventState(StateMachine stateMachine)
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
            _stateMachine.ChangeState(_stateMachine.MarketEventState);
        }
    }
}
