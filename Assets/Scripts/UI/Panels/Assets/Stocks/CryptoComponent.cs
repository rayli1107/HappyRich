using Actions;
using Assets;
using PlayerInfo;
using ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Panels.Assets
{
    public class CryptoComponent : StockPanel
    {
        public AbstractCryptoCurrency crypto;
        public override AbstractStock stock => crypto;
    }
}