namespace StateMachine
{
    public class VictoryState : IState
    {
        private StateMachine _stateMachine;

        public VictoryState(StateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public void EnterState(StateMachineParameter param)
        {
            UI.UIManager.Instance.ShowSimpleMessageBox(
                param.message,
                UI.Panels.Templates.ButtonChoiceType.OK_ONLY,
                (_) => _stateMachine.ChangeState(_stateMachine.GameExitState));
        }

        public void ExitState()
        {
        }

        public void Update()
        {
        }
    }
}
