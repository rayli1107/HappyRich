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
    }
}