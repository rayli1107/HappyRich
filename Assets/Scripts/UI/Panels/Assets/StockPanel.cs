using Actions;
using Assets;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Panels.Assets
{
    public class StockPanel : MonoBehaviour, IActionCallback
    {
#pragma warning disable 0649
        [SerializeField]
        private TextMeshProUGUI _textName;
        [SerializeField]
        private TextMeshProUGUI _textValue;
        [SerializeField]
        private TextMeshProUGUI _textChange;
        [SerializeField]
        private TextMeshProUGUI _textYield;
        [SerializeField]
        private TextMeshProUGUI _textShares;
        [SerializeField]
        private TextMeshProUGUI _textTotalValue;
        [SerializeField]
        private Button _buttonSell;
#pragma warning restore 0649

        public Player player;
        public AbstractStock stock;

        public void Refresh()
        {

            Localization local = Localization.Instance;
            if (_textName != null)
            {
                _textName.text = stock.name;
            }

            if (_textValue != null)
            {
                _textValue.text = local.GetCurrency(stock.value);
            }

            if (_textChange != null)
            {
                float change = stock.change;
                _textChange.text = local.GetPercent(change);
            }

            int count = 0;
            int value = 0;
            PurchasedStock result;
            if (player.portfolio.stocks.TryGetValue(stock.name, out result))
            {
                count = result.count;
                value = result.getValue();
            }

            if (_textShares != null)
            {
                _textShares.text = count.ToString();
            }

            if (_textTotalValue != null)
            {
                _textTotalValue.text = local.GetCurrency(value);
            }

            if (_buttonSell)
            {
                _buttonSell.gameObject.SetActive(count > 0);
            }

        }

        private void OnEnable()
        {
            if (player == null || stock == null)
            {
                return;
            }

            Refresh();
        }

        public void OnClick()
        {
            UIManager.Instance.ShowStockTradePanel(stock);
        }

        public void OnBuy()
        {
            new BuyStocksAction(player, stock, this).Start();
        }

        public void OnActionCallback(bool success)
        {
            if (success)
            {
                Refresh();
            }
        }
    }
}