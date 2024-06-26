﻿using Assets;
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
        [SerializeField]
        private float _multiplier = 1f;
#pragma warning restore 0649

        public Action<int> callback;

        public int minValue
        {
            get { return Mathf.FloorToInt(_slider.minValue * _multiplier); }
            set { _slider.minValue = value / _multiplier; }
        }

        public int maxValue
        {
            get { return Mathf.FloorToInt(_slider.maxValue * _multiplier); }
            set { _slider.maxValue = value / _multiplier; }
        }

        public int value
        {
            get { return Mathf.FloorToInt(_slider.value * _multiplier); }
            set { _slider.value = value / _multiplier; }
        }
    }
}
