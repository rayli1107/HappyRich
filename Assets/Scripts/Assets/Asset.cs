namespace Assets
{
    public class AbstractAsset
    {
        public string name { get; private set; }
//        public int value { get; protected set; }
        public AbstractLiability liability { get; private set; }

        private int _passiveIncome;
        private int _value;

        public AbstractAsset(
            string name, int value, AbstractLiability liability, int passiveIncome)
        {
            this.name = name;
            _value = value;
            this.liability = liability;
            _passiveIncome = passiveIncome;
        }

        public virtual int getValue()
        {
            return _value;
        }

        public virtual int getIncome()
        {
            int income = _passiveIncome;
            if (liability != null)
            {
                income -= liability.getExpense();
            }
            return income;
        }
    }

    public class Car : AbstractAsset
    {
        public Car(int value) :
            base("Car", value, new AutoLoan(value), 0)
        {
        }
    }
}
