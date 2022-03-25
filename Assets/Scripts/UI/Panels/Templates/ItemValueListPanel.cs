using UnityEngine;

namespace UI.Panels.Templates
{
    public class ItemValueListPanel : MonoBehaviour
    {
        public ItemValuePanel firstItemValuePanel { get; private set; }

        public int itemCount => transform.childCount - 1;

        private void Awake()
        {
            firstItemValuePanel = GetComponentInChildren<ItemValuePanel>();
        }

        public ItemValuePanel AddItem(string label, int tabCount)
        {
            ItemValuePanel newPanel = Instantiate(
                firstItemValuePanel, firstItemValuePanel.transform.parent);
            newPanel.tabCount = tabCount;
            newPanel.label = label;
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
    }
}
