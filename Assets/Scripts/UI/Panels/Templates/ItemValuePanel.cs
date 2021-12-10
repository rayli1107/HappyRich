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

        public int tabCount { get; private set; }

        private Button _button;

        private void Awake()
        {
            _button = GetComponentInChildren<Button>(true);
            clickAction = null;
        }

        public void setTabCount(int i)
        {
            tabCount = i;
            _tab.minWidth = tabWidth * i;
            _tab.preferredWidth = tabWidth * i;
        }

        public void setLabel(string s)
        {
            _label.text = s;
        }

        public void removeValue()
        {
            _value.gameObject.SetActive(false);
        }

        public void setValueAsCurrency(int i, bool flipped=false)
        {
            Localization local = Localization.Instance;
            _value.gameObject.SetActive(true);
            _value.text = local.GetCurrency(i, flipped);
        }

        public void setValueAsChange(int value)
        {
            _value.gameObject.SetActive(true);
            _value.text = Localization.Instance.GetValueAsChange(value);
        }

        public void setValuePlain(int value)
        {
            _value.gameObject.SetActive(true);
            _value.text = value.ToString();
        }

        private void OnEnable()
        {
            tabCount = 0;
        }
    }
}
