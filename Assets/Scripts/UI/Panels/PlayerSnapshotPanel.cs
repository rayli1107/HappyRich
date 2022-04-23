using Actions;
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
        [SerializeField]
        private Button _buttonBuyInsurance;
        [SerializeField]
        private Button _buttonCancelInsurance;
#pragma warning restore 0649

private System.Action _buyInsuranceAction;
        private System.Action _cancelInsuranceAction;

        public void UpdatePlayerInfo(Player player)
        {
            Localization local = Localization.Instance;
            Snapshot snapshot = new Snapshot(player);

            if (_textAge)
            {
                _textAge.text = player.age.ToString();
            }

            if (_textHappiness)
            {
                _textHappiness.text = player.happiness.ToString();
            }

            if (_textFI)
            {
                _textFI.text = string.Format(
                    "{0}%", snapshot.financialIndependenceProgress);
            }

            if (_textCash)
            {
                _textCash.text = local.GetCurrencyPlain(player.cash);
            }

            if (_textCashflow)
            {
                _textCashflow.text = local.GetIncomeRange(snapshot.totalCashflowRange);
            }

            if (_textNetworth)
            {
                _textNetworth.text = local.GetCurrency(snapshot.netWorth);
            }

            Profession job = player.GetMainJob();
            if (_imageProfession != null)
            {
                _imageProfession.enabled = job != null;
                if (job != null)
                {
                    _imageProfession.sprite = job.image;
                }
            }

            if (_textProfession != null)
            {
                _textProfession.text = job != null ? job.name : "Unemployed";
            }

            _buyInsuranceAction =
                () => BuyHealthInsuranceAction.Run(player, () => UpdatePlayerInfo(player));
            _cancelInsuranceAction =
                () => CancelHealthInsurance.Run(player, () => UpdatePlayerInfo(player));

            if (_buttonBuyInsurance != null)
            {
                _buttonBuyInsurance.gameObject.SetActive(
                    player != null && !player.portfolio.hasHealthInsurance);
            }

            if (_buttonCancelInsurance != null)
            {
                _buttonCancelInsurance.gameObject.SetActive(
                    player != null && player.portfolio.hasHealthInsurance);
            }
        }


        public void OnBuyInsurance()
        {
            _buyInsuranceAction?.Invoke();
        }

        public void OnCancelInsurance()
        {
            _cancelInsuranceAction?.Invoke();
        }
    }
}
