using Actions;
using Events.Personal;
using PlayerInfo;
using PlayerState;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StateMachine
{
    public class PersonalEventState : IState
    {
        private StateMachine _stateMachine;

        public PersonalEventState(StateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public void EnterState(StateMachineParameter param)
        {
            UI.UIManager.Instance.ShowTimedTransitionScreen(
               "Personal Event", new Color(1, 0.5f, 0), runPersonalEvent);
        }

        private void distributeCashflow()
        {
            GameManager.Instance.player.DistributeCashflow(
                () => _stateMachine.ChangeState(_stateMachine.YearEndEventState));
        }

        private void runPersonalEvent()
        {
            PersonalEventManager.Instance.GetPersonalEvent()?.Invoke(distributeCashflow);
        }

        public void ExitState()
        {
        }

        public void Update()
        {
        }
    }
}
