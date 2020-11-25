using UnityEngine;
using UI.Panels.Actions;

namespace UI.Panels
{
    public class ActionMenuPanel : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField]
        private GameObject _prefabJobSearchPanel;
#pragma warning restore 0649

        public Player player;

        public void OnJobSearchButton()
        {
            GameObject gameObj = Instantiate(_prefabJobSearchPanel);
            JobSearchPanel panel = gameObj.GetComponent<JobSearchPanel>();
            panel.player = GameManager.Instance.player;
            UIManager.Instance.ShowMessageBox(
                gameObj, null, Panels.ButtonChoiceType.CANCEL_ONLY);
        }
    }
}
