namespace StateMachine
{
    public class PlayerPostActionState : IState
    {
        private StateMachine _stateMachine;

        public PlayerPostActionState(StateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public void EnterState(StateMachineParameter param)
        {
            UI.UIManager.Instance.EnableActionButton(false);
        }

        public void ExitState()
        {
            UI.UIManager.Instance.EnableActionButton(true);
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
