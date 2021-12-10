using System;
using TMPro;
using UI.Panels.Templates;
using UnityEngine;

namespace UI.Panels
{
    public class SimpleNumberInputPanel : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField]
        private TextMeshProUGUI _textMessage;
        [SerializeField]
        private TMP_InputField _textInput;
#pragma warning restore 0649

        public TextMeshProUGUI text => _textMessage;
        public TMP_InputField input => _textInput;
        public int maxValue = int.MaxValue;
        public int minValue = 0;
        public int defaultValue = 0;

        private int _number;
        private int number
        {
            get => _number;
            set
            {
                _number = Mathf.Clamp(value, minValue, maxValue);
                _textInput.text = _number.ToString();
                _textInput.textComponent.alignment = TextAlignmentOptions.Left;
            }
        }

        public string message
        {
            set
            {
                _textMessage.text = value;
            }
        }

        public Action<ButtonType, int> numberInputCallback
        {
            set
            {
                GetComponent<MessageBox>().messageBoxHandler =
                    (ButtonType t) => value(t, number);
            }
        }

        public Action<TransactionHandler, int> startTransactionHandler
        {
            set
            {
                GetComponent<MessageBox>().startTransactionHandler =
                    (TransactionHandler handler) => value(handler, number);
            }
        }

        public Func<ButtonType, int, string> confirmMessageHandler
        {
            set
            {
                GetComponent<MessageBox>().confirmMessageHandler =
                    (ButtonType t) => value(t, number);
            }
        }

        public void Refresh()
        {
            number = defaultValue;
        }

        private void OnEnable()
        {
            Refresh();
        }

        public void OnNumberChanged()
        {
            if (int.TryParse(_textInput.text, out int result))
            {
                number = Mathf.Min(result, maxValue);
            }
            else
            {
                number = number;
            }
        }
    }
}
