using Actions;
using PlayerInfo;
using ScriptableObjects;
using TMPro;
using UI.Panels.Templates;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Panels
{
    public class PlayerSnapshotPanel : ModalObject
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
        private Button _buttonPlayerDetails;
        [SerializeField]
        private Button _buttonAssets;
        [SerializeField]
        private Button _buttonIncome;
        [SerializeField]
        private Button _buttonHappiness;
        [SerializeField]
        private Button _buttonFire;
        [SerializeField]
        private Button _buttonJobSearch;
        [SerializeField]
        private Button _buttonNetworking;
        [SerializeField]
        private Button _buttonInvestments;
        [SerializeField]
        private Button _buttonSelfImprovement;
        [SerializeField]
        private Button _buttonBuyInsurance;
        [SerializeField]
        private Button _buttonCancelInsurance;
        [SerializeField]
        private Button _buttonStockBrockerage;
        [SerializeField]
        private Button _buttonEndTurn;
#pragma warning restore 0649

        public Button buttonPlayerDetails => _buttonPlayerDetails;
        public Button buttonAssets =>_buttonAssets;
        public Button buttonIncome => _buttonIncome;
        public Button buttonHappiness => _buttonHappiness;
        public Button buttonFire => _buttonFire;
        public Button buttonJobSearch => _buttonJobSearch;
        public Button buttonNetworking => _buttonNetworking;
        public Button buttonInvestments => _buttonInvestments;
        public Button buttonSelfImprovement => _buttonSelfImprovement;
        public Button buttonBuyInsurance => _buttonBuyInsurance;
        public Button buttonCancelInsurance => _buttonCancelInsurance;
        public Button buttonStockBrockerage => _buttonStockBrockerage;
        public Button buttonEndTurn => _buttonEndTurn;

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
