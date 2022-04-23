using Actions;
using PlayerInfo;
using System.Collections.Generic;
using UI.Panels.Templates;
using UnityEngine;

using ItemValueEntry = System.Tuple<
    int, string, UnityEngine.Vector2Int, System.Action>;

namespace UI.Panels.PlayerDetails
{
    public class IncomeExpenseListPanel : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField]
        private ItemValuePanel _panelTotalCashflow;
        [SerializeField]
        private ItemValuePanel _panelFinancialIndependence;
        [SerializeField]
        private ItemValueListPanel _panelActiveIncome;
        [SerializeField]
        private ItemValueListPanel _panelPassiveIncome;
        [SerializeField]
        private ItemValueListPanel _panelExpenses;
        [SerializeField]
        private bool _showTotalValues = false;
#pragma warning restore 0649

        public Player player;
        public Snapshot incomeExpenseSnapshot;

        private void setupItemValueList(
            ItemValueListPanel panel,
            List<ItemValueEntry> entries,
            int totalValue,
            bool flipped = false)
        {
            setupItemValueList(
                panel, entries, new Vector2Int(totalValue, totalValue), flipped);
        }


        private void setupItemValueList(
            ItemValueListPanel panel,
            List<ItemValueEntry> entries,
            Vector2Int totalValue,
            bool flipped = false)
        {
            Localization local = Localization.Instance;
            int tabCount = panel.firstItemValuePanel.tabCount + 1;

            panel.Clear();

            if (_showTotalValues)
            {
                panel.firstItemValuePanel.SetValue(
                    local.GetIncomeRange(totalValue, flipped));
            }
            else
            {
                panel.firstItemValuePanel.RemoveValue();
            }

            foreach (ItemValueEntry entry in entries)
            {
                Vector2Int incomeRange = entry.Item3;
                if (incomeRange == Vector2Int.zero)
                {
                    continue;
                }

                ItemValuePanel childPanel = panel.AddItemValue(
                    entry.Item2,
                    tabCount + entry.Item1,
                    local.GetIncomeRange(entry.Item3, flipped));
                childPanel.clickAction = entry.Item4;
            }
            panel.ActivateIfNonEmpty();
        }

        private void refreshFromSnapshot(Snapshot snapshot)
        {
            Localization local = Localization.Instance;
            _panelTotalCashflow.SetValue(
                local.GetIncomeRange(snapshot.totalCashflowRange));
            _panelFinancialIndependence.SetValue(
                string.Format("{0}%", snapshot.financialIndependenceProgress));

            setupItemValueList(
                _panelActiveIncome,
                snapshot.itemsActiveIncome,
                snapshot.totalActiveIncome);
            setupItemValueList(
                _panelPassiveIncome,
                snapshot.itemsPassiveIncome,
                snapshot.passiveIncomeRange);
            setupItemValueList(
                _panelExpenses,
                snapshot.itemsFixedExpenses,
                snapshot.totalFixedExpenses,
                true);
        }

        public void RefreshContent()
        {
            if (incomeExpenseSnapshot != null)
            {
                refreshFromSnapshot(incomeExpenseSnapshot);
                return;
            }

            if (player == null)
            {
                return;
            }

            refreshFromSnapshot(
                new Snapshot(
                    player,
                    asset => () => asset.OnDetail(player, reloadWindow),
                    liability => () => liability.OnDetail(player, reloadWindow),
                    () => CancelHealthInsurance.Run(player, reloadWindow)));
        }

        private void OnEnable()
        {
            RefreshContent();
        }

        private void reloadWindow()
        {
            GetComponent<MessageBox>().Destroy();
            UIManager.Instance.ShowIncomeExpenseStatusPanel(incomeExpenseSnapshot);
        }
    }
}
