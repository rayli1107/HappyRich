namespace StateMachine
{
    public class YearEndEventState : IState
    {
        private StateMachine _stateMachine;

        public YearEndEventState(StateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public void EnterState(StateMachineParameter param)
        {
            PlayerInfo.Player player = GameManager.Instance.player;
            EventLogManager.Instance.OnTurnEnd(player);
            ++player.age;
            UI.UIManager.Instance.UpdatePlayerInfo(player);
            PlayerInfo.Snapshot snapshot = new PlayerInfo.Snapshot(player);

            if (player.happiness >= GameManager.Instance.requiredHappiness &&
                snapshot.passiveIncomeRange.x >= snapshot.totalFixedExpenses)
            {
                StateMachineParameter newParam = new StateMachineParameter();
                newParam.victory = true;
                newParam.message = "You were able to achieve financial independence and a happy life before " +
                    "retirement age. Congratulations!";
                _stateMachine.ChangeState(_stateMachine.GameEndingState, newParam);
            }
            else if (player.cash <= 0)
            {
                StateMachineParameter newParam = new StateMachineParameter();
                newParam.victory = false;
                newParam.message = "You are now financially broke. Game over.";
                _stateMachine.ChangeState(_stateMachine.GameEndingState, newParam);
            }
            else if (player.happiness <= 0)
            {
                StateMachineParameter newParam = new StateMachineParameter();
                newParam.victory = false;
                newParam.message = "You are really unhappy with your life right now. Game over.";
                _stateMachine.ChangeState(_stateMachine.GameEndingState, newParam);
            }
            else if (player.age >= GameManager.Instance.retirementAge)
            {
                StateMachineParameter newParam = new StateMachineParameter();
                newParam.victory = false;
                newParam.message = "You reached retirement age but unfortunately have not achieved" +
                    "financial independence or a truly happy life. Game over.";
                _stateMachine.ChangeState(_stateMachine.GameEndingState, newParam);
            }
            else 
            {
                _stateMachine.ChangeState(_stateMachine.ResolveTimedInvestmentState);
            }
        }

        public void ExitState()
        {
        }

        public void Update()
        {
        }
    }
}
