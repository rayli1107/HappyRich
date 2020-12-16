﻿using UnityEngine;
using UI.Panels.Templates;

namespace UI.Panels.Actions
{
    public class ActionMenuPanel : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField]
        private JobSearchPanel _prefabJobSearchPanel;
        [SerializeField]
        private InvestmentsPanel _prefabInvestmentsPanel;
#pragma warning restore 0649

        public Player player;

        public void OnJobSearchButton()
        {
            Instantiate(_prefabJobSearchPanel, UIManager.Instance.transform).player = player;
        }

        public void OnInvestmentsButton()
        {
            Instantiate(_prefabInvestmentsPanel, UIManager.Instance.transform).player = player;
        }
    }
}
