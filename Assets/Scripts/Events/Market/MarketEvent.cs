using StateMachine;
using System;

namespace Events.Market
{
    public class MarketEvent
    {
        private Action _eventDoneCallback;

        public MarketEvent(Action eventDoneCallback)
        {
            _eventDoneCallback = eventDoneCallback;
        }

        public void Run()
        {
            _eventDoneCallback?.Invoke();
        }
    }
}
