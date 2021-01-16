﻿using System.Collections.Generic;

namespace Assets
{
    public class DistressedRealEstate : AbstractRealEstate
    {
        public int rehabPrice { get; private set; }

        public override int value =>
            originalPrice + rehabPrice + privateLoanDelayedPayment;
        public override int maxPrivateLoanAmount => originalPrice;

        public override string label =>
            string.Format("Distressed {0}", base.label);
        public override string description =>
            string.Format("Distressed {0}", base.description);

        public int appraisalPrice { get; private set; }
        public int actualIncome { get; private set; }

        public DistressedRealEstate(
            RealEstateTemplate template,
            int purchasePrice,
            int rehabPrice,
            int appraisalPrice,
            int annualIncome,
            int unitCount)
            : base(template, purchasePrice, 0, 0, unitCount)
        {
            this.rehabPrice = rehabPrice;
            this.appraisalPrice = appraisalPrice;
            actualIncome = annualIncome;
        }
    }
}
