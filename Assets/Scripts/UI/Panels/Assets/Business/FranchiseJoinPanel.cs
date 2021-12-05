using Assets;
using PlayerInfo;
using ScriptableObjects;
using System;
using TMPro;
using UI.Panels.Templates;
using UnityEngine;

namespace UI.Panels.Assets
{
    public class FranchiseJoinPanel : AbstractBusinessPurchasePanel
    {
#pragma warning disable 0649
        [SerializeField]
        protected TextMeshProUGUI _textFranchiseFee;
#pragma warning restore 0649

        private Franchise _franchise;
        public Franchise franchise
        {
            get => _franchise;
            set
            {
                _franchise = value;
                _business = value;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            GetComponent<MessageBox>().confirmMessageHandler = GetConfirmMessage;
        }

        public override void Refresh()
        {
            base.Refresh();

            if (player == null || asset == null)
            {
                return;
            }

            Localization local = Localization.Instance;
            if (_textFranchiseFee != null)
            {
                _textFranchiseFee.text = local.GetCurrency(franchise.franchiseFee, true);
            }
        }

        public void SwitchView(bool advanced)
        {
            MessageBox messageBox = GetComponent<MessageBox>();
            messageBox.Destroy();
            UIManager.Instance.ShowFranchiseJoinPanel(
                franchise,
                partialAsset,
                messageBox.messageBoxHandler,
                messageBox.startTransactionHandler,
                advanced);
        }

        private string GetConfirmMessage(ButtonType buttonType)
        {
            Localization local = Localization.Instance;
            if (buttonType == ButtonType.OK)
            {
                return string.Format(
                    "Join the {0} franchise for {1}?",
                    local.GetBusinessDescription(asset.description),
                    local.GetCurrency(asset.totalCost, true));
            }
            return "";
        }
    }
}
