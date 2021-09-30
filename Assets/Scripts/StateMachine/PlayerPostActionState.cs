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
            UI.UIManager.Instance.ShowEndTurnButton(true);
        }

        public void ExitState()
        {
            UI.UIManager.Instance.EnableActionButton(true);
            UI.UIManager.Instance.ShowEndTurnButton(false);
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
