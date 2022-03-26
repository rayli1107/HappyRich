using UnityEngine;

namespace StateMachine
{
    public class StateMachineParameter
    {
        public bool victory;
        public string message;
        public bool newYearEnterMarketEventState = true;
    }

    public class StateMachine
    {
        public GameInitState GameInitState { get; private set; }
        public CharacterSelectionState CharacterSelectionState { get; private set; }
        public PlayerInitState PlayerInitState { get; private set; }
        public ResolveTimedInvestmentState ResolveTimedInvestmentState { get; private set; }
        public MarketEventState MarketEventState { get; private set; }
        public StockMarketEventState StockMarketEventState { get; private set; }
        public RefinancePropertyState RefinancePropertyState { get; private set; }
        public PlayerActionState PlayerActionState { get; private set; }
        public PlayerPostActionState PlayerPostActionState { get; private set; }
        public PersonalEventState PersonalEventState { get; private set; }
        public GameEndingState GameEndingState { get; private set; }
        public YearEndEventState YearEndEventState { get; private set; }
        public YearStartState YearStartState { get; private set; }
        public GameExitState GameExitState { get; private set; }

        private IState _currentState;

        public StateMachine()
        {
            GameInitState = new GameInitState(this);
            CharacterSelectionState = new CharacterSelectionState(this);
            PlayerInitState = new PlayerInitState(this);
            ResolveTimedInvestmentState = new ResolveTimedInvestmentState(this);
            MarketEventState = new MarketEventState(this);
            StockMarketEventState = new StockMarketEventState(this);
            RefinancePropertyState = new RefinancePropertyState(this);
            PlayerActionState = new PlayerActionState(this);
            PlayerPostActionState = new PlayerPostActionState(this);
            PersonalEventState = new PersonalEventState(this);
            GameExitState = new GameExitState(this);
            GameEndingState = new GameEndingState(this);
            YearEndEventState = new YearEndEventState(this);
            YearStartState = new YearStartState(this);

            _currentState = GameInitState;
        }

        public void Start(StateMachineParameter param)
        {
            _currentState.EnterState(param);
        }

        public void ChangeState(IState newState, StateMachineParameter param = null)
        {
            if (_currentState != null)
            {
                _currentState.ExitState();
            }
//            Debug.LogFormat("State: {0}", newState.GetType().Name);
            _currentState = newState;

            if (_currentState != null)
            {
                _currentState.EnterState(param);
            }
        }

        public void Update()
        {
            if (_currentState != null)
            {
                _currentState.Update();
            }
        }

        public bool CheckState(IState state)
        {
            return _currentState == state;
        }

        public void OnPlayerActionDone()
        {
            if (CheckState(PlayerActionState))
            {
                PlayerActionState.OnPlayerActionDone();
            }
            else
            {
                Debug.LogError("Invalid state.");
            }
        }

        public void OnPlayerTurnDone()
        {
            if (CheckState(PlayerActionState))
            {
                PlayerActionState.OnPlayerTurnDone();
            }
            else if (CheckState(PlayerPostActionState))
            {
                PlayerPostActionState.OnPlayerTurnDone();
            }
            else 
            {
                Debug.LogError("Invalid state.");
            }
        }
    }
}
