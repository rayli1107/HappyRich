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

        public void EnterState()
        {
            Debug.Log("PersonalEventState.EnterState");
            new Events.Personal.PersonalEvent(onEventDone).Run();
        }

        public void ExitState()
        {
        }

        public void Update()
        {
        }

        private void onEventDone()
        {
            _stateMachine.ChangeState(_stateMachine.MarketEventState);
        }
    }
}
