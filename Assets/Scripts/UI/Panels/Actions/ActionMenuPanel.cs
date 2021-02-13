using PlayerInfo;
using UnityEngine;

namespace UI.Panels.Actions
{
    public class ActionMenuPanel : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField]
        private JobSearchPanel _prefabJobSearchPanel;
        [SerializeField]
        private InvestmentsPanel _prefabInvestmentsPanel;
        [SerializeField]
        private NetworkingPanel _prefabNetworkingPanel;
        [SerializeField]
        private SelfImprovementPanel _prefabSelfImprovementPanel;
#pragma warning restore 0649

public Player player;

        public void OnJobSearchButton()
        {
            Instantiate(_prefabJobSearchPanel, UIManager.Instance.transform).player = player;
        }

        public void OnInvestmentsButton()
        {
            Instantiate(_prefabInvestmentsPanel, UIManager.Instance.transform).player = player;
        }

        public void OnNetworkingButton()
        {
            Instantiate(_prefabNetworkingPanel, UIManager.Instance.transform).player = player;
        }

        public void OnSelfImprovementButton()
        {
            Instantiate(_prefabSelfImprovementPanel, UIManager.Instance.transform).player = player;
        }
    }
}
