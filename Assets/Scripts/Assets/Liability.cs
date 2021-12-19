using Actions;
using PlayerInfo;
using System;
using System.Collections.Generic;
using UI;
using UI.Panels.Templates;
using UnityEngine;

namespace Assets
{
    public class AbstractLiability
    {
        public virtual bool payable => false;
        public virtual string shortName { get; private set; }
        public virtual string longName => shortName;
        public virtual int amount { get; protected set; }
        public int interestRate { get; private set; }
        public virtual int expense => amount * interestRate / 100;

        private const int _detailFontSizeMax = 32;

        public AbstractLiability(
            string name, int amount, int interestRate)
        {
            shortName = name;
            this.amount = amount;
            this.interestRate = interestRate;
        }

        public virtual int PayOff(int payment)
        {
            payment = Mathf.Min(payment, amount);
            amount -= payment;
            return payment;
        }

        public void Add(int amount)
        {
            this.amount += amount;
        }

        public virtual List<string> GetPartialDetails()
        {
            Localization local = Localization.Instance;
            List<string> details = new List<string>();
            if (amount > 0)
            {
                details.Add(
                    string.Format(
                        "{0} Amount: {1}",
                        shortName,
                        local.GetCurrency(amount, true)));
                details.Add(
                    string.Format(
                        "{0} Interest Rate: {1}",
                        shortName,
                        local.colorWrap(
                            string.Format("{0}%", interestRate), local.colorNegative)));
                if (expense > 0)
                {
                    details.Add(
                        string.Format(
                            "{0} Annual Payment: {1}",
                            shortName,
                            local.GetCurrency(expense, true)));
                }
            }
            return details;
        }

        public virtual List<string> GetDetails()
        {
            Localization local = Localization.Instance;
            if (amount == 0)
            {
                return new List<string>();
            }

            return new List<string>()
            {
                local.GetLiability(this),
                string.Format(
                    "Amount: {0}",
                    local.GetCurrency(amount, true)),
                string.Format(
                    "Interest Rate: {0}",
                    local.colorWrap(string.Format("{0}%", interestRate), local.colorNegative)),
                string.Format(
                    "Annual Payment: {0}",
                    local.GetCurrency(expense, true))
            };
        }

        public virtual void OnDetail(Player player, Action callback)
        {
            if (payable && amount > 0)
            {
                LoanPayoffActions.PayAssetLoanPrincipal(
                    player, null, this, callback);
                return;
            }

            List<string> details = GetDetails();
            SimpleTextMessageBox panel = UIManager.Instance.ShowSimpleMessageBox(
                string.Join("\n", details),
                ButtonChoiceType.OK_ONLY,
                _ => callback?.Invoke());
            panel.text.fontSizeMax = _detailFontSizeMax;
        }
    }

    public class AutoLoan : AbstractLiability
    {
        public override bool payable => true;
        public AutoLoan(int amount) :
            base("Auto Loan", amount, InterestRateManager.Instance.autoLoanRate)
        {            
        }
    }

    public class StudentLoan : AbstractLiability
    {
        public override bool payable => true;
        public StudentLoan(int amount) :
            base("Student Loan", amount, InterestRateManager.Instance.studentLoanRate)
        {
        }
    }

    public class PersonalLoan : AbstractLiability
    {
        public override bool payable => true;
        public PersonalLoan(int amount) :
            base("Personal Loan", amount, InterestRateManager.Instance.personalLoanRate)
        {
        }
    }

    public class CombinedLiability : AbstractLiability
    {
        public AbstractAsset asset { get; private set; }
        public override string longName => string.Format(
            "{0} - {1}", shortName, asset.name);
        public override int amount
        {
            get
            {
                int amount = 0;
                foreach (AbstractLiability liability in asset.liabilities)
                {
                    amount += liability.amount;
                }
                return amount;
            }
        }

        public override int expense
        {
            get
            {
                int expense = 0;
                foreach (AbstractLiability liability in asset.liabilities)
                {
                    expense += liability.expense;
                }
                return expense;
            }
        }

        public CombinedLiability(AbstractAsset asset) : base("Loans", 0, 0)
        {
            this.asset = asset;
        }

        public override List<string> GetPartialDetails()
        {
            Localization local = Localization.Instance;
            List<string> details = new List<string>();
            if (amount > 0)
            {
                details.Add(
                    string.Format(
                        "Total Loan Amount: {0}",
                        local.GetCurrency(amount, true)));
                if (expense > 0)
                {
                    details.Add(
                        string.Format(
                            "Total Loan Payment: {0}",
                            local.GetCurrency(expense, true)));
                }
            }
            return details;
        }
    }
}
