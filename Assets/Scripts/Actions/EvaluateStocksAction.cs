using Assets;
using PlayerInfo;
using System;
using UI.Panels.Templates;

namespace Actions
{
    public static class EvaluateStocksAction
    {
        public static void Run(Action callback)
        {
            StockManager.Instance.EvaluateStocks();
            UI.UIManager.Instance.ShowSimpleMessageBox(
                "You've analyzed financial reports for different growth stocks and " +
                "was able to estimate their value.",
                ButtonChoiceType.OK_ONLY,
                _ => callback?.Invoke());
        }
    }
}
