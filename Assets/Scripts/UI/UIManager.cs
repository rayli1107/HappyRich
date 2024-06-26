﻿using InvestmentPartnerInfo;
using Michsky.UI.ModernUIPack;
using PlayerInfo;
using PlayerState;
using ScriptableObjects;
using System;
using System.Collections.Generic;
using UI.Panels;
using UI.Panels.Actions;
using UI.Panels.Assets;
using UI.Panels.PlayerDetails;
using UI.Panels.Templates;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public class UIManager : MonoBehaviour
    {
#pragma warning disable 0649
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
        private StockMarketPanel _prefabStockMarketPanel;
        [SerializeField]
        private SimpleNumberInputPanel _prefabNumberInputPanel;
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
        private SmallBusinessPurchasePanel _prefabAdvancedSmallBusinessPurchasePanel;
        [SerializeField]
        private SmallBusinessPurchasePanel _prefabSimpleSmallBusinessPurchasePanel;
        [SerializeField]
        private FranchiseJoinPanel _prefabAdvancedFranchiseJoinPanel;
        [SerializeField]
        private FranchiseJoinPanel _prefabSimpleFranchiseJoinPanel;
        [SerializeField]
        private StartupPurchasePanel _prefabAdvancedStartupPurchasePanel;
        [SerializeField]
        private StartupPurchasePanel _prefabSimpleStartupPurchasePanel;
        [SerializeField]
        private ContactListPanel _prefabContactListPanel;
        [SerializeField]
        private AvailableActionsPanel _prefabAvailableActionsPanel;
        [SerializeField]
        private HappinessListPanel _prefabHappinessListPanel;
        [SerializeField]
        private JobListPanel _prefabJobListPanel;
        [SerializeField]
        private TraitsSkillsListPanel _prefabTraitsSkillsListPanel;
        [SerializeField]
        private CharacterSelectionPanel _prefabCharacterSelectionPanel;
        [SerializeField]
        private TimedModalObject _prefabTimedTransitionScreen;
        [SerializeField]
        private RaisePersonalFundsMessageBox _prefabRaisePersonalFundsMessageBox;
        [SerializeField]
        private EventLogPanel _prefabEventLogPanel;
        [SerializeField]
        private GameObject _prefabMessageBoxPanel;
        [SerializeField]
        private GrowthStockComponent _prefabGrowthStockPanel;
        [SerializeField]
        private YieldStockComponent _prefabYieldStockComponent;
        [SerializeField]
        private CryptoComponent _prefabCryptoComponent;
        [SerializeField]
        private TutorialFocusPanel _prefabTutorialFocusPanel;
        [SerializeField]
        private Button _buttonEndTurnNotReady;
        [SerializeField]
        private Button _buttonEndTurnReady;
#pragma warning restore 0649

        public static UIManager Instance { get; private set; }
        public bool ready { get; private set; }

        public ModalObject topModalObject =>
            _modalObjects.Count == 0 ? null : _modalObjects[_modalObjects.Count - 1];

        private List<ModalObject> _modalObjects;
        private EventSystem _eventSystem;
        private PlayerSnapshotPanel _playerSnapshotPanel;
        private ActionMenuPanel _actionMenuPanel;

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
            _actionMenuPanel = GetComponentInChildren<ActionMenuPanel>(true);
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
            _actionMenuPanel.SetEnable(enable);
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
            MessageBoxHandler handler,
            Action detailHandler = null,
            Action helpHandler = null)
        {
            SimpleTextMessageBox messageBox = Instantiate(
                _prefabSimpleTextMessageBox, transform);
            messageBox.text.text = message;
            messageBox.messageBoxHandler = handler;
            messageBox.EnableButtons(buttonChoice);
            messageBox.buttonDetailHandler = detailHandler;
            messageBox.buttonHelpHandler = helpHandler;
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
            panel.Refresh();
        }

        public void ShowAssetLiabilityStatusPanel()
        {
            AssetLiabilityListPanel panel = Instantiate(_prefabAssetLiabilityListPanel, transform);
            panel.player = GameManager.Instance.player;
            panel.RefreshContent();
        }

        public void ShowIncomeExpenseStatusPanel()
        {
            ShowIncomeExpenseStatusPanel(null);
        }


        public void ShowIncomeExpenseStatusPanel(Snapshot snapshot)
        {
            IncomeExpenseListPanel panel = Instantiate(_prefabIncomeExpenseListPanel, transform);
            panel.player = GameManager.Instance.player;
            panel.incomeExpenseSnapshot = snapshot;
            panel.RefreshContent();
        }

        public void ShowStockMarketPanel()
        {
            ShowStockMarketPanel(null);
        }

        public void ShowStockMarketPanel(MessageBoxHandler handler)
        {
            StockMarketPanel panel = Instantiate(_prefabStockMarketPanel, transform);
            panel.player = GameManager.Instance.player;
            panel.GetComponent<MessageBox>().messageBoxHandler = handler;
            panel.Refresh();
        }

        public void ShowGrowthStockPanel(
            Assets.GrowthStock stock,
            Action callback=null)
        {
            Player player = GameManager.Instance.player;
            GrowthStockComponent component = Instantiate(_prefabGrowthStockPanel, transform);
            component.player = player;
            component.growthStock = stock;
            component.GetComponent<MessageBox>().messageBoxHandler = _ => callback?.Invoke();
            component.Refresh();
        }

        public void ShowYieldStockPanel(
            Assets.YieldStock stock,
            Action callback=null)
        {
            Player player = GameManager.Instance.player;
            YieldStockComponent component = Instantiate(_prefabYieldStockComponent, transform);
            component.player = player;
            component.yieldStock = stock;
            component.GetComponent<MessageBox>().messageBoxHandler = _ => callback?.Invoke();
            component.Refresh();
        }

        public void ShowCryptoPanel(
            Assets.AbstractCryptoCurrency stock,
            Action callback=null)
        {
            Player player = GameManager.Instance.player;
            CryptoComponent component = Instantiate(_prefabCryptoComponent, transform);
            component.player = player;
            component.crypto = stock;
            component.GetComponent<MessageBox>().messageBoxHandler = _ => callback?.Invoke();
            component.Refresh();
        }

        public SimpleNumberInputPanel ShowNumberInputPanel(
            string message,
            int maxValue,
            Action<ButtonType, int> numberInputCallback,
            Func<ButtonType, int, string> confirmMessageHandler,
            Action<TransactionHandler, int> startTransactionHandler,
            ButtonChoiceType buttonChoice = ButtonChoiceType.OK_CANCEL)
        {
            SimpleNumberInputPanel panel = Instantiate(_prefabNumberInputPanel, transform);
            panel.message = message;
            panel.maxValue = maxValue;
            panel.numberInputCallback = numberInputCallback;
            panel.confirmMessageHandler = confirmMessageHandler;
            panel.startTransactionHandler = startTransactionHandler;

            MessageBox messageBox = panel.GetComponent<MessageBox>();
            messageBox.EnableButtons(buttonChoice);

            panel.Refresh();
            return panel;
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

        public void ShowSmallBusinessPurchasePanel(
            Assets.SmallBusiness asset,
            Assets.PartialInvestment partialAsset,
            MessageBoxHandler messageBoxHandler,
            StartTransactionHandler startTransactionHandler,
            bool advanced)
        {
            SmallBusinessPurchasePanel panel = Instantiate(
                advanced ?
                _prefabAdvancedSmallBusinessPurchasePanel :
                _prefabSimpleSmallBusinessPurchasePanel,
                transform);

            panel.player = GameManager.Instance.player;
            panel.asset = asset;
            panel.business = asset;
            panel.partialAsset = partialAsset;

            MessageBox messageBox = panel.GetComponent<MessageBox>();
            messageBox.messageBoxHandler = messageBoxHandler;
            messageBox.startTransactionHandler = startTransactionHandler;

            panel.Refresh();
        }

        public void ShowStartupPurchasePanel(
            Assets.Startup asset,
            Assets.PartialInvestment partialAsset,
            MessageBoxHandler messageBoxHandler,
            StartTransactionHandler startTransactionHandler,
            bool advanced)
        {
            StartupPurchasePanel panel = Instantiate(
                advanced ?
                _prefabAdvancedStartupPurchasePanel :
                _prefabSimpleStartupPurchasePanel,
                transform);

            panel.player = GameManager.Instance.player;
            panel.asset = asset;
            panel.partialAsset = partialAsset;

            MessageBox messageBox = panel.GetComponent<MessageBox>();
            messageBox.messageBoxHandler = messageBoxHandler;
            messageBox.startTransactionHandler = startTransactionHandler;

            panel.Refresh();
        }

        public void ShowFranchiseJoinPanel(
            Assets.Franchise asset,
            Assets.PartialInvestment partialAsset,
            MessageBoxHandler messageBoxHandler,
            StartTransactionHandler startTransactionHandler,
            bool advanced)
        {
            FranchiseJoinPanel panel = Instantiate(
                advanced ?
                _prefabAdvancedFranchiseJoinPanel :
                _prefabSimpleFranchiseJoinPanel,
                transform);

            panel.player = GameManager.Instance.player;
            panel.franchise = asset;
            panel.asset = asset;
            panel.partialAsset = partialAsset;

            MessageBox messageBox = panel.GetComponent<MessageBox>();
            messageBox.messageBoxHandler = messageBoxHandler;
            messageBox.startTransactionHandler = startTransactionHandler;

            panel.Refresh();
        }

        public void ShowTimedTransitionScreen(
            string message,
            Color color,
            Action callback)
        {
            TimedModalObject screen = Instantiate(
                _prefabTimedTransitionScreen, transform);
            screen.text = message;
            screen.color = color;
            screen.callback = callback;
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

        public void ShowEventLogPanel(MessageBoxHandler messageBoxHandler = null)
        {
            EventLogPanel panel = Instantiate(_prefabEventLogPanel, transform);

            MessageBox messageBox = panel.GetComponent<MessageBox>();
            messageBox.messageBoxHandler = messageBoxHandler;

            panel.gameObject.SetActive(true);
            panel.Refresh();
        }

        public void ShowAvailableActionsPanel(
            List<AvailableActionContext> buyActions,
            Func<string> getLabelFn,
            MessageBoxHandler messageBoxHandler,
            int maxAllowed = -1)
        {
            AvailableActionsPanel panel = Instantiate(
                _prefabAvailableActionsPanel, transform);
            panel.player = GameManager.Instance.player;
            panel.getLabelFn = getLabelFn;
            panel.maxAllowed = maxAllowed;
            panel.Initialize(buyActions);

            MessageBox messageBox = panel.GetComponent<MessageBox>();
            messageBox.messageBoxHandler = messageBoxHandler;
        }

        public void ShowCharacterSelectionPanel(
            List<Profession> professions,
            Action<Profession> callback)
        {
            CharacterSelectionPanel panel = Instantiate(_prefabCharacterSelectionPanel, transform);
            panel.professions = professions;
            panel.professionIndex = 0;
            panel.selectionCallback = callback;
            panel.gameObject.SetActive(true);
            panel.Refresh();
        }

        public void ShowRaisePersonalFundsMessageBox(
            Player player,
            int amount,
            Action<bool> callback)
        {
            RaisePersonalFundsMessageBox messageBox = Instantiate(
                _prefabRaisePersonalFundsMessageBox, transform);
            messageBox.player = player;
            messageBox.amount = amount;
            messageBox.callback = callback;
            messageBox.Refresh();
        }

        public void DestroyAllModal()
        {
            while (_modalObjects.Count > 1)
            {
                _modalObjects[_modalObjects.Count - 1].Destroy();
            }
        }

        public void ShowEndTurnButton(bool ready)
        {
            _buttonEndTurnReady.gameObject.SetActive(ready);
            _buttonEndTurnNotReady.gameObject.SetActive(!ready);
        }

        private void onEndTurnConfirm(ButtonType buttonType)
        {
            if (buttonType == ButtonType.OK)
            {
                GameManager.Instance.StateMachine.OnPlayerTurnDone();
            }
        }

        public void OnEndTurnButton(bool ready)
        {
            if (ready)
            {
                GameManager.Instance.StateMachine.OnPlayerTurnDone();
            }
            else
            {
                string message = "You haven't done anything this year yet. Are you sure you want to end your turn?";
                ShowSimpleMessageBox(message, ButtonChoiceType.OK_CANCEL, onEndTurnConfirm);
            }
        }

        public TutorialFocusPanel ShowTutorialFocusPanel()
        {
            TutorialFocusPanel panel = Instantiate(_prefabTutorialFocusPanel, transform);
            panel.gameObject.SetActive(true);
            return panel;
        }
    }
}
