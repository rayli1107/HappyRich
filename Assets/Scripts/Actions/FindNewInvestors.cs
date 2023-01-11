using InvestmentPartnerInfo;
using PlayerInfo;
using System;
using UI.Panels.Templates;

namespace Actions
{
    public static class FindNewInvestors
    {
        public static void FindInvestorNone(Action callback)
        {
            UI.UIManager.Instance.ShowSimpleMessageBox(
                "You made a few friends but none of them were interested in investing.",
                ButtonChoiceType.OK_ONLY,
                (_) => callback?.Invoke());
        }

        private static void actionDone(Player player, Action callback)
        {
            UI.UIManager.Instance.UpdatePlayerInfo(player);
            callback?.Invoke();
        }

        public static void FindInvestor(
            Player player, InvestmentPartner partner, Action callback)
        {
            player.contacts.Add(partner);
            string description = null;
            string partnerType = partner.riskTolerance.ToString();
            switch (partner.riskTolerance)
            {
                case RiskTolerance.kHigh:
                    description = "prefers investing as equity partners.";
                    break;
                case RiskTolerance.kMedium:
                    description = "is open to both equity and debt partnership.";
                    break;
                case RiskTolerance.kLow:
                default:
                    description = "prefers investing using debt.";
                    break;
            }

            string formattedCash = Localization.Instance.GetCurrency(partner.cash);
            string partnerName = Localization.Instance.GetName(partner.name);
            string message = string.Format(
                "You met {0}, a follow investor, who has {1} of available cash and {2}",
                partnerName, formattedCash, description);
            EventLogManager.Instance.LogFormat(
                "Add Investor - {0} {1} {2}", partnerName, partnerType, formattedCash);
            UI.UIManager.Instance.ShowSimpleMessageBox(
                message, ButtonChoiceType.OK_ONLY, (_) => actionDone(player, callback));
        }
    }
}
