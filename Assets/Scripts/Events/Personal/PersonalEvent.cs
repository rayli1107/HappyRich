using Actions;
using PlayerInfo;
using StateMachine;
using System;

namespace Events.Personal
{
    public class PersonalEvent
    {
        private Action _callback;
        private Player _player;

        public PersonalEvent (Player player, Action eventDoneCallback)
        {
            _player = player;
            _callback = eventDoneCallback;
        }

        public void Run()
        {
            AbstractAction action = FamilyManager.Instance.GetPersonalEventAction(
                _player, (_) => _callback.Invoke());
            if (action != null)
            {
                action.Start();
            }
            else
            {
                _callback.Invoke();
            }
//            new JobLossEvent(_state).Run();
        }
    }
}
