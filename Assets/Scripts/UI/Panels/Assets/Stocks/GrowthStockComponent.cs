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
        [SerializeField]
        private GrowthStockComponent _prefabGrowthStockPanel;
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

        public void ShowStockPanel()
        {
            GrowthStockComponent component = Instantiate(
                _prefabGrowthStockPanel, UIManager.Instance.transform);
            component.player = player;
            component.growthStock = growthStock;
            component.Refresh();

            StockPanel panel = component.GetComponent<StockPanel>();
            panel.player = player;
            panel.stock = growthStock;
            panel.Refresh();
        }
    }
}