using Actions;
using UnityEngine;

namespace UI.Panels.Actions
{
    public class InvestmentsPanel : MonoBehaviour
    {
        public Player player;

        public void OnSmallInvestmentButton()
        {
            UIManager.Instance.DestroyAllModal();
            new SmallInvestmentAction(player).Start();
        }
    }
}
