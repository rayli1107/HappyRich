using System;
using TMPro;
using UI.Panels.Templates;
using UnityEngine;

namespace UI.Panels
{
    public delegate void NumberInputCallback(int n);

    public class NumberInputPanel : ModalObject
    {
#pragma warning disable 0649
        [SerializeField]
        private TextMeshProUGUI _textMessage;
        [SerializeField]
        private TextMeshProUGUI _textMax;
        [SerializeField]
        private TextMeshProUGUI _textNumber;
#pragma warning restore 0649

        public string message;
        public int max = int.MaxValue;
        public NumberInputCallback numberCallback;
        public Action cancelCallback;
        public Action<int, TransactionHandler> startTransactionHandler;
        public Func<int, string> confirmMessageHandler;

        private int _number;

        public void Refresh()
        {
            _textMessage.text = message;
            _textMax.text = max.ToString();
            OnClear();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Refresh();
        }

        public void OnNumberInput(int n)
        {
            try
            {
                _number = Mathf.Min(checked(_number * 10 + n), max);
                _textNumber.text = _number.ToString();
            }
            catch (System.OverflowException)
            {
            }
        }

        public void OnClear()
        {
            _number = 0;
            _textNumber.text = _number.ToString();
        }

        private void onFinish()
        {
            numberCallback?.Invoke(_number);
            Destroy();
        }

        private void transactionHandler(bool success)
        {
            if (success)
            {
                onFinish();
            }
        }

        private void tryFinish()
        {
            if (startTransactionHandler != null)
            {
                startTransactionHandler(_number, (bool b) => transactionHandler(b));
            }
            else
            {
                onFinish();
            }
        }

        private void handleConfirm(ButtonType buttonType)
        {
            if (buttonType == ButtonType.OK)
            {
                tryFinish();
            }
        }

        public void OnConfirm()
        {
            string message = confirmMessageHandler?.Invoke(_number);
            if (message == null || message.Length == 0)
            {
                tryFinish();
            }
            else
            {
                UIManager.Instance.ShowSimpleMessageBox(
                    message,
                    ButtonChoiceType.OK_CANCEL,
                    (ButtonType t) => handleConfirm(t));
            }
        }

        public void OnCancel()
        {
            cancelCallback?.Invoke();
            Destroy();
        }
    }
}
