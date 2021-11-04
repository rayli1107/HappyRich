using Actions;
using Events.Personal;
using PlayerInfo;
using PlayerState;
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

        private Action<Action> getGoodEvent(Player player, System.Random random)
        {
            List<Action<Action>> events = new List<Action<Action>>();
            return CompositeActions.GetRandomAction(events, random);
        }
        private Action<Action> getBadEvent(Player player, System.Random random)
        {
            List<Action<Action>> events = new List<Action<Action>>();
            events.Add(JobLossEvent.GetEvent(player, random));
            if (!player.states.Exists(s => s is TragedyPenaltyState))
            {
                events.Add(TragedyEvents.GetEvent(player, random));
            }
            return CompositeActions.GetRandomAction(events, random);
        }

        public void EnterState(StateMachineParameter param)
        {
            Player player = GameManager.Instance.player;
            System.Random random = GameManager.Instance.Random;

            List<Action<Action>> allEvents = new List<Action<Action>>();

            allEvents.Add(getBadEvent(player, random));
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
