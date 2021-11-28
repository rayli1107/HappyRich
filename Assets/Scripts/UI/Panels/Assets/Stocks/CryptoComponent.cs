using Actions;
using Assets;
using PlayerInfo;
using ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Panels.Assets
{
    public class CryptoComponent : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField]
        private CryptoComponent _prefabCryptoComponent;
#pragma warning restore 0649

        public Player player;
        public AbstractCryptoCurrency crypto;

        public void Refresh()
        {
        }

        private void OnEnable()
        {
            if (player == null || crypto == null)
            {
                return;
            }

            Refresh();
        }

        public void ShowStockPanel()
        {
            CryptoComponent component = Instantiate(
                _prefabCryptoComponent, UIManager.Instance.transform);
            component.player = player;
            component.crypto = crypto;
            component.Refresh();

            StockPanel panel = component.GetComponent<StockPanel>();
            panel.player = player;
            panel.stock = crypto;
            panel.Refresh();
        }
    }
}