namespace StateMachine
{
    public class PlayerPostActionState : IState
    {
        private StateMachine _stateMachine;

        public PlayerPostActionState(StateMachine stateMachine)
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

        public void OnPlayerTurnDone()
        {
            _stateMachine.ChangeState(_stateMachine.PersonalEventState);
        }
    }
}
