namespace StateMachine
{
    public class GameEndingState : IState
    {
        private StateMachine _stateMachine;

        public GameEndingState(StateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }

        private void showEventLog()
        {
            UI.UIManager.Instance.ShowEventLogPanel(
                (_) => _stateMachine.ChangeState(_stateMachine.GameExitState));
        }

        public void EnterState(StateMachineParameter param)
        {
            UI.UIManager.Instance.ShowSimpleMessageBox(
                param.message,
                UI.Panels.Templates.ButtonChoiceType.OK_ONLY,
                (_) => showEventLog());
        }

        public void ExitState()
        {
        }

        public void Update()
        {
        }
    }
}
