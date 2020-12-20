using ScriptableObjects;
using TMPro;
using UnityEngine;
using Actions;

namespace UI.Panels.PlayerDetails
{
    public class ContactPanel : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField]
        private TextMeshProUGUI _textName;
        [SerializeField]
        private TextMeshProUGUI _textDuration;
        [SerializeField]
        private TextMeshProUGUI _textAvailableCash;
        [SerializeField]
        private TextMeshProUGUI _textRiskToleranceHigh;
        [SerializeField]
        private TextMeshProUGUI _textRiskToleranceMedium;
        [SerializeField]
        private TextMeshProUGUI _textRiskToleranceLow;
#pragma warning restore 0649

        public Player player;
        public InvestmentPartner partner;

        public void Refresh()
        {
            if (player == null || partner == null)
            {
                return;
            }

            Localization local = Localization.Instance;
            if (_textName != null)
            {
                _textName.text = local.GetName(partner.name);
            }

            if (_textDuration != null)
            {
                _textDuration.text = partner.duration.ToString();
            }

            if (_textAvailableCash != null)
            {
                _textAvailableCash.text = local.GetCurrency(partner.cash);
            }

            if (_textRiskToleranceHigh != null)
            {
                _textRiskToleranceHigh.gameObject.SetActive(
                    partner.riskTolerance == RiskTolerance.kHigh);
            }

            if (_textRiskToleranceMedium != null)
            {
                _textRiskToleranceMedium.gameObject.SetActive(
                    partner.riskTolerance == RiskTolerance.kMedium);
            }

            if (_textRiskToleranceLow != null)
            {
                _textRiskToleranceLow.gameObject.SetActive(
                    partner.riskTolerance == RiskTolerance.kLow);
            }
        }

        private void OnEnable()
        {
            Refresh();
        }
    }
}
