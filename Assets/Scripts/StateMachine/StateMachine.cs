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

        public IState currentState { get; private set; }

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

            currentState = GameInitState;
        }

        public void Start(StateMachineParameter param)
        {
            currentState.EnterState(param);
        }

        public void ChangeState(IState newState, StateMachineParameter param = null)
        {
            if (currentState != null)
            {
                currentState.ExitState();
            }
            //            Debug.LogFormat("State: {0}", newState.GetType().Name);
            currentState = newState;

            if (currentState != null)
            {
                currentState.EnterState(param);
            }
        }

        public void Update()
        {
            if (currentState != null)
            {
                currentState.Update();
            }
        }

        public bool CheckState(IState state)
        {
            return currentState == state;
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
