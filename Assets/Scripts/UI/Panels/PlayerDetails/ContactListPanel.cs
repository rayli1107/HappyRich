using PlayerInfo;
using System.Collections.Generic;
using UI.Panels.Templates;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Panels.PlayerDetails
{
    public class ContactListPanel : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField]
        private ContactPanel _prefabContactPanel;
#pragma warning restore 0649

        public Player player;
        public ContactSelectCallback callback;
        public List<InvestmentPartner> partners;

        public void Refresh()
        {
            if (player == null)
            {
                return;
            }

            Transform content = GetComponentInChildren<ScrollRect>(true).content.transform;
            foreach (InvestmentPartner partner in player.contacts)
            {
                ContactPanel panel = Instantiate(_prefabContactPanel, content);
                panel.player = player;
                panel.partner = partner;
                panel.callback = callback;
                panel.Refresh();
            }

            if (content.childCount == 0)
            {
                UIManager.Instance.ShowSimpleMessageBox(
                    "You don't have any professional contacts yet.", ButtonChoiceType.OK_ONLY, null);
                GetComponent<MessageBox>().Destroy();
                return;
            }
        }

        private void OnEnable()
        {
            Refresh();
        }
    }
}
