namespace StateMachine
{
    public interface IState
    {
        void EnterState();
        void ExitState();
        void Update();
    }
}
