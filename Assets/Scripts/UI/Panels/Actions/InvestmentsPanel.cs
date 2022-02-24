﻿using Actions;
using PlayerInfo;
using ScriptableObjects;
using System;
using System.Collections.Generic;
using UI.Panels.Templates;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Panels.Actions
{
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
            UIManager.Instance.ShowAvailableActionsPanel(
                InvestmentManager.Instance.GetAvailableSmallInvestments(
                    player, GameManager.Instance.Random),
                _ => InvesmtmentActionCallback());
        }

        public void OnLargeInvestmentButton()
        {
            UIManager.Instance.DestroyAllModal();
            UIManager.Instance.ShowAvailableActionsPanel(
                InvestmentManager.Instance.GetAvailableLargeInvestments(
                    player, GameManager.Instance.Random),
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
