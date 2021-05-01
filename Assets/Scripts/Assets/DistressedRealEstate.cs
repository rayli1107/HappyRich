﻿using System.Collections.Generic;
using UnityEngine;

namespace Assets
{
    public class DistressedRealEstate : AbstractRealEstate
    {
        public int rehabPrice { get; private set; }

        public override int value =>
            originalPrice + rehabPrice + (privateLoan == null ? 0 : privateLoan.delayedExpense);
        public override int totalCost => value;
        public override int loanValue => originalPrice;

        public int appraisalPrice { get; private set; }
        public int actualIncome { get; private set; }

        public DistressedRealEstate(
            RealEstateTemplate template,
            List<InvestmentPartner> debtPartners,
            int purchasePrice,
            int rehabPrice,
            int appraisalPrice,
            int annualIncome,
            int unitCount,
            int maxLtv)
            : base(template, purchasePrice, 0, 0, unitCount)
        {
            this.rehabPrice = rehabPrice;
            this.appraisalPrice = appraisalPrice;
            actualIncome = annualIncome;

            int rate = InterestRateManager.Instance.distressedLoanRate;
            privateLoan = new RealEstatePrivateLoan(
                this, debtPartners, maxLtv, rate, true);

            label = string.Format("Distressed {0}", label);
            description = string.Format("Distressed {0}", description);
        }
    }
}
