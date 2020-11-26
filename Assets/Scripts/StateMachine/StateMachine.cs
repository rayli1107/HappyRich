using UnityEngine;

namespace StateMachine
{
    public class StateMachine
    {
        public GameInitState GameInitState { get; private set; }
        public PlayerInitState PlayerInitState { get; private set; }
        public MarketEventState MarketEventState { get; private set; }
        public PlayerActionState PlayerActionState { get; private set; }
        public PlayerPostActionState PlayerPostActionState { get; private set; }
        public PersonalEventState PersonalEventState { get; private set; }
        public GameOverState GameOverState { get; private set; }
        public VictoryState VictoryState { get; private set; }

        private IState _currentState;

        public StateMachine()
        {
            GameInitState = new GameInitState(this);
            PlayerInitState = new PlayerInitState(this);
            MarketEventState = new MarketEventState(this);
            PlayerActionState = new PlayerActionState(this);
            PlayerPostActionState = new PlayerPostActionState(this);
            PersonalEventState = new PersonalEventState(this);
            GameOverState = new GameOverState(this);
            VictoryState = new VictoryState(this);

            _currentState = GameInitState;
        }

        public void Start()
        {
            _currentState.EnterState();
        }

        public void ChangeState(IState newState)
        {
            if (_currentState != null)
            {
                _currentState.ExitState();
            }

            _currentState = newState;

            if (_currentState != null)
            {
                _currentState.EnterState();
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
