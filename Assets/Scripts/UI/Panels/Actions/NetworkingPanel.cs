using Actions;
using PlayerInfo;
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

        public void OnMaintainRelationshipButton()
        {
            UIManager.Instance.DestroyAllModal();
            new MaintainRelationshipAction(player).Start();
        }
    }
}
