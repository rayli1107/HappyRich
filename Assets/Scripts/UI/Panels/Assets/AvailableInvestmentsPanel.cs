using Actions;
using Assets;
using PlayerInfo;
using ScriptableObjects;
using System;
using System.Collections.Generic;
using TMPro;
using UI.Panels.Templates;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Panels.Assets
{
    public struct AvailableInvestmentContext
    {
        public AbstractInvestment asset { get; private set; }
        public Action<Action<bool>> buyAction { get; private set; }

        public AvailableInvestmentContext(
            AbstractInvestment asset,
            Action<Action<bool>> buyAction)
        {
            this.asset = asset;
            this.buyAction = buyAction;
        }
    }

    public class AvailableInvestmentsPanel : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField]
        private Transform _content;
        [SerializeField]
        private ItemValuePanel _panelAvailableCash;
        [SerializeField]
        private ItemValuePanel _panelAvailableInvestorCash;
        [SerializeField]
        private ItemValueListPanel  _prefabActionButton;
#pragma warning restore 0649

        public Player player;
        public List<ItemValueListPanel> buyActionPanels { get; private set; }
        private bool _investmentsAvailable =>
            buyActionPanels.Exists(b => b.gameObject.activeInHierarchy);

        private void Awake()
        {
            GetComponent<MessageBox>().confirmMessageHandler = GetConfirmMessage;
        }

        private void buyCallback(int index, bool success)
        {
            if (success && index < buyActionPanels.Count)
            {
                buyActionPanels[index].gameObject.SetActive(false);
            }

            if (!_investmentsAvailable)
            {
                GetComponent<MessageBox>().OnButtonCancel();
            }
            else
            {
                Refresh();
            }
        }

        public void Refresh()
        {
            Localization local = Localization.Instance;
            if (_panelAvailableCash != null)
            {
                _panelAvailableCash.SetValue(
                    local.GetCurrency(player.cash));
            }

            if (_panelAvailableInvestorCash != null)
            {
                int investorCash = 0;
                player.GetPartners().ForEach(p => { investorCash += p.cash; });
                _panelAvailableInvestorCash.gameObject.SetActive(investorCash > 0);
                _panelAvailableInvestorCash.SetValue(local.GetCurrency(investorCash));
            }
        }

        public virtual void Initialize(
            List<AvailableInvestmentContext> buyActions)
        {
            foreach (ItemValueListPanel panel in
                _content.GetComponentsInChildren<ItemValueListPanel>())
            {
                panel.transform.parent = null;
                GameObject.Destroy(panel);
            }

            Localization local = Localization.Instance;
            buyActionPanels = new List<ItemValueListPanel>();
            for (int i = 0; i < buyActions.Count; ++i)
            {
                int index = i;
                Action buyAction = () => buyActions[index].buyAction(
                    success => buyCallback(index, success));
                AbstractInvestment asset = buyActions[index].asset;

                ItemValueListPanel panel = Instantiate(_prefabActionButton, _content);
                int tabCount = panel.firstItemValuePanel.tabCount + 1;
                panel.firstItemValuePanel.label = asset.investmentType;
                panel.AddItem(asset.label, tabCount);
                panel.AddItemValue("Total Cost", tabCount, local.GetCurrency(asset.totalCost));
                panel.buttonAction = buyAction;
                panel.gameObject.SetActive(true);
                buyActionPanels.Add(panel);
            }

            Refresh();
        }

        private string GetConfirmMessage(ButtonType buttonType)
        {
            return _investmentsAvailable ? "Pass on these opportunities?" : "";
        }
    }
}
