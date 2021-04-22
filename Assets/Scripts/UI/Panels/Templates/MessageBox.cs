using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Panels.Templates
{
    public enum ButtonType
    {
        OK,
        CANCEL,
        OUTSIDE_BOUNDARY
    }

    public enum ButtonChoiceType
    {
        OK_ONLY,
        OK_CANCEL,
        CANCEL_ONLY,
        BACK_ONLY,
        NONE
    }

    public delegate void MessageBoxHandler(ButtonType buttonType);
    public delegate string GetConfirmMessageCallback(ButtonType buttonType);

    public class MessageBox : ModalObject
    {
        public MessageBoxHandler messageBoxHandler;
        public GetConfirmMessageCallback confirmMessageHandler;

#pragma warning disable 0649
        [SerializeField]
        private GameObject _buttonOk;
        [SerializeField]
        private GameObject _buttonCancel;
        [SerializeField]
        private GameObject _buttonBack;
#pragma warning restore 0649

        private void onFinish(ButtonType buttonType)
        {
            messageBoxHandler?.Invoke(buttonType);
            Destroy();
        }

        private void handleConfirm(ButtonType buttonType, ButtonType actualButtonType)
        {
            if (buttonType == ButtonType.OK)
            {
                onFinish(actualButtonType);
            }
        }

        private void handleButton(ButtonType buttonType)
        {
            string message = confirmMessageHandler?.Invoke(buttonType);
            if (message == null || message.Length == 0)
            {
                onFinish(buttonType);
            }
            else
            {
                UIManager.Instance.ShowSimpleMessageBox(
                    message,
                    ButtonChoiceType.OK_CANCEL,
                    (ButtonType t) => handleConfirm(t, buttonType));
            }
        }

        public virtual void OnButtonOk()
        {
            handleButton(ButtonType.OK);
        }

        public virtual void OnButtonCancel()
        {
            handleButton(ButtonType.CANCEL);
        }

        public void EnableButtons(ButtonChoiceType buttonChoice)
        {
            switch (buttonChoice)
            {
                case ButtonChoiceType.BACK_ONLY:
                    _buttonBack.gameObject.SetActive(true);
                    break;
                case ButtonChoiceType.CANCEL_ONLY:
                    _buttonCancel.gameObject.SetActive(true);
                    break;
                case ButtonChoiceType.OK_CANCEL:
                    _buttonOk.gameObject.SetActive(true);
                    _buttonCancel.gameObject.SetActive(true);
                    break;
                case ButtonChoiceType.OK_ONLY:
                    _buttonOk.gameObject.SetActive(true);
                    break;
                default:
                    break;
            }
        }
    }
}
