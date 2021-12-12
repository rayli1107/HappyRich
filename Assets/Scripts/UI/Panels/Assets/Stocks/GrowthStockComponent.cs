using Actions;
using Assets;
using PlayerInfo;
using ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Panels.Assets
{
    public class GrowthStockComponent : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField]
        private TextMeshProUGUI _textEstimatedValue;
#pragma warning restore 0649

        public Player player;
        public GrowthStock growthStock;

        public void Refresh()
        {
            Localization local = Localization.Instance;
            if (_textEstimatedValue != null)
            {
                _textEstimatedValue.text =
                    player.HasSkill(SkillType.STOCK_EVALUATION) &&
                    StockManager.Instance.stockEvaluated ?
                    local.GetCurrency(Mathf.RoundToInt(growthStock.basePrice)) :
                    "???";
            }
        }

        private void OnEnable()
        {
            if (player == null || growthStock == null)
            {
                return;
            }

            Refresh();
        }
    }
}