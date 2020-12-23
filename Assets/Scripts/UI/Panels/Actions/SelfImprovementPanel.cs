using Actions;
using UnityEngine;

namespace UI.Panels.Actions
{
    public class SelfImprovementPanel : MonoBehaviour
    {
        public Player player;

        public void OnSelfReflectionButton()
        {
            UIManager.Instance.DestroyAllModal();
//            new FindNewInvestors(player).Start();
        }
    }
}
