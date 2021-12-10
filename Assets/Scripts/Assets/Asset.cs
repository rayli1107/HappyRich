using System.Collections.Generic;

namespace Assets
{
    public class AbstractAsset
    {
        public virtual string name { get; protected set; }

        public AbstractLiability combinedLiability
        {
            get
            {
                List<AbstractLiability> loans = liabilities;
                return loans.Count == 1 ? loans[0] : _combinedLiability;
            }
        }

        public virtual List<AbstractLiability> liabilities {
            get
            {
                return new List<AbstractLiability>();
            }
        }

        public virtual int totalIncome { get; private set; }
        public virtual int value { get; private set; }
        public virtual int income => totalIncome - combinedLiability.expense;
        public virtual int expectedIncome => income;

        private CombinedLiability _combinedLiability;


        public AbstractAsset(string name, int value, int totalIncome)
        {
            this.name = name;
            this.value = value;
            this.totalIncome = totalIncome;
            _combinedLiability = new CombinedLiability(this);
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

        public virtual List<string> GetDetails()
        {
            Localization local = Localization.Instance;
            List<string> details = new List<string>()
            {
                local.GetAsset(this),
                string.Format(
                    "Total Value: {0}",
                    local.GetCurrency(value)),
            };

            if (totalIncome > 0)
            {
                details.Add(
                    string.Format("Total Income: {0}", local.GetCurrency(totalIncome)));
            }

            details.AddRange(combinedLiability.GetPartialDetails());

            int netIncome = income;
            if (netIncome > 0)
            {
                details.Add(
                    string.Format("Net Income: {0}", local.GetCurrency(netIncome)));
            }

            return details;
        }
    }

    public class Car : AbstractAsset
    {
        public AutoLoan loan { get; private set; }
        public override List<AbstractLiability> liabilities {
            get {
                List<AbstractLiability> ret = base.liabilities;
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
