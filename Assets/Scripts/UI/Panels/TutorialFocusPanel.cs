using Actions;
using PlayerInfo;
using ScriptableObjects;
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

        public Vector3 focusPosition
        {
            get => _tutorialFocusMask.transform.position;
            set
            {
                _tutorialFocusMask.transform.position = value;
            }
        }
        public TextMeshProUGUI text => _tutorialText;

        private SafeAreaCanvas _parentCanvas;

        private void Awake()
        {
            _parentCanvas = GetComponentInParent<SafeAreaCanvas>();
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
