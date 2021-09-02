using Actions;
using PlayerInfo;
using StateMachine;
using System;
using UnityEngine;

namespace Events.Market
{
    public class MarketEvent
    {
        private Player _player;
        private Action _eventDoneCallback;

        public MarketEvent(
            Player player,
            Action eventDoneCallback)
        {
            _player = player;
            _eventDoneCallback = eventDoneCallback;
        }

        public void Run()
        {
            if (true)
            {
                Debug.LogFormat("MarketEvent.Run");
                NewCryptoAction action = new NewCryptoAction(
                    _player, (_) => _eventDoneCallback?.Invoke());
                action.Start();
/*
                    RiskyInvestmentManager.Instance.GetInvestmentAction(
                    _player, (_) => _eventDoneCallback?.Invoke()).Start();
*/

            }
            else
            {
                _eventDoneCallback?.Invoke();
            }
        }
    }
}
