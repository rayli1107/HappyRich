namespace Transaction
{
    public interface ITransactionHandler
    {
        void OnTransactionSuccess();
        void OnTransactionFailure();
    }
}
