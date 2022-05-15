using Actions;
using PlayerInfo;
using ScriptableObjects;
using System;
using System.Collections.Generic;
using UI.Panels.Assets;
using UI.Panels.Templates;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Panels.Actions
{
    public class InvestmentsPanel : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField]
        private Button _buttonSmallInvestments;
        [SerializeField]
        private Button _buttonLargeInvestments;
        [SerializeField]
        private Button _buttonEvaluateStocks;
        [SerializeField]
        private AvailableInvestmentsPanel _prefabAvailableInvestmentsPanel;
#pragma warning restore 0649

        public Player player;

        public Button buttonSmallInvestments => _buttonSmallInvestments;
        public Button buttonLargeInvestments => _buttonLargeInvestments;

        private TutorialAction _tutorialAction => TutorialManager.Instance.InvestmentOnce;

        private void invesmtmentActionCallback()
        {
            UIManager.Instance.UpdatePlayerInfo(player);
            GameManager.Instance.StateMachine.OnPlayerActionDone();
        }

        private void showAvailableInvestmentsPanel(
            List<AvailableInvestmentContext> buyActions,
            MessageBoxHandler messageBoxHandler)
        {
            AvailableInvestmentsPanel panel = Instantiate(
                _prefabAvailableInvestmentsPanel,
                UIManager.Instance.transform);
            panel.player = GameManager.Instance.player;
            panel.Initialize(buyActions);
            panel.gameObject.SetActive(true);

            MessageBox messageBox = panel.GetComponent<MessageBox>();
            messageBox.messageBoxHandler = messageBoxHandler;
        }

        public void OnSmallInvestmentButton()
        {
            UIManager.Instance.DestroyAllModal();
            showAvailableInvestmentsPanel(
                InvestmentManager.Instance.GetAvailableSmallInvestments(
                    player, GameManager.Instance.Random),
                _ => invesmtmentActionCallback());
        }

        public void OnLargeInvestmentButton()
        {
            UIManager.Instance.DestroyAllModal();
            showAvailableInvestmentsPanel(
                InvestmentManager.Instance.GetAvailableLargeInvestments(
                    player, GameManager.Instance.Random),
                _ => invesmtmentActionCallback());
        }

        public void OnEvaluateStockButton()
        {
            UIManager.Instance.DestroyAllModal();
            EvaluateStocksAction.Run(invesmtmentActionCallback);
        }

        public void Refresh()
        {
            if (_buttonEvaluateStocks != null)
            {
                _buttonEvaluateStocks.gameObject.SetActive(
                    player != null && player.HasSkill(SkillType.STOCK_EVALUATION));
            }
        }

        public void OnHelpButton()
        {
            _tutorialAction.ForceRun(null);
        }

        private void OnEnable()
        {
            Refresh();
//            _tutorialAction.Run(null);
        }
    }
}
