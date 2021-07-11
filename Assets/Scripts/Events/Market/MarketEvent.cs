﻿using PlayerInfo;
using StateMachine;
using System;

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
            if (false)
            {
                RiskyInvestmentManager.Instance.GetInvestmentAction(
                    _player, (_) => _eventDoneCallback?.Invoke()).Start();

            }
            else
            {
                _eventDoneCallback?.Invoke();
            }
        }
    }
}
