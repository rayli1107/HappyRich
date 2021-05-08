﻿using PlayerInfo;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UI.Panels;
using UI.Panels.Assets;
using TMPro;
using Michsky.UI.ModernUIPack;
using UI.Panels.PlayerDetails;
using UI.Panels.Templates;
using UI.Panels.Actions;
using PlayerState;
using System;
using ScriptableObjects;

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
        private SimpleTextPrompt _prefabSimpleTextPrompt;
        [SerializeField]
        private ActionMenuPanel _prefabActionMenuPanel;
        [SerializeField]
        private StockMarketPanel _prefabStockMarketPanel;
        [SerializeField]
        private StockPanel _prefabStockTradePanel;
        [SerializeField]
        private NumberInputPanel _prefabNumberInputPanel;
        [SerializeField]
        private RentalRealEstatePurchasePanel _prefabAdvancedRentalRealEstatePurchasePanel;
        [SerializeField]
        private RentalRealEstatePurchasePanel _prefabSimpleRentalRealEstatePurchasePanel;
        [SerializeField]
        private DistressedRealEstatePurchasePanel _prefabAdvancedDistressedRealEstatePurchasePanel;
        [SerializeField]
        private DistressedRealEstatePurchasePanel _prefabSimpleDistressedRealEstatePurchasePanel;
        [SerializeField]
        private RentalRealEstateRefinancePanel _prefabAdvancedRentalRealEstateRefinancePanel;
        [SerializeField]
        private RentalRealEstateRefinancePanel _prefabSimpleRentalRealEstateRefinancePanel;
        [SerializeField]
        private RealEstateSalePanel _prefabSimpleRealEstateSalePanel;
        [SerializeField]
        private StartupBusinessPurchasePanel _prefabAdvancedStartupBusinessPurchasePanel;
        [SerializeField]
        private StartupBusinessPurchasePanel _prefabSimpleStartupBusinessPurchasePanel;
        [SerializeField]
        private ContactListPanel _prefabContactListPanel;
        [SerializeField]
        private HappinessListPanel _prefabHappinessListPanel;
        [SerializeField]
        private JobListPanel _prefabJobListPanel;
        [SerializeField]
        private TraitsSkillsListPanel _prefabTraitsSkillsListPanel;
        [SerializeField]
        private GameObject _prefabMessageBoxPanel;
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
            MessageBoxHandler handler,
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
            MessageBoxHandler handler)
        {
            SimpleTextMessageBox messageBox = Instantiate(
                _prefabSimpleTextMessageBox, transform);
            messageBox.text.text = message;
            messageBox.messageBoxHandler = handler;
            messageBox.EnableButtons(buttonChoice);
            return messageBox;
        }

        public SimpleTextPrompt ShowSimpleTextPrompt(
            string message,
            TextInputCallback textInputCallback,
            TextInputConfirmMessageHandler confirmMessageHandler,
            bool cancelEnabled,
            bool requireNonEmpty)
        {
            SimpleTextPrompt prompt = Instantiate(
                _prefabSimpleTextPrompt, transform);
            prompt.message = message;
            prompt.textInputCallback = textInputCallback;
            prompt.confirmMessageHandler = confirmMessageHandler;
            prompt.cancelEnabled = cancelEnabled;
            prompt.requireNonEmpty = requireNonEmpty;
            prompt.gameObject.SetActive(true);
            return prompt;
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
            StockMarketPanel panel = Instantiate(_prefabStockMarketPanel, transform);
            panel.player = GameManager.Instance.player;
            panel.Refresh();
        }

        public void ShowStockTradePanel(Assets.AbstractStock stock)
        {
            StockPanel panel = Instantiate(_prefabStockTradePanel, transform);
            panel.player = GameManager.Instance.player;
            panel.stock = stock;
            panel.Refresh();
        }

        public void ShowNumberInputPanel(
            string message,
            int max,
            NumberInputCallback numberInputCallback,
            Action cancelCallback)
        {
            NumberInputPanel panel = Instantiate(_prefabNumberInputPanel, transform);
            panel.message = message;
            panel.max = max;
            panel.numberCallback = numberInputCallback;
            panel.cancelCallback = cancelCallback;
            panel.Refresh();
        }

        public void ShowRentalRealEstatePurchasePanel(
            Assets.AbstractRealEstate asset,
            Assets.PartialInvestment partialAsset,
            MessageBoxHandler messageBoxHandler,
            StartTransactionHandler startTransactionHandler,
            bool advanced)
        {
            RentalRealEstatePurchasePanel panel = Instantiate(
                advanced ?
                _prefabAdvancedRentalRealEstatePurchasePanel :
                _prefabSimpleRentalRealEstatePurchasePanel,
                transform);

            panel.player = GameManager.Instance.player;
            panel.asset = asset;
            panel.partialAsset = partialAsset;

            MessageBox messageBox = panel.GetComponent<MessageBox>();
            messageBox.messageBoxHandler = messageBoxHandler;
            messageBox.startTransactionHandler = startTransactionHandler;

            panel.Refresh();
        }

        public void ShowRentalRealEstateRefinancePanel(
            Assets.RefinancedRealEstate asset,
            Assets.PartialInvestment partialAsset,
            MessageBoxHandler handler,
            bool advanced)
        {
            RentalRealEstateRefinancePanel panel = Instantiate(
                advanced ?
                _prefabAdvancedRentalRealEstateRefinancePanel :
                _prefabSimpleRentalRealEstateRefinancePanel,
                transform);

            panel.player = GameManager.Instance.player;
            panel.asset = asset;
            panel.partialAsset = partialAsset;
            panel.refinancedAsset = asset;
            panel.GetComponent<MessageBox>().messageBoxHandler = handler;
            panel.Refresh();
        }

        public void ShowDistressedRealEstatePurchasePanel(
            Assets.DistressedRealEstate asset,
            Assets.PartialInvestment partialAsset,
            MessageBoxHandler messageBoxHandler,
            StartTransactionHandler startTransactionHandler,
            bool advanced)
        {
            DistressedRealEstatePurchasePanel panel = Instantiate(
                advanced ?
                _prefabAdvancedDistressedRealEstatePurchasePanel :
                _prefabSimpleDistressedRealEstatePurchasePanel,
                transform);

            panel.player = GameManager.Instance.player;
            panel.distressedAsset = asset;
            panel.asset = asset;
            panel.partialAsset = partialAsset;

            MessageBox messageBox = panel.GetComponent<MessageBox>();
            messageBox.messageBoxHandler = messageBoxHandler;
            messageBox.startTransactionHandler = startTransactionHandler;

            panel.Refresh();
        }

        public void ShowRealEstateSalePanel(
            Assets.RentalRealEstate asset,
            Assets.PartialInvestment partialAsset,
            int initialOffer,
            int finalOffer,
            MessageBoxHandler handler,
            bool advanced)
        {
            RealEstateSalePanel panel = Instantiate(
                _prefabSimpleRealEstateSalePanel, transform);
            panel.player = GameManager.Instance.player;
            panel.asset = asset;
            panel.partialAsset = partialAsset;
            panel.initialOffer = initialOffer;
            panel.finalOffer = finalOffer;
            panel.GetComponent<MessageBox>().messageBoxHandler = handler;
            panel.Refresh();
        }

        public void ShowStartupBusinessPurchasePanel(
            Assets.Business asset,
            Assets.PartialInvestment partialAsset,
            MessageBoxHandler messageBoxHandler,
            StartTransactionHandler startTransactionHandler,
            bool advanced)
        {
            StartupBusinessPurchasePanel panel = Instantiate(
                advanced ?
                _prefabAdvancedStartupBusinessPurchasePanel :
                _prefabSimpleStartupBusinessPurchasePanel,
                transform);

            panel.player = GameManager.Instance.player;
            panel.asset = asset;
            panel.partialAsset = partialAsset;

            MessageBox messageBox = panel.GetComponent<MessageBox>();
            messageBox.messageBoxHandler = messageBoxHandler;
            messageBox.startTransactionHandler = startTransactionHandler;

            panel.Refresh();
        }


        public void ShowContactListPanel(
            List<InvestmentPartner> partners,
            ContactSelectCallback callback=null)
        {
            ContactListPanel panel = Instantiate(_prefabContactListPanel, transform);
            panel.player = GameManager.Instance.player;
            panel.callback = callback;
            panel.partners = partners;
            panel.Refresh();
        }

        public void ShowTraitsSkillsListPanel()
        {
            TraitsSkillsListPanel panel = Instantiate(
                _prefabTraitsSkillsListPanel, transform);
            panel.player = GameManager.Instance.player;
            panel.Refresh();
        }

        public void ShowJobListPanel(JobListPanelMode mode)
        {
            Player player = GameManager.Instance.player;
            JobListPanel panel = Instantiate(
                _prefabJobListPanel, UIManager.Instance.transform);
            panel.player = player;
            panel.mode = mode;
            panel.Refresh();
        }

        public void ShowHappinessListPanel()
        {
            HappinessListPanel panel = Instantiate(
                _prefabHappinessListPanel, transform);
            panel.player = GameManager.Instance.player;
            panel.Refresh();
        }

        public void ShowPlayerStateInfo(AbstractPlayerState state, MessageBoxHandler callback)
        {
            Localization local = Localization.Instance;
            string message = string.Join(
                "\n", local.GetPlayerState(state), state.description);
            ShowSimpleMessageBox(message, ButtonChoiceType.OK_ONLY, callback);
        }

        public void ShowSkillInfo(SkillInfo skillInfo, MessageBoxHandler callback)
        {
            Localization local = Localization.Instance;
            string message = string.Join(
                "\n", local.GetSkill(skillInfo), skillInfo.skillDescription);
            ShowSimpleMessageBox(message, ButtonChoiceType.OK_ONLY, callback);
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
