using Actions;
using PlayerInfo;
using System;
using System.Collections.Generic;
using UI.Panels.Templates;
using UnityEngine;

namespace UI.Panels.Actions
{
    using GetInvestmentFn = Func<Action<int, bool>, List<AbstractBuyInvestmentAction>>;

    public class InvestmentsPanel : MonoBehaviour
    {
        public Player player;

        private void InvesmtmentActionCallback(ButtonType buttonType)
        {
            UIManager.Instance.UpdatePlayerInfo(player);
            GameManager.Instance.StateMachine.OnPlayerActionDone();
        }


        public void OnSmallInvestmentButton()
        {
            UIManager.Instance.DestroyAllModal();
            UIManager.Instance.ShowAvailableInvestmentsPanel(
                (Action<int, bool> cb) => InvestmentManager.Instance.GetAvailableSmallInvestments(
                    GameManager.Instance.player, GameManager.Instance.Random, cb),
                InvesmtmentActionCallback);
        }

        public void OnLargeInvestmentButton()
        {
            UIManager.Instance.DestroyAllModal();
            UIManager.Instance.ShowAvailableInvestmentsPanel(
                (Action<int, bool> cb) => InvestmentManager.Instance.GetAvailableLargeInvestments(
                    GameManager.Instance.player, GameManager.Instance.Random, cb),
                InvesmtmentActionCallback);
        }
    }
}
