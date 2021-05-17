using Assets;
using PlayerInfo;
using UI;
using UI.Panels.Templates;

namespace Actions
{
    public enum RiskLevel
    {
        kLow,
        kMedium,
        kHigh
    }

    public abstract class RiskyInvestmentAction : AbstractAction
    {
        protected Player player { get; private set; }

        public RiskyInvestmentAction(Player player, ActionCallback callback) : base(callback)
        {
            this.player = player;
        }

        public override void Start()
        {
            /*
            partialAsset = new PartialInvestment(
                _asset,
                player.GetEquityPartners(),
                RealEstateManager.Instance.defaultEquitySplit,
                RealEstateManager.Instance.maxEquityShares);
            ShowPurchasePanel();
            */
        }
    }
}
