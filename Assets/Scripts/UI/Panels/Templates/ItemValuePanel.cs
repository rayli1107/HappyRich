using Actions;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI.Panels.Templates
{
    public class ItemValuePanel : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField]
        private TextMeshProUGUI _label;
        [SerializeField]
        private TextMeshProUGUI _value;
        [SerializeField]
        private LayoutElement _tab;
        [SerializeField]
        private float tabWidth = 40;
#pragma warning restore 0649

        public Action clickAction
        {
            set
            {
                _button.enabled = value != null;
                if (value == null)
                {
                    _button.onClick.RemoveAllListeners();
                }
                else
                {
                    _button.onClick.AddListener(new UnityAction(value));
                }
            }
        }

        private int _tabCount;
        public int tabCount
        {
            get => _tabCount;
            set
            {
                _tabCount = value;
                _tab.minWidth = tabWidth * _tabCount;
                _tab.preferredWidth = tabWidth * _tabCount;
            }
        }

        public string label
        {
            get => _label.text;
            set { _label.text = value; }
        }

        private Button _button;

        private void Awake()
        {
            _button = GetComponentInChildren<Button>(true);
            clickAction = null;
            tabCount = 0;
            RemoveValue();
        }

        public void RemoveValue()
        {
            _value.gameObject.SetActive(false);
        }

        public void SetValue(string value)
        {
            _value.gameObject.SetActive(true);
            _value.text = value;
        }

        public void SetValue(int value)
        {
            SetValue(value.ToString());
        }

        /*
                public void SetValueAsCurrencyPlain(int i, bool flipped = false)
                {
                    SetValue(Localization.Instance.GetCurrency(i, flipped));
                }

                public void SetValueAsCurrency(int i, bool flipped=false)
                {
                    SetValue(Localization.Instance.GetCurrency(i, flipped));
                }

                public void SetValueAsChange(int value)
                {
                    SetValue(Localization.Instance.GetValueAsChange(value));
                }

                public void SetValuePlain(int value)
                {
                    SetValue(value.ToString());
                }
        */
        private void OnEnable()
        {
            tabCount = 0;
        }
    }
}
