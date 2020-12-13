using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UI.Panels;
using UI.Panels.Assets;
using TMPro;
using Michsky.UI.ModernUIPack;
using UI.Panels.PlayerDetails;
using UI.Panels.Templates;
using UI.Panels.Actions;

namespace UI
{
    public class UIManager : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField]
        private GameObject _actionButton;
        [SerializeField]
        private ModalWindowManager _prefabModalWindowManager;
        [SerializeField]
        private PlayerStatusMenuPanel _prefabPlayerStatusManeuPanel;
        [SerializeField]
        private AssetLiabilityListPanel _prefabAssetLiabilityListPanel;
        [SerializeField]
        private IncomeExpenseListPanel _prefabIncomeExpenseListPanel;
        [SerializeField]
        private SimpleTextMessageBox _prefabSimpleTextMessageBox;
        [SerializeField]
        private ActionMenuPanel _prefabActionMenuPanel;

        [SerializeField]
        private GameObject _prefabMessageBoxPanel;
        [SerializeField]
        private GameObject _prefabScrollableTextPanel;
        [SerializeField]
        private GameObject _prefabSimpleTextPanel;
        [SerializeField]
        private GameObject _prefabStockMarketPanel;
        [SerializeField]
        private GameObject _prefabStockTradePanel;
        [SerializeField]
        private GameObject _prefabNumberInputPanel;

#pragma warning restore 0649

        public static UIManager Instance { get; private set; }
        public bool ready { get; private set; }

        private List<ModalObject> _modalObjects;
        private EventSystem _eventSystem;
        private PlayerSnapshotPanel _playerSnapshotPanel;

        private void Awake()
        {
            ready = false;
            Instance = this;
            _eventSystem = EventSystem.current;
            _modalObjects = new List<ModalObject>();
            foreach (ModalObject panel in GetComponentsInChildren<ModalObject>(true))
            {
                panel.gameObject.SetActive(true);
            }
        }

        void Start()
        {
            _playerSnapshotPanel = GetComponentInChildren<PlayerSnapshotPanel>(true);
            ready = true;
        }

        public void RegisterModalItem(ModalObject modalObject)
        {
            if (_modalObjects.Count > 0)
            {
                _modalObjects[_modalObjects.Count - 1].EnableInput(false);
            }
            _modalObjects.Add(modalObject);
        }

        public void UnregisterModalItem(ModalObject modalObject)
        {
            _modalObjects.Remove(modalObject);
            if (_modalObjects.Count > 0)
            {
                _modalObjects[_modalObjects.Count - 1].EnableInput(true);
            }
        }

        void Update()
        {
            /*
                        int count = _modalObjects.Count;
                        if (count > 0)
                        {
                            _modalObjects[count - 1].ActivePanelUpdate();
                        }
                        if (Input.GetMouseButtonDown(0) && _modalObjects.Count > 0)
                        {
                            ModalObject modalObject = _modalObjects[_modalObjects.Count - 1];
                            if (!DetectHit(Input.mousePosition, modalObject.gameObject))
                            {
                                modalObject.OnClickOutsideBoundary();
                            }
                        }
                        */
        }

        private bool DetectHit(Vector2 position, GameObject obj)
        {
            PointerEventData data = new PointerEventData(_eventSystem);
            data.position = Input.mousePosition;

            List<RaycastResult> results = new List<RaycastResult>();
            _eventSystem.RaycastAll(data, results);
            foreach (RaycastResult result in results)
            {
                if (result.gameObject == obj)
                {
                    return true;
                }
            }
            return false;
        }

        public void EnableActionButton(bool enable)
        {
            _actionButton.GetComponent<Button>().interactable = enable;
        }

        public void UpdatePlayerInfo(Player player)
        {
            _playerSnapshotPanel.UpdatePlayerInfo(player);
        }

        public MessageBox ShowMessageBox(
            GameObject childPanel,
            IMessageBoxHandler handler,
            ButtonChoiceType buttonChoice)
        {
            GameObject msgBoxObj = Instantiate(_prefabMessageBoxPanel, transform);
            childPanel.transform.SetParent(msgBoxObj.transform);
            childPanel.transform.localScale = Vector3.one;
            childPanel.SetActive(true);

            MessageBox msgBox = msgBoxObj.GetComponent<MessageBox>();
//            msgBox.childRect = childPanel.GetComponent<RectTransform>();
//            msgBox.buttonChoice = buttonChoice;
            msgBox.messageBoxHandler = handler;
            msgBoxObj.GetComponent<Image>().color = Color.blue;
            msgBoxObj.SetActive(true);
            return msgBox;
        }

        public SimpleTextMessageBox ShowSimpleMessageBox(
            string message,
            ButtonChoiceType buttonChoice,
            IMessageBoxHandler handler)
        {
            SimpleTextMessageBox messageBox = Instantiate(
                _prefabSimpleTextMessageBox, transform);
            messageBox.text.text = message;
            messageBox.messageBoxHandler = handler;
            messageBox.EnableButtons(buttonChoice);
            return messageBox;
            /*
            GameObject gameObj = Instantiate(_prefabSimpleTextPanel);
            TextMeshProUGUI text = gameObj.GetComponent<TextMeshProUGUI>();
            text.text = message;
            return ShowMessageBox(gameObj, handler, buttonChoice);*/ 
        }

        public void ShowPlayerStatusMenuPanel()
        {
            PlayerStatusMenuPanel panel = Instantiate(_prefabPlayerStatusManeuPanel, transform);
            panel.player = GameManager.Instance.player;
            panel.gameObject.SetActive(true);
        }


        public void ShowActionMenuPanel()
        {
            ActionMenuPanel panel = Instantiate(_prefabActionMenuPanel, transform);
            panel.player = GameManager.Instance.player;
            /*
            GameObject gameObj = Instantiate(_prefabActionMenuPanel);
            ActionMenuPanel panel = gameObj.GetComponent<ActionMenuPanel>();
            panel.player = GameManager.Instance.player;
            ShowMessageBox(gameObj, null, ButtonChoiceType.BACK_ONLY);
            */
        }

        public void ShowAssetLiabilityStatusPanel()
        {
            AssetLiabilityListPanel panel = Instantiate(_prefabAssetLiabilityListPanel, transform);
            panel.player = GameManager.Instance.player;
            panel.RefreshContent();
        }

        public void ShowIncomeExpenseStatusPanel()
        {
            IncomeExpenseListPanel panel = Instantiate(_prefabIncomeExpenseListPanel, transform);
            panel.player = GameManager.Instance.player;
            panel.RefreshContent();
        }

        public void ShowStockMarketPanel()
        {
            GameObject gameObject = Instantiate(_prefabStockMarketPanel);
            StockMarketPanel panel = gameObject.GetComponent<StockMarketPanel>();
            panel.player = GameManager.Instance.player;
            ShowMessageBox(gameObject, null, ButtonChoiceType.OK_ONLY);
        }

        public void ShowStockTradePanel(Assets.AbstractStock stock)
        {
            GameObject gameObject = Instantiate(_prefabStockTradePanel);
            StockPanel panel = gameObject.GetComponent<StockPanel>();
            panel.player = GameManager.Instance.player;
            panel.stock = stock;
            ShowMessageBox(gameObject, null, ButtonChoiceType.NONE);
        }

        public void ShowNumberInputPanel(string message, int max, INumberInputCallback callback)
        {
            GameObject gameObject = Instantiate(_prefabNumberInputPanel);
            NumberInputPanel panel = gameObject.GetComponent<NumberInputPanel>();
            panel.message = message;
            panel.max = max;
            panel.callback = callback;
            ShowMessageBox(gameObject, panel, ButtonChoiceType.NONE);
        }

        public void DestroyAllModal()
        {
            while (_modalObjects.Count > 1)
            {
                _modalObjects[_modalObjects.Count - 1].Destroy();
            }
        }

        public void OnEndTurnButton()
        {
            GameManager.Instance.StateMachine.OnPlayerTurnDone();
        }
    }
}
