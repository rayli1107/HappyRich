using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Panels.Templates
{
    public class ItemValueListPanel : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField]
        private ItemValuePanel _firstItemValuePanel;
#pragma warning restore 0649
        public ItemValuePanel firstItemValuePanel => _firstItemValuePanel;

        private int itemCount => transform.childCount - 1;

        public Action buttonAction
        {
            set
            {
                Button button = GetComponent<Button>();
                button.enabled = value != null;
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(
                    new UnityEngine.Events.UnityAction(value));
            }
        }

        public ItemValuePanel AddItem(string label, int tabCount)
        {
            ItemValuePanel newPanel = Instantiate(
                firstItemValuePanel, firstItemValuePanel.transform.parent);
            newPanel.tabCount = tabCount;
            newPanel.label = label;
            newPanel.RemoveValue();
            return newPanel;
        }

        public ItemValuePanel AddItemValue(string label, int tabCount, int value)
        {
            ItemValuePanel newPanel = AddItem(label, tabCount);
            newPanel.SetValue(value);
            return newPanel;
        }

        public ItemValuePanel AddItemValue(string label, int tabCount, string value)
        {
            ItemValuePanel newPanel = AddItem(label, tabCount);
            newPanel.SetValue(value);
            return newPanel;
        }

        public void Clear()
        {
            while (transform.childCount > 1)
            {
                DestroyImmediate(transform.GetChild(1).gameObject);
            }
        }

        public void ActivateIfNonEmpty()
        {
            gameObject.SetActive(itemCount > 0);
        }
    }
}
