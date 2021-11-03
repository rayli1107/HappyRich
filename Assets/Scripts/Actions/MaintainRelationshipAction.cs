using Assets;
using PlayerInfo;
using ScriptableObjects;
using System;
using System.Collections.Generic;
using UI.Panels.Templates;

namespace Actions
{
    

    public class MaintainRelationshipAction : AbstractAction
    {
        private Player _player;

        public MaintainRelationshipAction(Player player) : base(null)
        {
            _player = player;
        }

        private void onMessageDone(ButtonType buttonType)
        {
            UI.UIManager.Instance.UpdatePlayerInfo(_player);
            GameManager.Instance.StateMachine.OnPlayerActionDone();
            RunCallback(true);
        }

        private void noOp()
        {
            UI.UIManager.Instance.ShowSimpleMessageBox(
                "You've reached out to all your professional contacts.",
                ButtonChoiceType.OK_ONLY,
                onMessageDone);
        }

        private void addSpecialist()
        {
            SpecialistInfo info  = SpecialistManager.Instance.GetNewSpecialist(
                _player, GameManager.Instance.Random);
            _player.AddSpecialist(info);

            Localization local = Localization.Instance;
            string message = string.Format(
                "One of your investors introduced you to a {0}, who {1}",
                local.GetSpecialist(info),
                info.specialistDescription);
            UI.UIManager.Instance.ShowSimpleMessageBox(
                message, ButtonChoiceType.OK_ONLY, onMessageDone);
        }

        private List<GrowthStock> getStockTipList()
        {
            List<GrowthStock> stocks = new List<GrowthStock>();

            foreach (GrowthStock stock in StockManager.Instance.growthStocks)
            {
                if (Math.Abs(stock.variance) > StockManager.Instance.tipThreshold)
                {
                    stocks.Add(stock);
                }
            }
            return stocks;
        }

        private void showStockTip(GrowthStock stock)
        {
            Localization local = Localization.Instance;
            string message;
            if (stock.variance > 0)
            {
                message = string.Format(
                    "One of your investors gave you a tip that the stock {0} is overvalued.",
                    local.GetStockName(stock));
            }
            else
            {
                message = string.Format(
                    "One of your investors gave you a tip that the stock {0} is undervalued.",
                    local.GetStockName(stock));
            }
            UI.UIManager.Instance.ShowSimpleMessageBox(
                message, ButtonChoiceType.OK_ONLY, onMessageDone);
        }

        private void showCryptocurrencyTip(AbstractCryptoCurrency crypto)
        {
            string message = string.Format(
                "According to one of your investors, the cryptocurrency {0} will be the next big hit.",
                Localization.Instance.GetStockName(crypto));
            UI.UIManager.Instance.ShowSimpleMessageBox(
                message, ButtonChoiceType.OK_ONLY, onMessageDone);
        }

        public override void Start()
        {
            foreach (InvestmentPartner partner in _player.contacts)
            {
                partner.RefreshDuration();
            }

            List<Action> actions = new List<Action>();
/*
            if (SpecialistManager.Instance.HasNewSpecialistsAvailable(_player))
            {
                actions.Add(addSpecialist);
                actions.Add(addSpecialist);
            }

            List<GrowthStock> stockTips = getStockTipList();
            if (stockTips.Count > 0)
            {
                GrowthStock stock = stockTips[GameManager.Instance.Random.Next(stockTips.Count)];
                actions.Add(() => showStockTip(stock));
            }
*/
            List<AbstractCryptoCurrency> cryptos = StockManager.Instance.cryptoCurrencies.FindAll(x => !x.tookOff);
            if (cryptos.Count > 0)
            {
                AbstractCryptoCurrency crypto = cryptos[GameManager.Instance.Random.Next(cryptos.Count)];
                actions.Add(() => showCryptocurrencyTip(crypto));
            }

            if (actions.Count == 0)
            {
                actions.Add(noOp);
            }



            int index = GameManager.Instance.Random.Next(actions.Count);
            actions[index].Invoke();
        }
    }
}
