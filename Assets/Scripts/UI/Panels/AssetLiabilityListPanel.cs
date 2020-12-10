using Assets;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace UI.Panels
{
    public class AssetLiabilityListPanel : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField]
        private TextMeshProUGUI _textNetWorth;
        [SerializeField]
        private TextMeshProUGUI _textCash;
        [SerializeField]
        private TextMeshProUGUI _textAsset;
        [SerializeField]
        private TextMeshProUGUI _textLiability;
        [SerializeField]
        private GameObject _contentAsset;
        [SerializeField]
        private GameObject _contentLiability;
#pragma warning restore 0649

        public Player player;

        private void Awake()
        {
        }

        public void RefreshContent()
        {
            if (player == null)
            {
                return;
            }

            List<AbstractLiability> liabilities = new List<AbstractLiability>();
            Debug.LogFormat("Player {0}", player);
            if (player != null)
            {
                Debug.LogFormat("Player portfolio {0}", player.portfolio);
            }
            liabilities.AddRange(player.portfolio.liabilities);

            int totalAsset = 0;
            int totalLiability = 0;
            foreach (AbstractAsset asset in player.portfolio.assets)
            {
                totalAsset += asset.getValue();
                if (asset.liability != null && asset.liability.amount > 0)
                {
                    liabilities.Add(asset.liability);
                }
            }
            foreach (AbstractLiability liability in liabilities)
            {
                totalLiability += liability.amount;
            }

            int netWorth = player.cash + totalAsset - totalLiability;

            Localization local = GameManager.Instance.Localization;
            _textNetWorth.text = local.GetCurrency(netWorth);
            _textCash.text = local.GetCurrency(player.cash);
            _textAsset.text = local.GetCurrency(totalAsset);
            _textLiability.text = local.GetCurrency(totalLiability);
        }

        private void OnEnable()
        {
            RefreshContent();
        }
    }
}
