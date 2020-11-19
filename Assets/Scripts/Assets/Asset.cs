namespace Assets
{
    public class AbstractAsset
    {
        public string name { get; private set; }
        public int value { get; private set; }
        public AbstractLiability liability { get; private set; }

        private int _passiveIncome;

        public AbstractAsset(
            string name, int value, AbstractLiability liability, int passiveIncome)
        {
            this.name = name;
            this.value = value;
            this.liability = liability;
            _passiveIncome = passiveIncome;
        }

        public int getIncome()
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
            base("Auto Loan", value, new AutoLoan(value), 0)
        {
        }
    }

    public class Education : AbstractAsset
    {
        public Education(int value) :
            base("Student Loan", 0, new StudentLoan(value), 0)
        {
        }
    }
}
