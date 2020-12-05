namespace Actions
{
    public interface IAction
    {
        void Start();
    }

    public interface IActionCallback
    {
        void OnActionCallback(bool success);
    }
}
