using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UI
{
    public class SafeAreaCanvas : MonoBehaviour
    {
        private Rect _prevSafeArea;

        private void UpdateCanvasSize()
        {
            float x = Screen.safeArea.x / Screen.width;
            float width = Screen.safeArea.width / Screen.width;
            float y = Screen.safeArea.y / Screen.height;
            float height = Screen.safeArea.height / Screen.height;
            float x2 = x + width;
            float y2 = y + height;

            RectTransform rect = GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(x, y);
            rect.anchorMax = new Vector2(x2, y2);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        private void Awake()
        {
            _prevSafeArea = Rect.zero;
        }

        private void Update()
        {
            if (_prevSafeArea != Screen.safeArea)
            {
                _prevSafeArea = Screen.safeArea;

                Debug.LogFormat(
                    "Screen w {0} h {1} safe area {2}",
                    Screen.width,
                    Screen.height,
                    Screen.safeArea);
                UpdateCanvasSize();
            }
        }
    }
}
