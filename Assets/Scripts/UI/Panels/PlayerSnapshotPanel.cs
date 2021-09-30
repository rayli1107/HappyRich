using PlayerInfo;
using ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Panels
{
    public class PlayerSnapshotPanel : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField]
        private Image _imageProfession;
        [SerializeField]
        private TextMeshProUGUI _textProfession;
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
                _textAge.text = snapshot.age.ToString();
            }

            if (_textHappiness)
            {
                _textHappiness.text = snapshot.happiness.ToString();
            }

            if (_textFI)
            {
                int fi = (100 * snapshot.expectedPassiveIncome) / snapshot.expectedExpenses;
                _textFI.text = string.Format("{0}%", fi);
            }

            if (_textCash)
            {
                _textCash.text = local.GetCurrencyPlain(snapshot.cash);
            }

            if (_textCashflow)
            {
                _textCashflow.text = local.GetCurrency(snapshot.expectedCashflow);
            }

            if (_textNetworth)
            {
                _textNetworth.text = local.GetCurrency(snapshot.netWorth);
            }

            Profession job = player.GetMainJob();
            if (_imageProfession != null)
            {
                _imageProfession.enabled = job != null;
                _imageProfession.sprite = job.image;
            }

            if (_textProfession != null)
            {
                _textProfession.text = job != null ? job.name : "Unemployed";
            }
        }
    }
}
