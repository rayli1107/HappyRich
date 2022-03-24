using UnityEngine;

namespace UI.Panels.Templates
{
    public class ItemValueListPanel : MonoBehaviour
    {
        public ItemValuePanel firstItemValuePanel { get; private set; }

        public int itemCount => GetComponentsInChildren<ItemValuePanel>().Length - 1;

        private void Awake()
        {
            firstItemValuePanel = GetComponentInChildren<ItemValuePanel>();
        }

        public ItemValuePanel AddItemValue(string label, int tabCount)
        {
            ItemValuePanel newPanel = Instantiate(
                firstItemValuePanel, firstItemValuePanel.transform.parent);
            newPanel.tabCount = tabCount;
            newPanel.label = label;
            return newPanel;
        }

        public ItemValuePanel AddItemValueAsPlain(string label, int tabCount, int value)
        {
            ItemValuePanel newPanel = AddItemValue(label, tabCount);
            newPanel.SetValuePlain(value);
            return newPanel;
        }

        public ItemValuePanel AddItemValueAsCurrency(string label, int tabCount, int value, bool flipped=false)
        {
            ItemValuePanel newPanel = AddItemValue(label, tabCount);
            newPanel.SetValueAsCurrency(value, flipped);
            return newPanel;
        }
    }
}
