namespace StateMachine
{
    public class PlayerInitState : IState
    {
        private StateMachine _stateMachine;

        public PlayerInitState(StateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public void EnterState()
        {
            GameManager.Instance.CreatePlayer();
            GameManager.Instance.player.AddSkill(
                SkillManager.Instance.GetSkillInfo(ScriptableObjects.SkillType.BUSINESS_OPERATIONS));
            GameManager.Instance.player.contacts.Add(
                new InvestmentPartner("Alice", 200000, RiskTolerance.kHigh, 10));
            GameManager.Instance.player.contacts.Add(
                new InvestmentPartner("Bob", 200000, RiskTolerance.kLow, 10));
            UI.UIManager.Instance.UpdatePlayerInfo(GameManager.Instance.player);

        }

        public void ExitState()
        {
        }

        public void Update()
        {
            _stateMachine.ChangeState(_stateMachine.StockMarketEventState);
        }
    }
}
