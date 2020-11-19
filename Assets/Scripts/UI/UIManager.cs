using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace UI
{
    public class UIManager : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField]
        private TextMeshProUGUI _textAge;
        [SerializeField]
        private TextMeshProUGUI _textHappiness;
        [SerializeField]
        private TextMeshProUGUI _textFI;
        [SerializeField]
        private TextMeshProUGUI _textCash;
        [SerializeField]
        private TextMeshProUGUI _textCashflow;
        [SerializeField]
        private TextMeshProUGUI _textNetworth;
#pragma warning restore 0649

        public static UIManager Instance { get; private set; }

        private List<ModalObject> modalObjects;
        private EventSystem _eventSystem;

        private void Awake()
        {
            Instance = this;
            _eventSystem = EventSystem.current;
        }

        void Start()
        {
            modalObjects = new List<ModalObject>();
        }

        public void RegisterModalItem(ModalObject modalObject)
        {
            if (modalObjects.Count > 0)
            {
                modalObjects[modalObjects.Count - 1].EnableInput(false);
            }
            modalObjects.Add(modalObject);
        }

        public void UnregisterModalItem(ModalObject modalObject)
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
                ModalObject modalObject = modalObjects[modalObjects.Count - 1];
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
            PlayerSnapshot snapshot = new PlayerSnapshot(player);

            if (_textAge)
            {
                _textAge.text = string.Format("Age: {0}", snapshot.age);
            }

            if (_textHappiness)
            {
                _textHappiness.text = string.Format("Happy {0}", snapshot.happiness);
            }

            if (_textFI)
            {
                int fi = (100 * snapshot.passiveIncome) / snapshot.expenses;
                _textFI.text = string.Format("FI: {0}%", fi);
            }

            if (_textCash)
            {
                _textCash.text = string.Format("Cash:\n{0}", snapshot.cash);
            }

            if (_textCashflow)
            {
                int cashflow = snapshot.activeIncome + snapshot.passiveIncome - snapshot.expenses;
                _textCashflow.text = string.Format("Cashflow:\n{0}", cashflow);
            }

            if (_textNetworth)
            {
                _textNetworth.text = string.Format("Net Worth:\n{0}", snapshot.netWorth);
            }
        }
    }
}
