using Actions;
using PlayerInfo;
using ScriptableObjects;
using System;
using System.Collections.Generic;
using UI.Panels.Templates;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Panels.Actions
{
    using GetInvestmentFn = Func<Action<int, bool>, List<AbstractBuyInvestmentAction>>;

    public class InvestmentsPanel : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField]
        private Button _buttonEvaluateStocks;
#pragma warning restore 0649

        public Player player;

        private void InvesmtmentActionCallback()
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
                _ => InvesmtmentActionCallback());
        }

        public void OnLargeInvestmentButton()
        {
            UIManager.Instance.DestroyAllModal();
            UIManager.Instance.ShowAvailableInvestmentsPanel(
                (Action<int, bool> cb) => InvestmentManager.Instance.GetAvailableLargeInvestments(
                    GameManager.Instance.player, GameManager.Instance.Random, cb),
                _ => InvesmtmentActionCallback());
        }

        public void OnEvaluateStockButton()
        {
            UIManager.Instance.DestroyAllModal();
            EvaluateStocksAction.Run(InvesmtmentActionCallback);
        }

        public void Refresh()
        {
            if (_buttonEvaluateStocks != null)
            {
                _buttonEvaluateStocks.gameObject.SetActive(
                    player != null && player.HasSkill(SkillType.STOCK_EVALUATION));
            }
        }

        private void OnEnable()
        {
            Refresh();
        }
    }
}
