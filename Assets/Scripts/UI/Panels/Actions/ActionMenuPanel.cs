using PlayerInfo;
using System.Collections.Generic;
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

        private void checkLocation(RectTransform rectTransform)
        {
            List<string> messages = new List<string>()
            {
                string.Format("checkLocation {0}", rectTransform.gameObject.name)
            };
            while (rectTransform != null)
            {
                messages.Add(
                    string.Format(
                        "  {0} position {1} anchoredPosition {2} offsetMin {3} offsetMax {4}",
                        rectTransform.gameObject.name,
                        rectTransform.transform.position,
                        rectTransform.anchoredPosition,
                        rectTransform.offsetMin,
                        rectTransform.offsetMax));
                if (rectTransform.parent == null)
                {
                    break;
                }
                rectTransform = rectTransform.parent.GetComponent<RectTransform>();
            }
            Debug.Log(string.Join("\n", messages));
        }

        private void Awake()
        {
            foreach (Button button in GetComponentsInChildren<Button>())
            {
//                button.GetComponent<Image>().alphaHitTestMinimumThreshold = _alphaHitThreshold;
//                checkLocation(button.GetComponent<RectTransform>());
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

