using Actions;
using PlayerInfo;
using System;
using UnityEngine;

namespace UI.Panels.Actions
{
    public class NetworkingPanel : MonoBehaviour
    {
        public Player player;

        private void onNewInvestorDone()
        {
            GameManager.Instance.StateMachine.OnPlayerActionDone();
        }

        public void OnNewInvestorsButton()
        {
            UIManager.Instance.DestroyAllModal();
            Action<Action> action = InvestmentPartnerManager.Instance.GetAction(
                GameManager.Instance.player, GameManager.Instance.Random);
            action?.Invoke(() => GameManager.Instance.StateMachine.OnPlayerActionDone());
        }

        public void OnMaintainRelationshipButton()
        {
            UIManager.Instance.DestroyAllModal();
            new MaintainRelationshipAction(player).Start();
        }
    }
}
