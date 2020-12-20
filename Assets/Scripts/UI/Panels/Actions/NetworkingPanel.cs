using Actions;
using UnityEngine;

namespace UI.Panels.Actions
{
    public class NetworkingPanel : MonoBehaviour
    {
        public Player player;

        public void OnNewInvestorsButton()
        {
            UIManager.Instance.DestroyAllModal();
            new FindNewInvestors(player).Start();
        }
    }
}
