using Actions;
using PlayerInfo;
using UnityEngine;

namespace StateMachine
{
    public class PlayerActionState : IState
    {
        private StateMachine _stateMachine;
        private RunOnceAction _tutorialAction;

        public PlayerActionState(StateMachine stateMachine)
        {
            _stateMachine = stateMachine;
            _tutorialAction = new RunOnceAction(
                () => TutorialManager.Instance.GetGameActionMessageAction()?.Invoke(null));
        }

        public void EnterState(StateMachineParameter param)
        {
            UI.UIManager.Instance.ShowTimedTransitionScreen(
                "Player's Turn", Color.green, onPlayerStart);
        }


        private void onPlayerStart()
        {
            Player player = GameManager.Instance.player;
            player.OnPlayerTurnStart(GameManager.Instance.Random);
            JobManager.Instance.OnPlayerTurnStart(GameManager.Instance.Random);
            SkillManager.Instance.OnPlayerTurnStart(player, GameManager.Instance.Random);
            UI.UIManager.Instance.UpdatePlayerInfo(player);
            _tutorialAction.Run();
        }

        public void ExitState()
        {
        }

        public void Update()
        {
        }

        public void OnPlayerActionDone()
        {
            _stateMachine.ChangeState(_stateMachine.PlayerPostActionState);
        }

        public void OnPlayerTurnDone()
        {
            _stateMachine.ChangeState(_stateMachine.PersonalEventState);
        }
    }
}
