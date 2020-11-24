using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Panels
{
    public enum ButtonChoiceType
    {
        OK_ONLY,
        OK_CANCEL,
        CANCEL_ONLY,
        BACK_ONLY,
        NONE
    }

    public class MessageBox : ModalObject
    {
        public IMessageBoxHandler messageBoxHandler;
        public RectTransform childRect;
        public ButtonChoiceType buttonChoice = ButtonChoiceType.OK_ONLY;

#pragma warning disable 0649
        [SerializeField]
        private RectTransform _panelButtons;
        [SerializeField]
        private RectTransform _buttonOk;
        [SerializeField]
        private RectTransform _buttonCancel;
        [SerializeField]
        private RectTransform _buttonBack;
#pragma warning restore 0649

        protected override void OnEnable()
        {
            base.OnEnable();

            List<RectTransform> buttons = new List<RectTransform>();
            switch (buttonChoice)
            {
                case ButtonChoiceType.BACK_ONLY:
                    buttons.Add(_buttonBack);
                    break;
                case ButtonChoiceType.CANCEL_ONLY:
                    buttons.Add(_buttonCancel);
                    break;
                case ButtonChoiceType.OK_CANCEL:
                    buttons.Add(_buttonOk);
                    buttons.Add(_buttonCancel);
                    break;
                case ButtonChoiceType.OK_ONLY:
                    buttons.Add(_buttonOk);
                    break;
                default:
                    break;
            }

            RectTransform rect = GetComponent<RectTransform>();
            rect.sizeDelta = childRect.sizeDelta;

            if (buttons.Count > 0)
            {
                rect.sizeDelta = new Vector2(
                    childRect.sizeDelta.x,
                    childRect.sizeDelta.y + _panelButtons.sizeDelta.y);
                _panelButtons.gameObject.SetActive(true);

                float deltaX = 1.0f / buttons.Count;
                for (int i = 0; i < buttons.Count; ++i)
                {
                    RectTransform rectButton = buttons[i];
                    rectButton.gameObject.SetActive(true);
                    rectButton.anchorMin = new Vector2(i * deltaX, 0);
                    rectButton.anchorMax = new Vector2((i + 1) * deltaX, 1);
                }
            }
        }

        public void OnButtonOk()
        {
            if (messageBoxHandler != null)
            {
                messageBoxHandler.OnButtonClick(ButtonType.OK);
            }
            Destroy(gameObject);
        }

        public void OnButtonCancel()
        {
            if (messageBoxHandler != null)
            {
                messageBoxHandler.OnButtonClick(ButtonType.CANCEL);
            }
            Destroy(gameObject);
        }

        public override void OnClickOutsideBoundary()
        {
            if (_enableInput)
            {
                if (messageBoxHandler != null)
                {
                    messageBoxHandler.OnButtonClick(ButtonType.OUTSIDE_BOUNDARY);
                }
                Destroy(gameObject);
            }
        }
    }
}
