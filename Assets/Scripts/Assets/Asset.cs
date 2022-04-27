using Actions;
using PlayerInfo;
using System;
using System.Collections.Generic;
using UI;
using UI.Panels.Templates;
using UnityEngine;

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

        public virtual int value { get; private set; }
        public virtual Vector2Int totalIncomeRange { get; private set; }
        public virtual Vector2Int netIncomeRange => calculateNetIncome(totalIncomeRange);
/*
        public virtual int totalIncome { get; private set; }
        public virtual int income => calculateNetIncome(totalIncome);
        public virtual int expectedTotalIncome => totalIncome;
        public virtual int expectedIncome => calculateNetIncome(expectedTotalIncome);
*/

        private CombinedLiability _combinedLiability;

        public AbstractAsset(string name, int value, Vector2Int totalIncomeRange)
        {
            this.name = name;
            this.value = value;
            this.totalIncomeRange = totalIncomeRange;
            _combinedLiability = new CombinedLiability(this);
        }

        public void addLiability(AbstractLiability liability)
        {
            liabilities.Add(liability);
        }

        public virtual void OnPurchase()
        {

        }

        public virtual void OnPurchaseStart()
        {

        }


        public virtual void OnPurchaseCancel()
        {

        }

        protected Vector2Int calculateNetIncome(Vector2Int total)
        {
            int expense = combinedLiability.expense;
            return new Vector2Int(total.x - expense, total.y - expense);
        }

        protected string getFormattedIncomeRange(Vector2Int range)
        {
            Localization local = Localization.Instance;
            if (range == Vector2Int.zero)
            {
                return null;
            }
            else if (range.x == range.y)
            {
                return local.GetCurrency(range.x);
            }
            else
            {
                return string.Format(
                    "{0} - {1}",
                    local.GetCurrency(range.x),
                    local.GetCurrency(range.y));
            }
        }

        protected virtual List<string> getTotalIncomeDetails()
        {
            List<string> details = new List<string>();
            string formatted = getFormattedIncomeRange(totalIncomeRange);
            if (formatted != null && formatted.Length > 0)
            {
                details.Add(string.Format("Total Income: {0}", formatted));
            }
            return details;
        }

        protected virtual List<string> getNetIncomeDetails()
        {
            List<string> details = new List<string>();
            string formatted = getFormattedIncomeRange(netIncomeRange);
            if (formatted != null && formatted.Length > 0)
            {
                details.Add(string.Format("Net Income: {0}", formatted));
            }
            return details;
        }

        protected virtual List<string> getValueDetails()
        {
            Localization local = Localization.Instance;
            return new List<string>()
            {
                string.Format(
                    "Total Value: {0}",
                    local.GetCurrency(value)),
            };
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
            };
            details.AddRange(getValueDetails());
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

        public virtual int GetActualIncome(System.Random random)
        {
            Vector2Int range = netIncomeRange;
            return random.Next(range.x, range.y + 1);
        }
    }

    public class Car : AbstractAsset
    {
        public Car(int value) : base("Car", value, Vector2Int.zero)
        {
        }
    }
}
