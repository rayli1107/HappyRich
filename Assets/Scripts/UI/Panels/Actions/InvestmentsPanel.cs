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
            InvestmentAction.GetSmallInvestmentAction(player).Start();
        }

        public void OnLargeInvestmentButton()
        {
            UIManager.Instance.DestroyAllModal();
            InvestmentAction.GetLargeInvestmentAction(player).Start();
        }
    }
}
