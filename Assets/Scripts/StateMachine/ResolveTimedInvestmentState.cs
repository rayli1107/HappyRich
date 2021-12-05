using PlayerInfo;

using StartupEntity = System.Tuple<
    Assets.PartialInvestment, Assets.Startup>;

namespace StateMachine
{
    public class ResolveTimedInvestmentState : IState
    {
        private StateMachine _stateMachine;

        public ResolveTimedInvestmentState(StateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }

/*        private void onResolve(Player player, int index, bool resolved)
        {
            if (resolved)
            {
                player.portfolio.timedInvestments.RemoveAt(index);
            }
            else
            {
                ++index;
            }

            resolveTimedInvestment(player, index);
        }

        private void resolveTimedInvestment(Player player, int index)
        {
            if (index >= player.portfolio.timedInvestments.Count)
            {
                onEventDone();
                return;
            }

            player.portfolio.timedInvestments[index].OnResolve(
                player,
                (bool b) => onResolve(player, index, b));
        }
        */
        public void EnterState(StateMachineParameter param)
        {
            Player player = GameManager.Instance.player;
            foreach (StartupEntity entity in player.portfolio.startupEntities)
            {
                entity.Item2.OnTurnStart();
            }
            StartupManager.Instance.GetResolveStartupAction(
                player, GameManager.Instance.Random).Invoke(onEventDone);
//            resolveTimedInvestment(GameManager.Instance.player, 0);
        }

        public void ExitState()
        {
        }

        public void Update()
        {
        }

        private void onEventDone() 
        {
            UI.UIManager.Instance.UpdatePlayerInfo(GameManager.Instance.player);

            StateMachineParameter param = new StateMachineParameter();
            param.newYearEnterMarketEventState = true;
            _stateMachine.ChangeState(_stateMachine.YearStartState, param);
        }

    }
}
