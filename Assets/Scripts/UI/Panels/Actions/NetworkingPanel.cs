using Actions;
using PlayerInfo;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Panels.Actions
{
    public class NetworkingPanel : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField]
        private Button _buttonFindInvestor;
        [SerializeField]
        private Button _buttonMaintainRelationship;
#pragma warning restore 0649

        public Player player;
        public Button buttonFindInvestor => _buttonFindInvestor;
        public Button buttonMaintainRelationship => _buttonMaintainRelationship;

        private TutorialAction _tutorialAction => TutorialManager.Instance.NetworkingOnce;

        public void OnNewInvestorsButton()
        {
            UIManager.Instance.DestroyAllModal();
            Action<Action> action = InvestmentPartnerManager.Instance.GetAction(
                GameManager.Instance.player, GameManager.Instance.Random);
            action?.Invoke(() => GameManager.Instance.StateMachine.OnPlayerActionDone());
        }

        public void OnMaintainRelationshipButton()
        {
            UIManager.Instance.DestroyAllModal();
            new MaintainRelationshipAction(player).Start();
        }

        public void OnHelpButton()
        {
            _tutorialAction.ForceRun(null);
        }

        private void OnEnable()
        {
//            _tutorialAction.Run(null);
        }
    }
}
