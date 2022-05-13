using Actions;
using PlayerInfo;
using ScriptableObjects;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Panels
{
    public class TutorialFocusPanel : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField]
        private Mask _tutorialFocusMask;
        [SerializeField]
        private CutoutMaskImage _cutoutMaskIamge;
        [SerializeField]
        private TextMeshProUGUI _tutorialText;
#pragma warning restore 0649

        private RectTransform _tutorialTextRect;
        private Vector2 _tutorialTextAnchorMax;
        private Vector2 _tutorialTextAnchorMin;
        private Vector2 _tutorialTextAnchorMaxFlipped;
        private Vector2 _tutorialTextAnchorMinFlipped;

        public Vector3 focusPosition
        {
            get => _tutorialFocusMask.transform.position;
            set
            {
                _tutorialFocusMask.transform.position = value;

                RectTransform rectTransform = _tutorialText.GetComponent<RectTransform>();
                if (_tutorialFocusMask.transform.localPosition.y < 0)
                {
                    rectTransform.anchorMax = _tutorialTextAnchorMaxFlipped;
                    rectTransform.anchorMin = _tutorialTextAnchorMinFlipped;
                }
                else
                {
                    rectTransform.anchorMax = _tutorialTextAnchorMax;
                    rectTransform.anchorMin = _tutorialTextAnchorMin;
                }
            }
        }
        public TextMeshProUGUI text => _tutorialText;

        private SafeAreaCanvas _parentCanvas;

        private void Awake()
        {
            _parentCanvas = GetComponentInParent<SafeAreaCanvas>();
            _tutorialTextRect = _tutorialText.GetComponent<RectTransform>();
            _tutorialTextAnchorMax = _tutorialTextRect.anchorMax;
            _tutorialTextAnchorMaxFlipped = new Vector2(
                _tutorialTextAnchorMax.x, 1 - _tutorialTextAnchorMin.y);
            _tutorialTextAnchorMin = _tutorialTextRect.anchorMin;
            _tutorialTextAnchorMinFlipped = new Vector2(
                _tutorialTextAnchorMin.x, 1 - _tutorialTextAnchorMax.y);
        }

        private void OnEnable()
        {
            _parentCanvas.canvasUpdateAction += onCanvasAreaUpdate;
            onCanvasAreaUpdate();
        }

        private void OnDisable()
        {
            _parentCanvas.canvasUpdateAction -= onCanvasAreaUpdate;
        }

        private void onCanvasAreaUpdate()
        {
            Rect canvasRect = _parentCanvas.GetComponent<RectTransform>().rect;
            RectTransform maskRect = _cutoutMaskIamge.GetComponent<RectTransform>();
            maskRect.SetSizeWithCurrentAnchors(
                RectTransform.Axis.Horizontal, canvasRect.width * 2);
            maskRect.SetSizeWithCurrentAnchors(
                RectTransform.Axis.Vertical, canvasRect.height * 2);
        }
    }
}
