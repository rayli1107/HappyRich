using System;
using TMPro;
using UI.Panels.Templates;
using UnityEngine;

namespace UI.Panels
{
    public enum NumberValueType
    {
        kPlain,
        kCurrency,
    }

    public delegate void NumberInputCallback(int n);

    public class NumberInputPanel : ModalObject
    {
#pragma warning disable 0649
        [SerializeField]
        private TextMeshProUGUI _textMessage;
        [SerializeField]
        private TextMeshProUGUI _textNumber;
#pragma warning restore 0649

        public string message;
        public int maxValue = int.MaxValue;
        public int defaultValue = 0;
        public NumberInputCallback numberCallback;
        public Action cancelCallback;
        public Action<int, TransactionHandler> startTransactionHandler;
        public Func<bool, int, string> confirmMessageHandler;
        public NumberValueType valueType = NumberValueType.kPlain;

        private int _number;

        public void Refresh()
        {
            _textMessage.text = message;
            OnClear();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Refresh();
        }

        private void setNumberText(int number)
        {
            Localization local = Localization.Instance;
            if (valueType == NumberValueType.kCurrency)
            {
                _textNumber.text = local.GetCurrency(number);
            }
            else
            {
                _textNumber.text = number.ToString();
            }
        }

        public void OnNumberChanged()
        {

        }

        public void OnNumberInput(int n)
        {
            try
            {
                _number = Mathf.Min(checked(_number * 10 + n), maxValue);
                setNumberText(_number);
            }
            catch (OverflowException)
            {
            }
        }

        public void OnClear()
        {
            _number = 0;
            setNumberText(_number);
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
            string message = confirmMessageHandler?.Invoke(true, _number);
            if (message == null || message.Length == 0)
            {
                tryFinish();
            }
            else
            {
                UIManager.Instance.ShowSimpleMessageBox(
                    message, ButtonChoiceType.OK_CANCEL, handleConfirm);
            }
        }

        private void onCancel()
        {
            cancelCallback?.Invoke();
            Destroy();
        }

        private void handleCancel(ButtonType buttonType)
        {
            if (buttonType == ButtonType.OK)
            {
                onCancel();
            }
        }

        public void OnCancel()
        {
            string message = confirmMessageHandler?.Invoke(false, 0);
            if (message == null || message.Length == 0)
            {
                onCancel();
            }
            else
            {
                UIManager.Instance.ShowSimpleMessageBox(
                    message, ButtonChoiceType.OK_CANCEL, handleCancel);
            }
        }
    }
}
