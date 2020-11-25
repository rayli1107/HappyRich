using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Panels
{
    public class TextListScrollablePanel : MonoBehaviour
    {
        public Player player;
        private float _textSize;
        private GridLayoutGroup _gridLayoutGroup;

        protected virtual void OnEnable()
        {
            _gridLayoutGroup = GetComponentInChildren<GridLayoutGroup>(true);
            float width = _gridLayoutGroup.GetComponent<RectTransform>().rect.width;
            _gridLayoutGroup.cellSize = new Vector2(width, _gridLayoutGroup.cellSize.y);
            _textSize = _gridLayoutGroup.cellSize.y - 4;
        }

        protected void AddText(string text)
        {
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(_gridLayoutGroup.transform);
            TextMeshProUGUI textComponent = textObj.AddComponent<TextMeshProUGUI>();
            textComponent.text = text;
            textComponent.fontSize = _textSize;
            textComponent.color = Color.black;
        }
    }
}
