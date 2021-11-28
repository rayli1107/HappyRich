using PlayerInfo;
using UnityEngine;
using UnityEngine.UI;

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
        [SerializeField]
        private float _alphaHitThreshold = 0.8f;
#pragma warning restore 0649

        private void Awake()
        {
            foreach (Button button in GetComponentsInChildren<Button>())
            {
                button.GetComponent<Image>().alphaHitTestMinimumThreshold = _alphaHitThreshold;
            }
        }

        public void OnJobSearchButton()
        {
            Instantiate(_prefabJobSearchPanel, UIManager.Instance.transform).player = GameManager.Instance.player;
        }

        public void OnInvestmentsButton()
        {
            InvestmentsPanel panel = Instantiate(
                _prefabInvestmentsPanel, UIManager.Instance.transform);
            panel.player = GameManager.Instance.player;
            panel.Refresh();
        }

        public void OnNetworkingButton()
        {
            Instantiate(_prefabNetworkingPanel, UIManager.Instance.transform).player = GameManager.Instance.player;
        }

        public void OnSelfImprovementButton()
        {
            Instantiate(_prefabSelfImprovementPanel, UIManager.Instance.transform).player = GameManager.Instance.player;
        }

        public void SetEnable(bool enabled)
        {
            foreach (Button button in GetComponentsInChildren<Button>())
            {
                button.interactable = enabled;
            }
        }
    }
}

