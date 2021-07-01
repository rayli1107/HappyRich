namespace StateMachine
{
    public class GameOverState : IState
    {
        private StateMachine _stateMachine;

        public GameOverState(StateMachine stateMachine)
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
