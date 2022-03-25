using Actions;
using Assets;
using PlayerInfo;
using TMPro;
using UI.Panels.Templates;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Panels.Assets
{
    public class StockPanel : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField]
        private TextMeshProUGUI _textName;
        [SerializeField]
        private TextMeshProUGUI _textTitle;
        [SerializeField]
        private TextMeshProUGUI _textValue;
        [SerializeField]
        private TextMeshProUGUI _textChange;
        [SerializeField]
        private ItemValuePanel _panelShares;
        [SerializeField]
        private ItemValuePanel _panelTotalValue;
        [SerializeField]
        private Button _buttonSell;
        [SerializeField]
        private Button _buttonTrade;
        [SerializeField]
        private bool _showAlways = true;
#pragma warning restore 0649

        public Player player;
        public AbstractStock stock;
        public Button buttonTrade => _buttonTrade;

        public void Refresh()
        {
            Localization local = Localization.Instance;
            if (_textName != null)
            {
                _textName.text = stock.name;
            }

            if (_textTitle != null)
            {
                _textTitle.text = stock.longName;
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
                value = result.value;
            }

            if (_panelShares != null)
            {
                _panelShares.gameObject.SetActive(_showAlways || count > 0);
                _panelShares.SetValue(count);
                _panelShares.tabCount = 1;
            }

            if (_panelTotalValue != null)
            {
                _panelTotalValue.gameObject.SetActive(_showAlways || count > 0);
                _panelTotalValue.SetValue(local.GetCurrencyPlain(value));
                _panelTotalValue.tabCount = 1;
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
            stock.OnDetail(null);
        }

        public void OnBuy()
        {
            new BuyStocksAction(player, stock, OnActionCallback).Start();
        }

        public void OnSell()
        {
            new SellStocksAction(player, stock, OnActionCallback).Start();
        }

        public void OnActionCallback(bool success)
        {
            if (success)
            {
                UIManager.Instance.UpdatePlayerInfo(player);
                Refresh();
            }
        }
    }
}