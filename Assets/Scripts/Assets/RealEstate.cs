using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets
{
    public class AbstractRealEstate : AbstractInvestment
    {
        public RealEstateTemplate template { get; private set; }
        public int unitCount { get; private set; }

        public AbstractRealEstate(
            RealEstateTemplate template,
            int originalPrice,
            int marketValue,
            int annualIncome,
            int unitCount,
            bool isDebtInterestDelayed)
            : base("", originalPrice, marketValue, annualIncome, isDebtInterestDelayed)
        {
            label = unitCount > 1 ?
                string.Format(template.label, unitCount) :
                template.label;
            description = unitCount > 1 ?
                string.Format(template.description, unitCount) :
                template.description;

            this.template = template;
            this.unitCount = unitCount;
        }
    }
}
