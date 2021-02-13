using PlayerInfo;
using TMPro;
using UnityEngine;

namespace UI.Panels
{
    public class PlayerSnapshotPanel : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField]
        private TextMeshProUGUI _textAge;
        [SerializeField]
        private TextMeshProUGUI _textHappiness;
        [SerializeField]
        private TextMeshProUGUI _textFI;
        [SerializeField]
        private TextMeshProUGUI _textCash;
        [SerializeField]
        private TextMeshProUGUI _textCashflow;
        [SerializeField]
        private TextMeshProUGUI _textNetworth;
#pragma warning restore 0649

        public void UpdatePlayerInfo(Player player)
        {
            Localization local = Localization.Instance;
            Snapshot snapshot = new Snapshot(player);

            if (_textAge)
            {
                _textAge.text = string.Format("Age: {0}", snapshot.age);
            }

            if (_textHappiness)
            {
                _textHappiness.text = string.Format("Happy {0}", snapshot.happiness);
            }

            if (_textFI)
            {
                int fi = (100 * snapshot.passiveIncome) / snapshot.expenses;
                _textFI.text = string.Format("FI: {0}%", fi);
            }

            if (_textCash)
            {
                _textCash.text = string.Format(
                    "Cash:\n{0}", local.GetCurrency(snapshot.cash));
            }

            if (_textCashflow)
            {
                _textCashflow.text = string.Format(
                    "Cashflow:\n{0}", local.GetCurrency(snapshot.cashflow));
            }

            if (_textNetworth)
            {
                _textNetworth.text = string.Format(
                    "Net Worth:\n{0}", local.GetCurrency(snapshot.netWorth));
            }
        }
    }
}
