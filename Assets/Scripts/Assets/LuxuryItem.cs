using ScriptableObjects;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets
{
    public class LuxuryItem : AbstractAsset
    {
        public LuxuryItemProfile profile { get; private set; }

        public LuxuryItem(LuxuryItemProfile profile, int price)
            : base(profile.itemName, price, 0)
        {
            this.profile = profile;
        }
    }
}
