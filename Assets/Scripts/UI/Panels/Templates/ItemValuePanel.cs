using TMPro;
using UnityEngine;

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
        private Color _positiveColor = Color.green;
        [SerializeField]
        private Color _negativeColor = Color.red;
#pragma warning restore 0649

        public bool colorFlip = false;

        public void setLabel(string s)
        {
            _label.text = s;
        }

        public void setValueAsCurrency(int i)
        {
            Localization local = GameManager.Instance.Localization;
            _value.text = local.GetCurrency(i);

            bool positive = i >= 0;
            if (colorFlip)
            {
                positive = !positive;
            }
            _value.color = positive ? _positiveColor : _negativeColor;
        }
    }
}
