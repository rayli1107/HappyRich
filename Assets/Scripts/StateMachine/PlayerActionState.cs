﻿using UnityEngine;

namespace StateMachine
{
    public class PlayerActionState : IState
    {
        private StateMachine _stateMachine;
        private bool _firstTurn;

        public PlayerActionState(StateMachine stateMachine)
        {
            _stateMachine = stateMachine;
            _firstTurn = true;
        }

        public void EnterState()
        {
            Player player = GameManager.Instance.player;
            if (_firstTurn)
            {
                _firstTurn = false;
            }
            else
            {
                player.DistributeCashflow();
            }
            UI.UIManager.Instance.UpdatePlayerInfo(player);
        }

        public void ExitState()
        {
        }

        public void Update()
        {
        }

        public void OnPlayerActionDone()
        {
            Debug.Log("OnPlayerActionDone");
            _stateMachine.ChangeState(_stateMachine.PlayerPostActionState);
        }

        public void OnPlayerTurnDone()
        {
            _stateMachine.ChangeState(_stateMachine.PersonalEventState);
        }
    }
}