using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Panels.Templates
{
    public delegate void TextInputCallback(string text);
    public delegate string TextInputConfirmMessageHandler(string text);

    public class SimpleTextPrompt : ModalObject 
    {
#pragma warning disable 0649
        [SerializeField]
        private TextMeshProUGUI _text;
        [SerializeField]
        private TMP_InputField _input;
        [SerializeField]
        private Button _buttonCancel;
#pragma warning restore 0649

        public TextInputCallback textInputCallback;
        public TextInputConfirmMessageHandler confirmMessageHandler;
        public bool requireNonEmpty;

        public string message
        {
            get => _text.text;
            set { _text.text = value; }
        }

        public bool cancelEnabled
        {
            get => _buttonCancel.gameObject.activeSelf;
            set { _buttonCancel.gameObject.SetActive(value); }
        }

        private void onFinish()
        {
            Destroy();
            textInputCallback?.Invoke(_input.text);
        }

        private void confirmMessageBoxHandler(ButtonType buttonType)
        {
            if (buttonType == ButtonType.OK)
            {
                onFinish();
            }
        }

        public void OnButtonOk()
        {
            if (requireNonEmpty && _input.text.Length == 0)
            {
                UIManager.Instance.ShowSimpleMessageBox(
                    "Please enter a valid input.", ButtonChoiceType.OK_ONLY, null);
                return;
            }

            string message = confirmMessageHandler?.Invoke(_input.text);
            if (message == null || message.Length == 0)
            {
                onFinish();
                return;
            }

            UIManager.Instance.ShowSimpleMessageBox(
                message, ButtonChoiceType.OK_CANCEL, confirmMessageBoxHandler);
        }

        public void OnButtonCancel()
        {
            Destroy();
            textInputCallback?.Invoke(null);
        }
    }
}
