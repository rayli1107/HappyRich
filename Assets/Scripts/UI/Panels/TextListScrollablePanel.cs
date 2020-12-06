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

        protected void AddObject(GameObject gameObject)
        {
            gameObject.transform.SetParent(_gridLayoutGroup.transform);
        }

        protected void AddText(string s)
        {
            AddText(s, Color.black);
        }

        protected void AddText(string text, Color color)
        {
            GameObject textObj = new GameObject("Text");
            AddObject(textObj);

            TextMeshProUGUI textComponent = textObj.AddComponent<TextMeshProUGUI>();
            textComponent.text = text;
            textComponent.fontSize = _textSize;
            textComponent.color = color;
        }
    }
}
