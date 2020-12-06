using Assets;
using System.Collections.Generic;
using UnityEngine;

namespace UI.Panels
{
    public class AssetLiabilityListPanel : TextListScrollablePanel
    {
        protected override void OnEnable()
        {
            base.OnEnable();

            int netWorth = player.cash;
            List<AbstractLiability> liabilities = new List<AbstractLiability>();
            liabilities.AddRange(player.liabilities);
            foreach (AbstractAsset asset in player.assets)
            {
                netWorth += asset.value;
                if (asset.liability != null && asset.liability.amount > 0)
                {
                    liabilities.Add(asset.liability);
                }
            }
            foreach (AbstractLiability liability in liabilities)
            {
                netWorth -= liability.amount;
            }

            Localization local = GameManager.Instance.Localization;
            AddText(string.Format("Total Net Worth: {0}", local.GetCurrency(netWorth)));
            AddText(string.Format("Cash: {0}", local.GetCurrency(player.cash)));

            if (player.assets.Count > 0)
            {
                AddText(string.Format("Assets:"));
                foreach (AbstractAsset asset in player.assets)
                {
                    AddText(string.Format(
                        "  {0}: {1}", asset.name, local.GetCurrency(asset.value)));
                }
            }

            if (liabilities.Count > 0)
            {
                AddText(string.Format("Liabilities:"));
                foreach (AbstractLiability liability in liabilities)
                {
                    AddText(string.Format(
                        "  {0}: {1}", liability.name, local.GetCurrency(liability.amount)));
                }
            }
        }
    }
}
