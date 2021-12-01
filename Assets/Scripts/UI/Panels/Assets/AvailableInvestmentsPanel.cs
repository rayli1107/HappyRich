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

using GetInvestmentFn = System.Func<
    PlayerInfo.Player,
    System.Random,
    System.Action<System.Action<bool>>>;
namespace UI.Panels.Assets
{
    public class AvailableInvestmentsPanel : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField]
        protected TextMeshProUGUI _textAvailableCash;
        [SerializeField]
        protected Button  _prefabInvestmentButton;
#pragma warning restore 0649

        public Player player;
        private List<Button> _buyActionButtons;

        private void Awake()
        {
            GetComponent<MessageBox>().confirmMessageHandler = GetConfirmMessage;
        }

        private void buyCallback(int index, bool success)
        {
            if (success && index < _buyActionButtons.Count)
            {
                _buyActionButtons[index].gameObject.SetActive(false);
            }

            if (!_buyActionButtons.Exists(b => b.gameObject.activeInHierarchy))
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
            _textAvailableCash.text = string.Format(
                "Available Cash: {0}", local.GetCurrency(player.cash));
        }

        public virtual void Initialize(
            List<BuyInvestmentContext> buyActions)
        {
            Transform parentTransform = GetComponentInChildren<VerticalLayoutGroup>().transform;

            foreach (Button button in GetComponentsInChildren<Button>())
            {
                if (button.transform.parent == parentTransform)
                {
                    button.transform.parent = null;
                    GameObject.Destroy(button);
                }
            }

            Localization local = Localization.Instance;
            _buyActionButtons = new List<Button>();
            for (int i = 0; i < buyActions.Count; ++i)
            {
                int index = i;
                AbstractInvestment asset = buyActions[index].asset;
                Action buyAction = () => buyActions[index].buyAction(
                    success => buyCallback(index, success));
                Button button = Instantiate(_prefabInvestmentButton, parentTransform);
                button.GetComponentInChildren<TextMeshProUGUI>().text = string.Format(
                    "{0}\nCost: {1}",
                    asset.label,
                    local.GetCurrency(asset.originalPrice));
                button.onClick.AddListener(new UnityEngine.Events.UnityAction(buyAction));
                button.transform.SetSiblingIndex(index + 1);
                button.gameObject.SetActive(true);
                _buyActionButtons.Add(button);
            }

            Refresh();
        }

        private string GetConfirmMessage(ButtonType buttonType)
        {
            return _buyActionButtons.Exists(b => b.gameObject.activeInHierarchy) ?
                "Pass on these investment opportunities?" : "";
        }
    }
}
