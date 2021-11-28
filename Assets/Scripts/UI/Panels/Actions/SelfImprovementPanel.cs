using Actions;
using PlayerInfo;
using UnityEngine;

namespace UI.Panels.Actions
{
    public class SelfImprovementPanel : MonoBehaviour
    {
        public Player player;

        private void handler(bool actionDone)
        {
            if (actionDone)
            {
                UIManager.Instance.DestroyAllModal();
            }
        }

        public void OnSelfReflectionButton()
        {
            SelfReflectionAction.Run(player, GameManager.Instance.Random, handler);
        }

        public void OnTrainingButton()
        {
            TrainingAction.Run(player, GameManager.Instance.Random, handler);
        }
    }
}
