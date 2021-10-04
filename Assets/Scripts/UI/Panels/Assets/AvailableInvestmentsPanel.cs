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
    using GetInvestmentFn = Func<Action<int, bool>, List<AbstractBuyInvestmentAction>>;

    public class AvailableInvestmentsPanel : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField]
        protected TextMeshProUGUI _textAvailableCash;
        [SerializeField]
        protected Button  _prefabInvestmentButton;
#pragma warning restore 0649

        public Player player;
        private List<Button> buyActionButtons;

        private void Awake()
        {
            GetComponent<MessageBox>().confirmMessageHandler = GetConfirmMessage;
        }

        private void buyCallback(int index, bool success)
        {
            if (success && index < buyActionButtons.Count)
            {
                buyActionButtons[index].gameObject.SetActive(false);
            }

            foreach (Button button in buyActionButtons)
            {
                if (button.gameObject.activeInHierarchy)
                {
                    return;
                }
            }

            GetComponent<MessageBox>().OnButtonCancel();
        }


        public virtual void Initialize(GetInvestmentFn getInvestmentFunction)
        {
            Localization local = Localization.Instance;
            _textAvailableCash.text = string.Format(
                "Available Cash: {0}", local.GetCurrency(player.cash));

            Transform parentTransform = GetComponentInChildren<VerticalLayoutGroup>().transform;

            foreach (Button button in GetComponentsInChildren<Button>())
            {
                if (button.transform.parent == parentTransform)
                {
                    button.transform.parent = null;
                    GameObject.Destroy(button);
                }
            }

            buyActionButtons = new List<Button>();
            List<AbstractBuyInvestmentAction> buyActions = getInvestmentFunction(buyCallback);
            for (int i = 0; i < buyActions.Count; ++i)
            {
                AbstractBuyInvestmentAction buyAction = buyActions[i];
                Button button = Instantiate(_prefabInvestmentButton, parentTransform);
                button.GetComponentInChildren<TextMeshProUGUI>().text = string.Format(
                    "{0}\nCost: {1}",
                    buyAction.asset.label,
                    local.GetCurrency(buyAction.asset.originalPrice));
                button.onClick.AddListener(new UnityEngine.Events.UnityAction(buyAction.Start));
                button.transform.SetSiblingIndex(i + 1);
                button.gameObject.SetActive(true);
                buyActionButtons.Add(button);
            }
        }

        private string GetConfirmMessage(ButtonType buttonType)
        {
            foreach (Button button in buyActionButtons)
            {
                if (button.gameObject.activeInHierarchy)
                {
                    return "Pass on these investment opportunities?";
                }
            }
            return "";
        }

    }
}
