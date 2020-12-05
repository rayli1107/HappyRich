using UnityEngine;

namespace StateMachine
{
    public class PersonalEventState : IState, IEventState
    {
        private StateMachine _stateMachine;

        public PersonalEventState(StateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public void EnterState()
        {
            Debug.Log("PersonalEventState.EnterState");
            new Events.Personal.PersonalEvent(this).Run();
        }

        public void ExitState()
        {
        }

        public void Update()
        {
        }

        public void OnEventDone()
        {
            _stateMachine.ChangeState(_stateMachine.MarketEventState);
        }
    }
}
