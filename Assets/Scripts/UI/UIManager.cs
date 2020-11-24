using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public class UIManager : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField]
        private GameObject _prefabMessageBoxPanel;
        [SerializeField]
        private GameObject _prefabTopLevelUIPanel;
        [SerializeField]
        private GameObject _prefabPlayerStatusMenuPanel;
        [SerializeField]
        private GameObject _prefabScrollableTextPanel;
#pragma warning restore 0649

        public static UIManager Instance { get; private set; }
        public bool ready { get; private set; }

        private List<Panels.ModalObject> modalObjects;
        private EventSystem _eventSystem;
        private Panels.PlayerSnapshotPanel _playerSnapshotPanel;

        private void Awake()
        {
            ready = false;
            Instance = this;
            _eventSystem = EventSystem.current;
        }

        void Start()
        {
            modalObjects = new List<Panels.ModalObject>();
            Instantiate(_prefabTopLevelUIPanel, transform);
            _playerSnapshotPanel = GetComponentInChildren<Panels.PlayerSnapshotPanel>(true);
            ready = true;
        }

        public void RegisterModalItem(Panels.ModalObject modalObject)
        {
            if (modalObjects.Count > 0)
            {
                modalObjects[modalObjects.Count - 1].EnableInput(false);
            }
            modalObjects.Add(modalObject);
        }

        public void UnregisterModalItem(Panels.ModalObject modalObject)
        {
            modalObjects.Remove(modalObject);
            if (modalObjects.Count > 0)
            {
                modalObjects[modalObjects.Count - 1].EnableInput(true);
            }
        }

        void Update()
        {
            int count = modalObjects.Count;
            if (count > 0)
            {
                modalObjects[count - 1].ActivePanelUpdate();
            }

            if (Input.GetMouseButtonDown(0) && modalObjects.Count > 0)
            {
                Panels.ModalObject modalObject = modalObjects[modalObjects.Count - 1];
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

        private void ShowMessageBox(
            GameObject childPanel,
            Panels.IMessageBoxHandler handler,
            UI.Panels.ButtonChoiceType buttonChoice)
        {
            GameObject msgBoxObj = Instantiate(_prefabMessageBoxPanel, transform);
            childPanel.transform.SetParent(msgBoxObj.transform);
            childPanel.transform.localScale = Vector3.one;
            RectTransform childRect = childPanel.GetComponent<RectTransform>();
            childRect.anchorMin = childRect.anchorMax = childRect.pivot = new Vector2(0, 1);
            childRect.anchoredPosition = new Vector2(0, 0);
            childPanel.SetActive(true);

            Panels.MessageBox msgBox = msgBoxObj.GetComponent<Panels.MessageBox>();
            msgBox.childRect = childPanel.GetComponent<RectTransform>();
            msgBox.buttonChoice = buttonChoice;
            msgBox.messageBoxHandler = handler;
            msgBoxObj.GetComponent<Image>().color = Color.blue;
            msgBoxObj.SetActive(true);
        }

        public void ShowPlayerStatusMenuPanel()
        {
            GameObject gameObj = Instantiate(_prefabPlayerStatusMenuPanel);
            Panels.PlayerStatusMenuPanel panel = gameObj.GetComponent<Panels.PlayerStatusMenuPanel>();
            panel.player = GameManager.Instance.player;
            ShowMessageBox(gameObj, panel, Panels.ButtonChoiceType.OK_CANCEL);
        }

        public void ShowAssetLiabilityStatusPanel()
        {
            GameObject gameObj = Instantiate(_prefabScrollableTextPanel);
            Panels.AssetLiabilityListPanel panel = gameObj.AddComponent<Panels.AssetLiabilityListPanel>();
            panel.player = GameManager.Instance.player;
            ShowMessageBox(gameObj, panel, Panels.ButtonChoiceType.NONE);
        }

        public void ShowIncomeExpenseStatusPanel()
        {
            GameObject gameObj = Instantiate(_prefabScrollableTextPanel);
            Panels.IncomeExpenseListPanel panel = gameObj.AddComponent<Panels.IncomeExpenseListPanel>();
            panel.player = GameManager.Instance.player;
            ShowMessageBox(gameObj, panel, Panels.ButtonChoiceType.NONE);
        }
    }
}
