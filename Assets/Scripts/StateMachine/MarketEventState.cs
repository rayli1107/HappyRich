using System;
using System.Collections.Generic;

namespace StateMachine
{
    public class MarketEventState : IState
    {
        private StateMachine _stateMachine;

        public MarketEventState(StateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }

        private static void runActions(List<Action<Action>> actions, int index, Action callback)
        {
            if (index >= actions.Count)
            {
                callback?.Invoke();
                return;
            }

            Action cb = () => runActions(actions, index + 1, callback);
            actions[index]?.Invoke(cb);
        }


        public void EnterState(StateMachineParameter param)
        {
            List<Action<Action>> actions = new List<Action<Action>>();
            actions.AddRange(StockManager.Instance.GetMarketEventActions(GameManager.Instance.Random));
            actions.AddRange(
                RiskyInvestmentManager.Instance.GetMarketEventActions(
                    GameManager.Instance.player, GameManager.Instance.Random));
            actions.AddRange(
                InvestmentPartnerManager.Instance.GetMarketEventActions(
                    GameManager.Instance.player, GameManager.Instance.Random));
            runActions(actions, 0, () => _stateMachine.ChangeState(_stateMachine.StockMarketEventState));
        }

        public void ExitState()
        {
        }

        public void Update()
        {
        }
    }
}
