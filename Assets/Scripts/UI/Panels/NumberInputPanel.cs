using TMPro;
using UI.Panels.Templates;
using UnityEngine;

namespace UI.Panels
{
    public interface INumberInputCallback
    {
        void OnNumberInput(int number);
        void OnNumberInputCancel();
    }

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
        public INumberInputCallback callback;

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


        public void OnConfirm()
        {
            if (callback != null)
            {
                callback.OnNumberInput(_number);
            }
            Destroy();
        }

        public void OnCancel()
        {
            if (callback != null)
            {
                callback.OnNumberInputCancel();
            }
            Destroy();
        }
    }
}
