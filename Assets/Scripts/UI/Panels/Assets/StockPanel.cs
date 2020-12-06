using TMPro;
using UnityEngine;
using Assets;

namespace UI.Panels.Assets
{
    public class StockPanel : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField]
        private TextMeshProUGUI _textName;
        [SerializeField]
        private TextMeshProUGUI _textValue;
        [SerializeField]
        private TextMeshProUGUI _textChange;
#pragma warning restore 0649

        public Player player;
        public AbstractStock stock;
        public bool value = true;

        private void OnEnable()
        {
            Localization local = GameManager.Instance.Localization;
            _textName.text = stock.name;
            _textValue.text = local.GetCurrency(stock.value);

            float change = stock.change;
            _textChange.text = local.GetPercent(change);
            _textChange.color = (change >= 0) ? Color.green : Color.red;
        }

        public void OnClick()
        {

        }
    }
}