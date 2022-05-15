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
        [SerializeField]
        private RectTransform _textPanel;
#pragma warning restore 0649

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
                setTextLocation();
            }
        }
        public TextMeshProUGUI text => _tutorialText;

        private SafeAreaCanvas _parentCanvas;

        private void Awake()
        {
            _parentCanvas = GetComponentInParent<SafeAreaCanvas>();
            _tutorialTextAnchorMax = _textPanel.anchorMax;
            _tutorialTextAnchorMaxFlipped = new Vector2(
                _tutorialTextAnchorMax.x, 1 - _tutorialTextAnchorMin.y);
            _tutorialTextAnchorMin = _textPanel.anchorMin;
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

        private void setTextLocation()
        {
            if (_tutorialFocusMask.transform.localPosition.y < 0)
            {
                _textPanel.anchorMax = _tutorialTextAnchorMaxFlipped;
                _textPanel.anchorMin = _tutorialTextAnchorMinFlipped;
            }
            else
            {
                _textPanel.anchorMax = _tutorialTextAnchorMax;
                _textPanel.anchorMin = _tutorialTextAnchorMin;
            }
        }

        private void onCanvasAreaUpdate()
        {
            Rect canvasRect = _parentCanvas.GetComponent<RectTransform>().rect;
            RectTransform maskRect = _cutoutMaskIamge.GetComponent<RectTransform>();
            maskRect.SetSizeWithCurrentAnchors(
                RectTransform.Axis.Horizontal, canvasRect.width * 2);
            maskRect.SetSizeWithCurrentAnchors(
                RectTransform.Axis.Vertical, canvasRect.height * 2);
            setTextLocation();
        }
    }
}
