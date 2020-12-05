namespace Actions
{
    public class AbstractAction : IAction
    {
        private IActionCallback _callback;

        public AbstractAction(IActionCallback callback)
        {
            _callback = callback;
        }

        public virtual void Start()
        {
        }

        protected void RunCallback(bool success)
        {
            if (_callback != null)
            {
                _callback.OnActionCallback(success);
            }
        }
    }
}
