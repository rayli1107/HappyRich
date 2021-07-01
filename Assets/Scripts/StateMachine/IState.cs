namespace StateMachine
{
    public interface IState
    {
        void EnterState(StateMachineParameter param);
        void ExitState();
        void Update();
    }
}
