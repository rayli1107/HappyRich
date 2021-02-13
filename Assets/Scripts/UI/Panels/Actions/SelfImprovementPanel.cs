using Actions;
using PlayerInfo;
using UnityEngine;

namespace UI.Panels.Actions
{
    public class SelfImprovementPanel : MonoBehaviour
    {
        public Player player;

        public void OnSelfReflectionButton()
        {
            UIManager.Instance.DestroyAllModal();
            new SelfReflectionAction(player).Start();
        }

        public void OnTrainingButton()
        {
            UIManager.Instance.DestroyAllModal();
            new TrainingAction(player).Start();
        }
    }
}
