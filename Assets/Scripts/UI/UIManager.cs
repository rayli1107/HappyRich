using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UI.Panels;
using TMPro;

namespace UI
{
    public class UIManager : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField]
        private GameObject _prefabMessageBoxPanel;
        [SerializeField]
        private GameObject _prefabPlayerStatusMenuPanel;
        [SerializeField]
        private GameObject _prefabActionMenuPanel;
        [SerializeField]
        private GameObject _prefabScrollableTextPanel;
        [SerializeField]
        private GameObject _prefabSimpleTextPanel;
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
            msgBox.childRect = childPanel.GetComponent<RectTransform>();
            msgBox.buttonChoice = buttonChoice;
            msgBox.messageBoxHandler = handler;
            msgBoxObj.GetComponent<Image>().color = Color.blue;
            msgBoxObj.SetActive(true);
            return msgBox;
        }

        public MessageBox ShowSimpleMessageBox(
            string message,
            int size,
            ButtonChoiceType buttonChoice,
            IMessageBoxHandler handler)
        {
            GameObject gameObj = Instantiate(_prefabSimpleTextPanel);
            TextMeshProUGUI text = gameObj.GetComponent<TextMeshProUGUI>();
            text.text = message;
            text.fontSize = size;
            return ShowMessageBox(gameObj, handler, buttonChoice);
        }

        public void ShowPlayerStatusMenuPanel()
        {
            GameObject gameObj = Instantiate(_prefabPlayerStatusMenuPanel);
            PlayerStatusMenuPanel panel = gameObj.GetComponent<PlayerStatusMenuPanel>();
            panel.player = GameManager.Instance.player;
            ShowMessageBox(gameObj, null, ButtonChoiceType.BACK_ONLY);
        }

        public void ShowActionMenuPanel()
        {
            GameObject gameObj = Instantiate(_prefabActionMenuPanel);
            ActionMenuPanel panel = gameObj.GetComponent<ActionMenuPanel>();
            panel.player = GameManager.Instance.player;
            ShowMessageBox(gameObj, null, ButtonChoiceType.BACK_ONLY);
        }

        public void ShowAssetLiabilityStatusPanel()
        {
            GameObject gameObj = Instantiate(_prefabScrollableTextPanel);
            AssetLiabilityListPanel panel = gameObj.AddComponent<AssetLiabilityListPanel>();
            panel.player = GameManager.Instance.player;
            ShowMessageBox(gameObj, null, ButtonChoiceType.NONE);
        }

        public void ShowIncomeExpenseStatusPanel()
        {
            GameObject gameObj = Instantiate(_prefabScrollableTextPanel);
            IncomeExpenseListPanel panel = gameObj.AddComponent<IncomeExpenseListPanel>();
            panel.player = GameManager.Instance.player;
            ShowMessageBox(gameObj, null, ButtonChoiceType.NONE);
        }

        public void DestroyAllModal()
        {
            while (_modalObjects.Count > 1)
            {
                _modalObjects[_modalObjects.Count - 1].OnClickOutsideBoundary();
            }
        }
    }
}
