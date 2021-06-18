namespace StateMachine
{
    public class GameInitState : IState
    {
        private StateMachine _stateMachine;

        public GameInitState(StateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public void EnterState()
        {
        }

        public void ExitState()
        {
        }

        public void Update()
        {
            if (UI.UIManager.Instance != null &&
                UI.UIManager.Instance.ready &&
                RealEstateManager.Instance != null &&
                StockManager.Instance != null &&
                BusinessManager.Instance != null &&
                SelfImprovementManager.Instance != null &&
                RiskyInvestmentManager.Instance != null &&
                FamilyManager.Instance != null)
            {
                RealEstateManager.Instance.Initialize(GameManager.Instance.Random);
                StockManager.Instance.Initialize(GameManager.Instance.Random);
                BusinessManager.Instance.Initialize(GameManager.Instance.Random);
                SelfImprovementManager.Instance.Initialize();
                RiskyInvestmentManager.Instance.Initialize(GameManager.Instance.Random);
                FamilyManager.Instance.Initialize(GameManager.Instance.Random);

                _stateMachine.ChangeState(_stateMachine.PlayerInitState);
            }
        }
    }
}
