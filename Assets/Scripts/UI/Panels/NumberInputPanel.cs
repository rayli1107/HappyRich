using TMPro;
using UI.Panels.Templates;
using UnityEngine;

namespace UI.Panels
{
    public interface INumberInputCallback
    {
        void OnNumberInput(NumberInputPanel messageBox, int number);
        void OnNumberInputCancel(NumberInputPanel messageBox);
    }

    public class NumberInputPanel : MonoBehaviour, IMessageBoxHandler
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

        private void OnEnable()
        {
            _textMessage.text = message;
            _textMax.text = string.Format("Max: {0}", max);
            OnClear();
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

        public void Destroy()
        {
            GetComponentInParent<MessageBox>().Destroy();
        }

        public void OnConfirm()
        {
            if (callback != null)
            {
                callback.OnNumberInput(this, _number);
            }
            else
            {
                Destroy();
            }
        }

        public void OnCancel()
        {
            if (callback != null)
            {
                callback.OnNumberInputCancel(this);
            }
            else
            {
                Destroy();
            }
        }

        public void OnButtonClick(MessageBox msgBox, ButtonType button)
        {
            OnCancel();
        }
    }
}
