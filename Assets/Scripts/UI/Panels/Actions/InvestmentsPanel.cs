using Actions;
using PlayerInfo;
using UI.Panels.Templates;
using UnityEngine;

namespace UI.Panels.Actions
{
    public class InvestmentsPanel : MonoBehaviour
    {
        public Player player;

        private void InvesmtmentActionCallback(ButtonType buttonType)
        {
            UIManager.Instance.UpdatePlayerInfo(player);
            GameManager.Instance.StateMachine.OnPlayerActionDone();
        }

        private AbstractBuyInvestmentAction getSmallInvestmentAction(ActionCallback callback)
        {
            if (GameManager.Instance.Random.Next(2) == 0)
            {
                return RealEstateManager.Instance.GetSmallInvestmentAction(
                    player, GameManager.Instance.Random, callback);
            }
            else
            {
                return BusinessManager.Instance.GetSmallInvestmentAction(
                    player, GameManager.Instance.Random, callback);
            }
        }

        private AbstractBuyInvestmentAction getLargeInvestmentAction(ActionCallback callback)
        {
            if (GameManager.Instance.Random.Next(2) == 0)
            {
                return RealEstateManager.Instance.GetLargeInvestmentAction(
                    player, GameManager.Instance.Random, callback);
            }
            else
            {
                return BusinessManager.Instance.GetLargeInvestmentAction(
                    player, GameManager.Instance.Random, callback);
            }
        }

        public void OnSmallInvestmentButton()
        {
            UIManager.Instance.DestroyAllModal();
            UIManager.Instance.ShowAvailableInvestmentsPanel(
                getSmallInvestmentAction,
                InvestmentManager.Instance.getAvailableInvestments(GameManager.Instance.player),
                InvesmtmentActionCallback);
        }

        public void OnLargeInvestmentButton()
        {
            UIManager.Instance.DestroyAllModal();
            UIManager.Instance.ShowAvailableInvestmentsPanel(
                getLargeInvestmentAction,
                InvestmentManager.Instance.getAvailableInvestments(GameManager.Instance.player),
                InvesmtmentActionCallback);
        }
    }
}
