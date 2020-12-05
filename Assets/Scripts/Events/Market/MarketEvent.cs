using StateMachine;

namespace Events.Market
{
    public class MarketEvent : IEvent
    {
        private IEventState _state;

        public MarketEvent(IEventState state)
        {
            _state = state;
        }

        public void Run()
        {
            _state.OnEventDone();
        }
    }
}
