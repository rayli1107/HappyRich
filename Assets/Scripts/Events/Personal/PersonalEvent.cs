using StateMachine;
using System;

namespace Events.Personal
{
    public class PersonalEvent
    {
        private Action _callback;

        public PersonalEvent (Action eventDoneCallback)
        {
            _callback = eventDoneCallback;
        }

        public void Run()
        {
            _callback?.Invoke();
//            new JobLossEvent(_state).Run();
        }
    }
}
