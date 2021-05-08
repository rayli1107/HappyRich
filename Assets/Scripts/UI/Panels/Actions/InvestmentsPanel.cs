using PlayerInfo;
using UnityEngine;

namespace UI.Panels.Actions
{
    public class InvestmentsPanel : MonoBehaviour
    {
        public Player player;

        private void InvesmtmentActionCallback(bool success)
        {
            if (success)
            {
                UIManager.Instance.UpdatePlayerInfo(player);
            }
            GameManager.Instance.StateMachine.OnPlayerActionDone();
        }

        public void OnSmallInvestmentButton()
        {
            UIManager.Instance.DestroyAllModal();
//            RealEstateManager.Instance.GetSmallInvestmentAction(
//                player, GameManager.Instance.Random, InvesmtmentActionCallback).Start();
            BusinessManager.Instance.GetSmallInvestmentAction(
                player, GameManager.Instance.Random, InvesmtmentActionCallback).Start();
        }

        public void OnLargeInvestmentButton()
        {
            UIManager.Instance.DestroyAllModal();
            RealEstateManager.Instance.GetLargeInvestmentAction(
                player, GameManager.Instance.Random, InvesmtmentActionCallback).Start();
        }
    }
}
