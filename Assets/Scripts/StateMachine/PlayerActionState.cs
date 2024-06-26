﻿using Actions;
using PlayerInfo;
using UnityEngine;

namespace StateMachine
{
    public class PlayerActionState : IState
    {
        public bool playerStartReady { get; private set; }
        private StateMachine _stateMachine;

        public PlayerActionState(StateMachine stateMachine)
        {
            _stateMachine = stateMachine;
            playerStartReady = false;
        }

        public void EnterState(StateMachineParameter param)
        {
            playerStartReady = false;
            UI.UIManager.Instance.ShowTimedTransitionScreen(
                "Player's Turn", Color.green, onPlayerStart);
        }


        private void onPlayerStart()
        {
            Player player = GameManager.Instance.player;
            player.OnPlayerTurnStart(GameManager.Instance.Random);
            JobManager.Instance.OnPlayerTurnStart(player, GameManager.Instance.Random);
            SkillManager.Instance.OnPlayerTurnStart(player, GameManager.Instance.Random);
            UI.UIManager.Instance.UpdatePlayerInfo(player);
//            TutorialManager.Instance.GameStartOnce.Run(null);
            EventLogManager.Instance.OnTurnStart(player);
            playerStartReady = true;
        }

        public void ExitState()
        {
        }

        public void Update()
        {
        }

        public void OnPlayerActionDone()
        {
            _stateMachine.ChangeState(_stateMachine.PlayerPostActionState);
        }

        public void OnPlayerTurnDone()
        {
            _stateMachine.ChangeState(_stateMachine.PersonalEventState);
        }
    }
}
