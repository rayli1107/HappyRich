namespace UI.Panels
{
    public enum ButtonType
    {
        OK,
        CANCEL,
        OUTSIDE_BOUNDARY
    }

    public interface IMessageBoxHandler
    {
        void OnButtonClick(MessageBox msgBox, ButtonType button);
    }
}
