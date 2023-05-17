using InvestmentPartnerInfo;
using System;
using System.Collections.Generic;
using UnityEngine;

using Investment = System.Tuple<InvestmentPartnerInfo.InvestmentPartner, int>;

namespace Assets
{
/*
 * [Serializable]
    public class InvestorContribution
    {
        [SerializeField]
        private int _partnerId;
        public int partnerId => _partnerId;

        public int amount;

        public InvestorContribution(int partnerId, int amount)
        {
            _partnerId = partnerId;
            this.amount = amount;
        }
    }
*/
    [Serializable]
    public class AdjustableLoanData
    {
/*
        [SerializeField]
        private string _assetName;
        public string assetName => _assetName;
*/
        [SerializeField]
        private int _defaultltv;
        public int defaultltv => _defaultltv;

        public int maxltv;
/*
        [SerializeField]
        private int _interestRate;
        public int interestRate => _interestRate;

        [SerializeField]
        private bool _delayed;
        public bool delayed => _delayed;
*/
/*
        [SerializeField]
        private int _loanUnitValue;
        public int loanUnitValue => _loanUnitValue;
*/
        public int minltv;

        [SerializeField]
        private int _ltv;
        public int ltv
        {
            get => _ltv;
            set { _ltv = Mathf.Clamp(value, minltv, maxltv); }
        }

        [SerializeField]
        private List<int> _investorContributions;

        public void Initialize(
//            AbstractInvestment asset,
            int defaultltv,
            int maxltv,
            int partnerCount)
//            int interestRate,
//            bool delayed)
        {
//            _assetName = asset.name;
            _defaultltv = defaultltv;
            this.maxltv = maxltv;
//            _interestRate = interestRate;
//            _delayed = delayed;
//            _loanUnitValue = asset.loanUnitValue;
            minltv = 0;
            _ltv = 0;
            ltv = defaultltv;

            _investorContributions = new List<int>(partnerCount);
            for (int i = 0; i < partnerCount; ++i)
            {
                _investorContributions.Add(0);
            }
        }

        public static int GetUnitCount(AbstractInvestment asset, int loanAmount)
        {
            int unitValue = asset.loanUnitValue;
            return (loanAmount + unitValue - 1) / unitValue;
        }

        public void AddPrivateLoan(int partnerId, int amount)
        {
            _investorContributions[partnerId] += amount;
        }

        public void RemovePrivateLoan(int totalAmount, Action<int, int> partnerCallback)
        {
            for (int i = 0; i < _investorContributions.Count && totalAmount > 0; ++i)
            {
                int investorAmount = Mathf.Min(totalAmount, _investorContributions[i]);
                if (investorAmount > 0)
                {
                    _investorContributions[i] -= investorAmount;
                    totalAmount -= investorAmount;
                    partnerCallback?.Invoke(i, investorAmount);
                }
            }
        }
    }
    /*
        [Serializable]
        public class RestructuredBusinessLoanData
        {
            [SerializeField]
            private string _assetName;
            public string assetName => _assetName;

            [SerializeField]
            private int _amount;
            public int amount => _amount;

            public void Initialize(Startup startup)
            {
                _assetName = startup.name;
                _amount = startup.combinedLiability.amount + startup.accruedDelayedInterest;
            }
        }
    */
    public class AbstractSecuredLoan : AbstractLiability
    {
        protected AbstractInvestment _asset { get; private set; }
//        public virtual int delayedExpense => 0;
        public override string longName => string.Format(
            "{0} - {1}", shortName, _asset.name);

        public AbstractSecuredLoan(
            AbstractInvestment asset, string label, int amount, int interestRate)
            : base(label, amount, interestRate)
        {
            _asset = asset;
        }
    }

    public class RestructuredBusinessLoan : AbstractSecuredLoan
    {
        public RestructuredBusinessLoan(AbstractInvestment asset, int amount)
            : base(asset,
                  "Business Loan",
                  amount,
                  InterestRateManager.Instance.businessLoanRate)
        {
        }
    }

    public class AdjustableSecuredLoan : AbstractSecuredLoan
    {
        protected AdjustableLoanData _data { get; private set; }
        private bool _delayed;
        public override int amount => _data.ltv * _asset.loanUnitValue;
        public override int expense => _delayed ? 0 : base.expense;
        public int delayedExpense => _delayed ? base.expense : 0;

        public int minltv => _data.minltv;
        public int maxltv => _data.maxltv;

        public int ltv
        {
            get => _data.ltv;
            set
            {
                int oldltv = ltv;
                _data.ltv = value;
                int newltv = ltv;
                int delta = (newltv - oldltv) * _asset.loanUnitValue;
                if (delta > 0)
                {
                    AddLoan(delta);
                }
                else if (delta < 0)
                {
                    RemoveLoan(-1 * delta);
                }
            }
        }


        public AdjustableSecuredLoan(
            AbstractInvestment asset, AdjustableLoanData data, string label, int interestRate, bool delayed)
            : base(asset, label, 0, interestRate)
        {
            _data = data;
            _delayed = delayed;
/*
            this.maxltv = getUnitCount(
                maxltv * asset.loanUnitValue - asset.combinedLiability.amount);
            minltv = 0;
            this.defaultltv = defaultltv;
            _ltv = 0;
            ltv = defaultltv;
*/
        }

        protected virtual void AddLoan(int delta)
        {
        }

        protected virtual void RemoveLoan(int delta)
        {
        }

        public override int PayOff(int _)
        {
            return 0;
        }

        public void setMinimumLoanAmount(int loanAmount)
        {
            int unitCount = AdjustableLoanData.GetUnitCount(_asset, loanAmount);
            _data.minltv = Mathf.Min(unitCount, _data.maxltv);
            _data.ltv = _data.ltv;
        }

        public void setLoanAmount(int loanAmount)
        {
            _data.ltv = AdjustableLoanData.GetUnitCount(_asset, loanAmount);
        }
    }

    public class Mortgage : AdjustableSecuredLoan
    {
        public Mortgage(AbstractInvestment asset, AdjustableLoanData data, bool delayed)
            : base(asset,
                   data,
                   "Mortgage",
                   InterestRateManager.Instance.realEstateLoanRate,
                   delayed)
        {
        }
    }

    public class BusinessLoan : AdjustableSecuredLoan
    {
        public BusinessLoan(AbstractInvestment asset, AdjustableLoanData data, bool delayed)
            : base(asset,
                   data,
                   "Business Loan",
                   InterestRateManager.Instance.businessLoanRate,
                   delayed)
        {
        }
    }

    public class StartupLoan : AdjustableSecuredLoan
    {
        public StartupLoan(AbstractInvestment asset, AdjustableLoanData data, bool delayed)
            : base(asset,
                   data,
                   "Startup Loan",
                   InterestRateManager.Instance.startupBusinessLoanRate,
                   delayed)
        {
        }
    }

    public class PrivateLoan : AdjustableSecuredLoan
    {
        private List<InvestmentPartner> _investmentPartners;
        /*
        public List<InvestmentPartner> privateLenders
        {
            get
            {
                List<InvestmentPartner> partners = new List<InvestmentPartner>();
                foreach (Investment investment in _investments)
                {
                    if (investment.Item2 > 0)
                    {
                        partners.Add(investment.Item1);
                    }
                }
                return partners;
            }
        }
        */
        public PrivateLoan(
            AbstractInvestment asset,
            AdjustableLoanData data,
            List<InvestmentPartner> partners,
            int interestRate,
            bool delayed) :
            base(asset,
                 data,
                 "Private Loan",
                 interestRate,
                 delayed)
        {
            _investmentPartners = partners;

            int availableCash = 0;
            if (partners != null)
            {
                foreach (InvestmentPartner partner in partners)
                {
                    Debug.LogFormat(
                        "Partner available cash {0} {1}",
                        partner.name,
                        Localization.Instance.GetCurrency(partner.cash));
                    availableCash += partner.cash;
                }
            }

            _data.maxltv = Mathf.Min(
                _data.maxltv, _data.ltv + availableCash / asset.loanUnitValue);
        }

        protected override void AddLoan(int delta)
        {
            Debug.LogFormat("Add Loan: {0}", Localization.Instance.GetCurrency(delta));
            foreach (InvestmentPartner partner in _investmentPartners)
            {
                int partnerAmount = Mathf.Min(partner.cash, delta);
                if (partnerAmount > 0)
                {
                    partner.cash -= partnerAmount;
                    delta -= partnerAmount;
                    Debug.LogFormat(
                        "Partner New Cash: {0} {1}",
                        partner.name,
                        Localization.Instance.GetCurrency(partner.cash));
                    _data.AddPrivateLoan(partner.partnerId, partnerAmount);
                }
            }
        }

        private void removeLoanPartnerCallback(int partnerId, int amount)
        {
            InvestmentPartner partner = InvestmentPartnerManager.Instance.GetPartnerById(partnerId);
            if (partner != null)
            {
                partner.cash += amount;
            }
        }

        protected override void RemoveLoan(int delta)
        {
            _data.RemovePrivateLoan(delta, removeLoanPartnerCallback);
            /*
            Debug.LogFormat("Remove Loan: {0}", Localization.Instance.GetCurrency(delta));
            int totalPaidOff = 0;
            for (int i = 0; i < _investments.Count && delta > 0; ++i)
            {
                InvestmentPartner partner = _investments[i].Item1;
                int partnerAmount = Mathf.Min(
                    _investments[i].Item2, delta);
                if (partnerAmount > 0)
                {
                    totalPaidOff += partnerAmount;
                    delta -= partnerAmount;
                    partner.cash += partnerAmount;
                    Debug.LogFormat(
                        "Partner New Cash: {0} {1}",
                        partner.name,
                        Localization.Instance.GetCurrency(partner.cash));

                    _investments[i] = new Investment(
                        partner, _investments[i].Item2 - partnerAmount);
                }
            }*/
        }
    }
}
