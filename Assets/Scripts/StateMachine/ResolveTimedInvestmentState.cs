using PlayerInfo;

namespace StateMachine
{
    public class ResolveTimedInvestmentState : IState
    {
        private StateMachine _stateMachine;

        public ResolveTimedInvestmentState(StateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }

        private void onResolve(Player player, int index, bool resolved)
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

        public void EnterState()
        {
            resolveTimedInvestment(GameManager.Instance.player, 0);
        }

        public void ExitState()
        {
        }

        public void Update()
        {
        }

        private void onEventDone()
        {
            _stateMachine.ChangeState(_stateMachine.MarketEventState);
        }

    }
}
