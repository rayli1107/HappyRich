using ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Panels
{
    public class JobListPanel : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField]
        private JobPanel _prefabJobPanel;
#pragma warning restore 0649

        public Player player;

        private void OnEnable()
        {
            GridLayoutGroup group = GetComponent<GridLayoutGroup>();
            RectTransform rect = GetComponent<RectTransform>();
            RectTransform prefabRect = _prefabJobPanel.GetComponent<RectTransform>();
            group.cellSize = prefabRect.sizeDelta;

            float height = prefabRect.sizeDelta.y * player.jobs.Count;
            height += group.spacing.y * (player.jobs.Count - 1);
            rect.sizeDelta = new Vector2(prefabRect.sizeDelta.x, height);

            transform.DetachChildren();

            for (int i = 0; i < player.jobs.Count; ++i)
            {
                JobPanel childPanel = Instantiate(_prefabJobPanel, transform);
                childPanel.player = player;
                childPanel.job = player.jobs[i];
                childPanel.gameObject.SetActive(true);
            }
        }

        public void Restart()
        {
            MessageBox messageBox = GetComponentInParent<MessageBox>();
            Destroy(messageBox.gameObject);
            UIManager.Instance.ShowJobListPanel();
        }
    }
}
