using UnityEngine.SceneManagement;

namespace StateMachine
{
    public class GameExitState : IState
    {
        private StateMachine _stateMachine;

        public GameExitState(StateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public void EnterState(StateMachineParameter param)
        {
            Scene scene = SceneManager.GetActiveScene(); 
            SceneManager.LoadScene(scene.name);
        }

        public void ExitState()
        {
        }

        public void Update()
        {
        }
    }
}
