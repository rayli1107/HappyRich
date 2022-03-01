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
    public struct AvailableActionContext
    {
        public string label { get; private set; }
        public Action<Action<bool>> buyAction { get; private set; }

        public AvailableActionContext(
            string label, Action<Action<bool>> buyAction)
        {
            this.label = label;
            this.buyAction = buyAction;
        }
    }

    public class AvailableActionsPanel : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField]
        protected TextMeshProUGUI _textAvailableCash;
        [SerializeField]
        protected Button  _prefabActionButton;
#pragma warning restore 0649

        public Player player;
        public int maxAllowed = -1;
        public Func<string> getLabelFn;
        private List<Button> _buyActionButtons;
        private int _resolved;

        private void Awake()
        {
            GetComponent<MessageBox>().confirmMessageHandler = GetConfirmMessage;
        }

        private void buyCallback(int index, bool success)
        {
            if (success && index < _buyActionButtons.Count)
            {
                _buyActionButtons[index].gameObject.SetActive(false);
                ++_resolved;
            }

            if (maxAllowed >= 0 &&_resolved >= maxAllowed) 
            {
                GetComponent<MessageBox>().OnButtonOk();
            }
            else if (!_buyActionButtons.Exists(b => b.gameObject.activeInHierarchy))
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
            if (_textAvailableCash != null && getLabelFn != null)
            {
                Localization local = Localization.Instance;
                _textAvailableCash.text = string.Format(
                    "Available Cash: {0}", local.GetCurrency(player.cash));
                _textAvailableCash.text = getLabelFn();
            }
        }

        public virtual void Initialize(
            List<AvailableActionContext> buyActions)
        {
            _resolved = 0;
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
                Action buyAction = () => buyActions[index].buyAction(
                    success => buyCallback(index, success));
                Button button = Instantiate(_prefabActionButton, parentTransform);
                button.GetComponentInChildren<TextMeshProUGUI>().text = buyActions[index].label;
                button.onClick.AddListener(new UnityEngine.Events.UnityAction(buyAction));
                button.transform.SetSiblingIndex(index + 1);
                button.gameObject.SetActive(true);
                _buyActionButtons.Add(button);
            }

            Refresh();
        }

        private string GetConfirmMessage(ButtonType buttonType)
        {
            if (buttonType == ButtonType.CANCEL)
            {
                return _buyActionButtons.Exists(b => b.gameObject.activeInHierarchy) ?
                    "Pass on these opportunities?" : "";
            }
            return "";
        }
    }
}
