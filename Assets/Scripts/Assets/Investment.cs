﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets
{
    public class AbstractInvestment : AbstractAsset
    {
        public int originalPrice { get; protected set; }
        public virtual int totalCost => originalPrice + delayedInterest;
        public virtual int loanValue => originalPrice;
        public virtual int loanUnitValue => loanValue / 100;
        public string label { get; protected set; }
        public string description { get; protected set; }
        public override string name => label;
        public virtual int downPayment => Mathf.Max(
            totalCost - combinedLiability.amount, 0);

        private int _baseIncome;
        public float multiplier;
        public override int totalIncome => Mathf.FloorToInt(_baseIncome * multiplier);

        public AbstractSecuredLoan primaryLoan { get; protected set; }
        public PrivateLoan privateLoan { get; protected set; }
        private bool _isDebtInterestDelayed;

        public List<AbstractSecuredLoan> securedLoans
        {
            get
            {
                List<AbstractSecuredLoan> loans = new List<AbstractSecuredLoan>();
                if (primaryLoan != null)
                {
                    loans.Add(primaryLoan);
                }
                if (privateLoan != null)
                {
                    loans.Add(privateLoan);
                }
                return loans;
            }
        }

        public override List<AbstractLiability> liabilities
        {
            get
            {
                List<AbstractLiability> ret = base.liabilities;
                ret.AddRange(securedLoans);
                return ret;
            }
        }

        public int delayedInterest
        {
            get
            {
                int interest = 0;
                foreach (AbstractSecuredLoan loan in securedLoans)
                {
//                    Debug.LogFormat("Loan delayed interest {0}", loan.delayedExpense);
                    interest += loan.delayedExpense;
                }
                return interest;
            }
        }

        public AbstractInvestment(
            string name,
            int originalPrice,
            int marketValue,
            int annualIncome,
            bool isDebtInterestDelayed)
            : base(name, marketValue, 0)
        {
            _baseIncome = annualIncome;
            multiplier = 1;
            this.originalPrice = originalPrice;
            _isDebtInterestDelayed = isDebtInterestDelayed;
        }

        public void AddPrivateLoan(
            List<InvestmentPartner> partners,
            int maxltv,
            int rate)
        {
            if (privateLoan == null)
            {
                privateLoan = new PrivateLoan(this, partners, maxltv, rate, _isDebtInterestDelayed);
            }
        }

        public void ClearPrivateLoan()
        {
            if (privateLoan != null)
            {
                privateLoan.ltv = 0;
                privateLoan = null;
            }
        }

        public override void OnPurchaseCancel()
        {
            base.OnPurchaseCancel();
            ClearPrivateLoan();
        }
    }
}
