using System.Collections.Generic;

namespace Assets
{
    public class AbstractAsset
    {
        public string name { get; private set; }
        public CombinedLiability combinedLiability { get; private set; }

        public virtual List<AbstractLiability> liabilities {
            get
            {
                return new List<AbstractLiability>();
            }
        }

        public virtual int totalIncome { get; private set; }
        public virtual int value { get; private set; }
        public virtual int income => totalIncome - combinedLiability.expense;

        public AbstractAsset(string name, int value, int totalIncome)
        {
            this.name = name;
            this.value = value;
            this.totalIncome = totalIncome;
            combinedLiability = new CombinedLiability(this);
        }

        public void addLiability(AbstractLiability liability)
        {
            liabilities.Add(liability);
        }

        public virtual void OnPurchase()
        {

        }

        public virtual void OnPurchaseCancel()
        {

        }
    }

    public class Car : AbstractAsset
    {
        public AutoLoan loan { get; private set; }
        public override List<AbstractLiability> liabilities {
            get {
                List<AbstractLiability> ret = new List<AbstractLiability>();
                ret.Add(loan);
                return ret;
            }
        }

        public Car(int value) : base("Car", value, 0)
        {
            loan = new AutoLoan(value);
        }
    }
}
