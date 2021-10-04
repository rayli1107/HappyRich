using PlayerInfo;
using ScriptableObjects;
using System.Collections.Generic;
using TMPro;
using UI.Panels.Templates;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI.Panels.PlayerDetails
{
    public class ContactListPanel : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField]
        private Transform _labelSpecialists;
        [SerializeField]
        private Transform _labelInvestors;
        [SerializeField]
        private Transform _prefabSpecialist;
        [SerializeField]
        private ContactPanel _prefabContactPanel;
#pragma warning restore 0649

        public Player player;
        public ContactSelectCallback callback;
        public List<InvestmentPartner> partners;

        private void onSpecialistClick(SpecialistInfo info)
        {
            Localization local = Localization.Instance;
            string message = string.Format(
                "A {0} {1}", local.GetSpecialist(info), info.specialistDescription);
            UI.UIManager.Instance.ShowSimpleMessageBox(
                message, ButtonChoiceType.OK_ONLY, null);
        }

        public void Refresh()
        {
            if (player == null)
            {
                return;
            }
            Localization local = Localization.Instance;

            if (_labelSpecialists != null)
            {
                _labelSpecialists.gameObject.SetActive(player.specialists.Count > 0);
                int index = _labelSpecialists.GetSiblingIndex();
                foreach (SpecialistInfo info in player.specialists)
                {
                    ++index;
                    Transform transformSpecialist = Instantiate(
                        _prefabSpecialist, _labelSpecialists.transform.parent);
                    transformSpecialist.GetComponentInChildren<TextMeshProUGUI>().text = local.GetSpecialist(info);
                    transformSpecialist.GetComponentInChildren<Button>().onClick.AddListener(
                        () => onSpecialistClick(info));
                    transformSpecialist.gameObject.SetActive(true);
                    transformSpecialist.SetSiblingIndex(index);
                }
            }

            if (_labelInvestors != null)
            {
                _labelInvestors.gameObject.SetActive(player.contacts.Count > 0);
                int index = _labelInvestors.GetSiblingIndex();
                foreach (InvestmentPartner partner in player.contacts)
                {
                    ++index;
                    ContactPanel panel = Instantiate(
                        _prefabContactPanel, _labelInvestors.transform.parent);
                    panel.player = player;
                    panel.partner = partner;
                    panel.callback = callback;
                    panel.transform.SetSiblingIndex(index);
                    panel.Refresh();
                }
            }
        }

        private void OnEnable()
        {
            Refresh();
        }
    }
}
