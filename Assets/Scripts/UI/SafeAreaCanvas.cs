using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class SafeAreaCanvas : MonoBehaviour
    {
#pragma warning disable 0649
        private Vector2 _factorRange = new Vector2(1.7f, 2f);
        private float _screenWidth = 800f;
#pragma warning restore 0649

        public Action canvasUpdateAction;

        private Rect _prevSafeArea;
        private void UpdateCanvasSize()
        {
            float x = Screen.safeArea.x;
            float y = Screen.safeArea.y;
            float w = Screen.safeArea.width;
            float h = Screen.safeArea.height;

            float deviceFactor = h / w;
            if (deviceFactor < _factorRange.x)
            {
                float w2 = h / _factorRange.x;
                x += (w - w2) / 2;
                w = w2;
            }
            else if (deviceFactor > _factorRange.y)
            {
                float h2 = _factorRange.y * w;
                y += (h - h2) / 2;
                h = h2;
            }
            x /= Screen.width;
            y /= Screen.height;
            w /= Screen.width;
            h /= Screen.height;

            RectTransform rect = GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(x, y);
            rect.anchorMax = new Vector2(x + w, y + h);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

/*
            Debug.LogFormat("device factor {0}", deviceFactor);
            Debug.LogFormat(
                "RectTransform rect {0}", rect.rect);
            Debug.LogFormat(
                "RectTransform anchorMin {0}", rect.anchorMin);
            Debug.LogFormat(
                "RectTransform anchorMax {0}", rect.anchorMax);
            Debug.LogFormat(
                "RectTransform offsetMin {0}", rect.offsetMin);
            Debug.LogFormat(
                "RectTransform offsetMax {0}", rect.offsetMax);
*/
            float scaledWidth = _screenWidth / w;
            CanvasScaler scaler = GetComponentInParent<CanvasScaler>();
            scaler.referenceResolution = new Vector2(scaledWidth, scaledWidth);
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
                canvasUpdateAction?.Invoke();
            }
        }
    }
}
