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

        private Action<Action> getRandomEvent(List<Action<Action>> events, System.Random random)
        {
            events = events.FindAll(e => e != null);
            return events.Count == 0 ? null : CompositeActions.GetRandomAction(events, random);
        }

        private Action<Action> getGoodEvent(Player player, System.Random random)
        {
            List<Action<Action>> events = new List<Action<Action>>();
//            events.Add(JobBonusEvent.GetEvent(player, random));
            events.Add(FamilyVacationEvent.GetEvent(player));
            return getRandomEvent(events, random);
        }

        private Action<Action> getNeutralEvent(Player player, System.Random random)
        {
            List<Action<Action>> events = new List<Action<Action>>();
            events.AddRange(LuxuryManager.Instance.GetEvents(player, random));
            return getRandomEvent(events, random);
        }


        private Action<Action> getBadEvent(Player player, System.Random random)
        {
            List<Action<Action>> events = new List<Action<Action>>();
            events.Add(JobLossEvent.GetEvent(player, random));
            if (!player.states.Exists(s => s is TragedyPenaltyState))
            {
                events.Add(TragedyEvents.GetEvent(player, random));
            }
            return getRandomEvent(events, random);
        }

        private Action<Action> getFamilyEvent(Player player, System.Random random)
        {
            List<Action<Action>> events = FamilyManager.Instance.GetEvents(player);
            return getRandomEvent(events, random);
        }

        public void EnterState(StateMachineParameter param)
        {
            Player player = GameManager.Instance.player;
            System.Random random = GameManager.Instance.Random;

            List<Action<Action>> allEvents = new List<Action<Action>>();
            //allEvents.Add(getBadEvent(player, random));
            allEvents.Add(getGoodEvent(player, random));
            //allEvents.Add(getNeutralEvent(player, random));
            allEvents.Add(getFamilyEvent(player, random));

            allEvents = allEvents.FindAll(e => e != null);
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
