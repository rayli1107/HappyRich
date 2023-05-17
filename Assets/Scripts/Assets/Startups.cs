using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets
{
    [Serializable]
    public class StartupData
    {
        [SerializeField]
        private InvestmentData _investmentData;
        public InvestmentData investmentData => _investmentData;

        [SerializeField]
        private string _description;
        public string description => _description;

        public string label;

        [SerializeField]
        private int _duration;
        public int duration => _duration;

        public int turn;

        public void Initialize(
            string description,
            string label,
            int startupCost,
            int duration)
        {
            investmentData.Initialize(startupCost, 0, 0, 0);
            _description = description;
            this.label = label;
            _duration = duration;
            turn = 0;
        }
    }
    public class Startup : AbstractInvestment
    {
        private StartupData _startupData;
        public override string investmentType => "Startup";
        public override string label => _startupData.label;
        public override string description => _startupData.description;
        public bool exited => _startupData.turn >= _startupData.duration;
        public override int value => totalCost;

        protected override bool _isDebtInterestDelayed => true;
        protected override int _privateLoanRate =>
            InterestRateManager.Instance.startupPrivateLoanRate;
        public int accruedDelayedInterest => _startupData.turn * delayedInterest;


        private int _defaultLoanLtv;
        private int _maxLoanLtv;

        private void AddStartupLoan()
        {
            if (_maxLoanLtv > 0)
            {
                _startupData.investmentData.securedLoan = new AdjustableLoanData();
                _startupData.investmentData.securedLoan.Initialize(
                    _defaultLoanLtv,
                    _maxLoanLtv,
                    InvestmentPartnerManager.Instance.partnerCount);
                setupSecuredLoan();
            }
        }
        private void setupSecuredLoan()
        {
            if (_startupData.investmentData.securedLoan != null)
            {
                primaryLoan = new StartupLoan(
                    this, _startupData.investmentData.securedLoan, _isDebtInterestDelayed);
            }
        }

        protected override void resetLoans()
        {
            ClearPrivateLoan();
            AddStartupLoan();
        }
        public Startup(StartupData startupData, int defaultLoanLtv, int maxLoanLtv)
            : base(startupData.investmentData)
        {
            _startupData = startupData;
            _defaultLoanLtv = defaultLoanLtv;
            _maxLoanLtv = maxLoanLtv;
            setupSecuredLoan();
        }

        public void SetName(string name)
        {
            _startupData.label = name;
        }

        public void OnTurnStart()
        {
            ++_startupData.turn;
        }

        protected override List<string> getValueDetails()
        {
            return new List<string>();
        }

        public override List<string> getPurchaseDetails()
        {
            Localization local = Localization.Instance;
            List<string> details = new List<string>()
            {
                string.Format(
                    "Startup Cost: {0}",
                    local.GetCurrency(totalCost))
            };
            int interest = delayedInterest;
            if (interest > 0)
            {
                details.Add(
                    string.Format(
                        "Annual Interest: {0}",
                        local.GetCurrency(interest, true)));
            }
            return details;
        }

    }
}
