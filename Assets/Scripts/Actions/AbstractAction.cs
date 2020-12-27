namespace Actions
{
    public delegate void ActionCallback(bool success);

    public class AbstractAction
    {
        private ActionCallback _callback;

        public AbstractAction(ActionCallback callback=null)
        {
            _callback = callback;
        }

        public virtual void Start()
        {
        }

        protected void RunCallback(bool success)
        {
            _callback?.Invoke(success);
        }
    }
}
