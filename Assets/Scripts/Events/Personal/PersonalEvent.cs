using StateMachine;

namespace Events.Personal
{
    public class PersonalEvent : IEvent
    {
        private IEventState _state;

        public PersonalEvent (IEventState state)
        {
            _state = state;
        }

        public void Run()
        {
            new JobLossEvent(_state).Run();
        }
    }
}
