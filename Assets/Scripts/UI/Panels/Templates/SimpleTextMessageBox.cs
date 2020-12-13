using TMPro;
using UnityEngine;

namespace UI.Panels.Templates
{
    public class SimpleTextMessageBox : MessageBox
    {
#pragma warning disable 0649
        [SerializeField]
        private TextMeshProUGUI _text;
#pragma warning restore 0649

        public TextMeshProUGUI text => _text;
    }
}
