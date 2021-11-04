using Actions;
using Events.Personal;
using PlayerInfo;
using System;
using System.Collections.Generic;

namespace StateMachine
{
    public class PersonalEventState : IState
    {
        private StateMachine _stateMachine;

        public PersonalEventState(StateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }

        private Action<Action> getAction(System.Random random, Player player)
        {
            List<Action<Action>> events = new List<Action<Action>>();
            events.AddRange(JobLossEvent.GetEvents(player));
            return events[random.Next(events.Count)];
        }

        public void EnterState(StateMachineParameter param)
        {
            Player player = GameManager.Instance.player;
            System.Random random = GameManager.Instance.Random;

            List<Action<Action>> allEvents = new List<Action<Action>>();

            List<Action<Action>> familyEvents = FamilyManager.Instance.GetEvents(player);
            if (familyEvents.Count > 0)
            {
                allEvents.Add(CompositeActions.GetRandomAction(familyEvents, random));
            }

            CompositeActions.GetRandomAction(allEvents, random)?.Invoke(
                () => _stateMachine.ChangeState(_stateMachine.YearEndEventState));
        }

        public void ExitState()
        {
        }

        public void Update()
        {
        }
    }
}
