using TMPro;
using UnityEngine;
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
        private Color _positiveColor = Color.green;
        [SerializeField]
        private Color _negativeColor = Color.red;
        [SerializeField]
        private float tabWidth = 40;
#pragma warning restore 0649

        public bool colorFlip = false;
        public int tabCount { get; private set; }

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

        public void setValueAsCurrency(int i)
        {
            Localization local = GameManager.Instance.Localization;
            _value.gameObject.SetActive(true);
            _value.text = local.GetCurrency(i);

            bool positive = i >= 0;
            if (colorFlip)
            {
                positive = !positive;
            }
            _value.color = positive ? _positiveColor : _negativeColor;
        }
        private void OnEnable()
        {
            tabCount = 0;
        }
    }
}
