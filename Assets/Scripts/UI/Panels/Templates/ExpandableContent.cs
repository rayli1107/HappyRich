using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Panels.Templates
{
    public class ExpandableContent : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField]
        private Button _buttonExpand;
        [SerializeField]
        private Button _buttonShrink;
        [SerializeField]
        private GameObject _panelContent;
        [SerializeField]
        private bool _startExpanded;
#pragma warning restore 0649

        private void OnEnable()
        {
            if (_startExpanded)
            {
                Expand();
            }
            else
            {
                Shrink();
            }
        }

        private void RefreshParents()
        {
            ScrollRect scrollRect = GetComponentInParent<ScrollRect>();
            LayoutRebuilder.ForceRebuildLayoutImmediate(scrollRect.content);
        }

        public void Expand()
        {
            _buttonExpand.gameObject.SetActive(false);
            _buttonShrink.gameObject.SetActive(true);
            _panelContent.SetActive(true);
            RefreshParents();
        }

        public void Shrink()
        {
            _buttonExpand.gameObject.SetActive(true);
            _buttonShrink.gameObject.SetActive(false);
            _panelContent.SetActive(false);
            RefreshParents();
        }

    }
}
