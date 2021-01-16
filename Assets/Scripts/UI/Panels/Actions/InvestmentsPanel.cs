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
            RealEstateManager.Instance.GetSmallInvestmentAction(
                player, GameManager.Instance.Random).Start();
        }

        public void OnLargeInvestmentButton()
        {
            UIManager.Instance.DestroyAllModal();
            RealEstateManager.Instance.GetLargeInvestmentAction(
                player, GameManager.Instance.Random).Start();
        }
    }
}
