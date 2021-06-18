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
            new Events.Personal.PersonalEvent(
                GameManager.Instance.player, onEventDone).Run();
        }

        public void ExitState()
        {
        }

        public void Update()
        {
        }

        private void onEventDone()
        {
            _stateMachine.ChangeState(_stateMachine.ResolveTimedInvestmentState);
        }
    }
}
