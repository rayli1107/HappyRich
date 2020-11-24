﻿using System.Collections.Generic;
using UnityEngine;

namespace UI.Panels
{
    public class AssetLiabilityListPanel : TextListScrollablePanel
    {
        protected override void OnEnable()
        {
            base.OnEnable();

            int netWorth = player.cash;
            List<Assets.AbstractLiability> liabilities = new List<Assets.AbstractLiability>();
            liabilities.AddRange(player.liabilities);
            foreach (Assets.AbstractAsset asset in player.assets)
            {
                netWorth += asset.value;
                if (asset.liability != null && asset.liability.amount > 0)
                {
                    liabilities.Add(asset.liability);
                }
            }
            foreach (Assets.AbstractLiability liability in liabilities)
            {
                netWorth -= liability.amount;
            }

            AddText(string.Format("Total Net Worth: {0}", netWorth));
            AddText(string.Format("Cash: {0}", player.cash));

            if (player.assets.Count > 0)
            {
                AddText(string.Format("Assets:"));
                foreach (Assets.AbstractAsset asset in player.assets)
                {
                    AddText(string.Format("  {0}: {1}", asset.name, asset.value));
                }
            }

            if (liabilities.Count > 0)
            {
                AddText(string.Format("Liabilities:"));
                foreach (Assets.AbstractLiability liability in liabilities)
                {
                    AddText(string.Format("  {0}: {1}", liability.name, liability.amount));
                }
            }
        }
    }
}
