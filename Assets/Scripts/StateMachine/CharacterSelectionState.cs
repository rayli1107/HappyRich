using ScriptableObjects;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StateMachine
{
    public class CharacterSelectionState : IState
    {
        private StateMachine _stateMachine;

        public CharacterSelectionState(StateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }

        private void onProfessionSelect(Profession profession)
        {
            GameManager.Instance.CreatePlayer(profession);
            _stateMachine.ChangeState(_stateMachine.PlayerInitState);
        }

        public void EnterState(StateMachineParameter param)
        {
            UI.UIManager.Instance.ShowCharacterSelectionPanel(
                JobManager.Instance.GetInitialProfessionList(GameManager.Instance.Random),
                onProfessionSelect);
        }

        public void ExitState()
        {
        }

        public void Update()
        {
        }
    }
}
