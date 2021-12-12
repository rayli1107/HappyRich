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
    }
}