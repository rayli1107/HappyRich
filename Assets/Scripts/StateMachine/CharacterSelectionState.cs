using Actions;
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
            Localization local = Localization.Instance;
            string retireAge = GameManager.Instance.retirementAge.ToString();

            List<string> messages = new List<string>()
            {
                string.Format(
                    "Welcome to the {0}!",
                    local.colorWrap("Game of Prosperity", new Color(1f, 0.6f, 0f))),
                string.Format(
                    "The goal of the game is to reach {0} and {1} before " +
                    "the age of retirement ({2}).",
                    local.colorWrap("100% Happiness", Color.yellow),
                    local.colorWrap("100% Financial Independence", new Color(0.72f, 0.43f, 0.48f)),
                    local.colorWrap(retireAge, Color.green))
            };

            TutorialMessageAction.GetAction(messages)?.Invoke(
                () => UI.UIManager.Instance.ShowCharacterSelectionPanel(
                    JobManager.Instance.GetInitialProfessionList(GameManager.Instance.Random),
                    onProfessionSelect));
        }

        public void ExitState()
        {
        }

        public void Update()
        {
        }
    }
}
