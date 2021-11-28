using Actions;
using Assets;
using PlayerInfo;
using ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Panels.Assets
{
    public class YieldStockComponent : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField]
        private TextMeshProUGUI _textYield;
        [SerializeField]
        private YieldStockComponent _prefabYieldStockComponent;
#pragma warning restore 0649

        public Player player;
        public YieldStock yieldStock;

        public void Refresh()
        {
            Localization local = Localization.Instance;
            if (_textYield != null)
            {
                Vector2Int range = yieldStock.yieldRange;
                if (range.y == range.x)
                {
                    _textYield.text = string.Format(
                        "{0}%", range.x);
                }
                else
                {
                    _textYield.text = string.Format(
                        "{0}% - {1}%", range.x, range.y);
                }
            }
        }

        private void OnEnable()
        {
            if (player == null || yieldStock == null)
            {
                return;
            }

            Refresh();
        }

        public void ShowStockPanel()
        {
            YieldStockComponent component = Instantiate(
                _prefabYieldStockComponent, UIManager.Instance.transform);
            component.player = player;
            component.yieldStock = yieldStock;
            component.Refresh();

            StockPanel panel = component.GetComponent<StockPanel>();
            panel.player = player;
            panel.stock = yieldStock;
            panel.Refresh();
        }

    }
}