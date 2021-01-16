using Assets;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Panels.Assets
{
    public class AssetLoanControlPanel : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField]
        private Slider _slider;
#pragma warning restore 0649

        public Action<int> callback;

        public int minValue
        {
            get { return Mathf.FloorToInt(_slider.minValue); }
            set { _slider.minValue = value; }
        }

        public int maxValue
        {
            get { return Mathf.FloorToInt(_slider.maxValue); }
            set { _slider.maxValue = value; }
        }

        public int value
        {
            get { return Mathf.FloorToInt(_slider.value); }
            set { _slider.value = value; }
        }
    }
}
