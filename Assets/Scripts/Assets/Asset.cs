using Actions;
using PlayerInfo;
using System;
using System.Collections.Generic;
using UI;
using UI.Panels.Templates;

namespace Assets
{
    public class AbstractAsset
    {
        public virtual string name { get; protected set; }
        private const int _detailFontSizeMax = 32;

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
        public virtual int income => calculateNetIncome(totalIncome);
        public virtual int expectedTotalIncome => totalIncome;
        public virtual int expectedIncome => calculateNetIncome(expectedTotalIncome);

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
        protected int calculateNetIncome(int total)
        {
            return total - combinedLiability.expense;
        }

        protected virtual List<string> getTotalIncomeDetails()
        {
            Localization local = Localization.Instance;
            List<string> details = new List<string>();
            if (totalIncome > 0)
            {
                details.Add(
                    string.Format(
                        "Total Income: {0}",
                        local.GetCurrency(totalIncome)));
            }
            return details;
        }

        protected virtual List<string> getNetIncomeDetails()
        {
            Localization local = Localization.Instance;
            List<string> details = new List<string>();
            int netIncome = income;
            if (netIncome > 0)
            {
                details.Add(
                    string.Format("Net Income: {0}", local.GetCurrency(netIncome)));
            }
            return details;
        }

        public virtual List<string> getPurchaseDetails()
        {
            return new List<string>();
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
            details.AddRange(getPurchaseDetails());
            details.AddRange(getTotalIncomeDetails());
            details.AddRange(combinedLiability.GetPartialDetails());
            details.AddRange(getNetIncomeDetails());

            return details;
        }

        public virtual void OnDetail(Player player, Action callback)
        {
            AbstractLiability loan = combinedLiability;
            if (loan != null && loan.payable && loan.amount > 0)
            {
                LoanPayoffActions.PayAssetLoanPrincipal(
                    player, this, loan, callback);
                return;
            }

            List<string> details = GetDetails();
            SimpleTextMessageBox panel = UIManager.Instance.ShowSimpleMessageBox(
                string.Join("\n", details),
                ButtonChoiceType.OK_ONLY,
                _ => callback?.Invoke());
            panel.text.fontSizeMax = _detailFontSizeMax;
        }

        public virtual void OnTurnStart(System.Random random)
        {

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
