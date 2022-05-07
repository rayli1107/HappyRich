using Actions;
using StateMachine;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UI;
using UI.Panels;
using UI.Panels.PlayerDetails;
using UI.Panels.Templates;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TutorialAction
{
    private RunOnceAction _once;
    private Action<bool, Action> _tutorialAction;

    public TutorialAction(Action<bool, Action> action)
    {
        _tutorialAction = action;
        _once = new RunOnceAction(cb => action?.Invoke(false, cb));
    }

    public void Run(Action cb)
    {
        _once.Run(cb);
    }

    public void ForceRun(Action cb)
    {
        _tutorialAction?.Invoke(true, cb);
    }
}

public class TutorialManager : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField]
    private TMP_SpriteAsset _spriteAsset;
    [SerializeField]
    private int _tutorialFontSize = 40;
#pragma warning restore 0649

    public static TutorialManager Instance { get; private set; }
    public bool tutorialEnabled { get; private set; }

    public TutorialAction GameInitOnce { get; private set; }
    //public TutorialAction GameStartOnce { get; private set; }
    public TutorialAction InvestmentOnce { get; private set; }
    public TutorialAction JobSearchOnce { get; private set; }
    public TutorialAction SelfImprovementOnce { get; private set; }
    public TutorialAction NetworkingOnce { get; private set; }

    private void Awake()
    {
        Instance = this;
        tutorialEnabled = false;

        GameInitOnce = new TutorialAction(GetGameInitMessageAction);
        //GameStartOnce = new TutorialAction(GetGameStartMessageAction);
        InvestmentOnce = new TutorialAction(GetInvestmentAction);
        JobSearchOnce = new TutorialAction(GetJobSearchAction);
        SelfImprovementOnce = new TutorialAction(GetSelfImprovementAction);
        NetworkingOnce = new TutorialAction(GetNetworkingAction);
    }


    private void enableTutorialMessageHandler(ButtonType button, Action callback)
    {
        tutorialEnabled = button == ButtonType.OK;
        callback?.Invoke();
    }

    public Action<Action> GetEnableTutorialAction()
    {
        return cb => UI.UIManager.Instance.ShowSimpleMessageBox(
            "Enable tutorial?",
            ButtonChoiceType.OK_CANCEL,
            b => enableTutorialMessageHandler(b, cb));
    }

    private void showTutorialMessage(bool force, List<string> messages, Action callback)
    {
        bool enable = force || tutorialEnabled;
        if (enable && messages != null)
        {
            MessageAction.GetAction(messages, _tutorialFontSize, _spriteAsset)?.Invoke(callback);
        }
        else
        {
            callback?.Invoke();
        }
    }

    public void GetGameStartMessageAction(bool force, Action cb)
    {
        Localization local = Localization.Instance;
        List<string> messages = new List<string>()
        {
            "The <sprite name=\"Cash\"> icon represents your <b>current available cash</b>, " +
            " and the <sprite name=\"Cashflow\"> icon represents your <b>annual cahsflow</b>.",

            "The <sprite name=\"Happiness\"> icon represents your <b>total happiness</b>, " +
            "and the <sprite name=\"Fire\"> icon represents your progress towards " +
            "<b>financial indepence</b>.",

            "<b>Financial indepence</b> is achieved once your " +
            "<b>passive income</b> exceeds your <b>total expenses</b>.",

            "There are four types of actions, and you can perform one action every year. " +
            "Click on each action to learn more them.",

            "Good luck and have fun!"
        };
        showTutorialMessage(force, messages, cb);
    }

    public void GetGameInitMessageAction(bool force, Action cb)
    {
        Localization local = Localization.Instance;
        string retireAge = GameManager.Instance.retirementAge.ToString();

        List<string> messages = new List<string>()
        {
            string.Format(
                "Welcome to the {0}!",
                local.colorWrap("Game of Prosperity", new Color(1f, 0.6f, 0f))),
            "The goal of the game is to reach <b>100% Happiness</b> and " +
            "100% <b>Financial Independence</b> before the retirement age, " +
            "which by default is <b>60</b>.",
            "First, please choose your starting profession."
        };

        showTutorialMessage(force, messages, cb);
    }



    public void GetJobSearchAction(bool force, Action cb)
    {
        Localization local = Localization.Instance;
        List<string> messages = new List<string>()
        {
            "<b>Job Search</b> actions allow you to look for additional job to raise your " +
            "<b>active income</b>. You can have up to one <b>full-time</b> and one " +
            "<b>part-time job</b> at the same time.",
        };
        showTutorialMessage(force, messages, cb);
    }

    public void GetSelfImprovementAction(bool force, Action cb)
    {
        Localization local = Localization.Instance;
        List<string> messages = new List<string>()
        {
            "<b>Self improvement</b> actions help you become a better person, " +
            "both in a professional and personal sense.",
        };
        showTutorialMessage(force, messages, cb);
    }

    public void GetNetworkingAction(bool force, Action cb)
    {
        Localization local = Localization.Instance;
        List<string> messages = new List<string>()
        {
            "<b>Networking</b> actions allow you to meet more people who can help you in " +
            "different ways.",
        };
        showTutorialMessage(force, messages, cb);
    }

    public void GetInvestmentAction(bool force, Action cb)
    {
        Localization local = Localization.Instance;
        List<string> messages = new List<string>()
        {
            "<b>Investment</b> actions allow you to invest in different assets that can help " +
            "generate passive income.",
        };

        showTutorialMessage(force, messages, cb);
    }

    private IEnumerator waitForPlayerTurn()
    {
        while (true) {
            IState currentState = GameManager.Instance.StateMachine.currentState;
            if (currentState is PlayerActionState &&
                ((PlayerActionState)currentState).playerStartReady)
            {
                break;
            }
            yield return null;
        }
    }

    private IEnumerator waitForTopPanel<PanelType>()
    {
        while (UIManager.Instance.topModalObject.GetComponent<PanelType>() == null)
        {
            yield return null;
        }
    }

    private IEnumerator waitForButtonClick(ModalObject panel, Button button, string text)
    {
        TutorialFocusPanel focusPanel = UIManager.Instance.ShowTutorialFocusPanel();
        focusPanel.focusPosition = button.transform.position;
        focusPanel.text.text = text;
        focusPanel.text.spriteAsset = _spriteAsset;

        List<Button> buttonsDisabled = panel.DisableButtons(button);
        bool clicked = false;
        UnityAction action = new UnityAction(() => clicked = true);
        button.onClick.AddListener(action);
        while (!clicked)
        {
            yield return null;
        }
        button.onClick.RemoveListener(action);
        panel.EnableButtons(buttonsDisabled);
        Destroy(focusPanel.gameObject);
    }


    private IEnumerator tutorialScript()
    {
        PlayerSnapshotPanel snapshotPanel = null;
        yield return StartCoroutine(waitForPlayerTurn());

        // Assets & Liability List
        yield return waitForTopPanel<PlayerSnapshotPanel>();
        snapshotPanel = UIManager.Instance.topModalObject.GetComponent<PlayerSnapshotPanel>();
        yield return StartCoroutine(
            waitForButtonClick(
                snapshotPanel,
                snapshotPanel.buttonAssets,
                "The <sprite name=\"Cash\"> icon represents your current <b>Available Cash</b>. " +
                "You can click on the button to view your current <b>Assets and Liabilities</b>."));
        yield return waitForTopPanel<AssetLiabilityListPanel>();

        // Income Expense List
        yield return waitForTopPanel<PlayerSnapshotPanel>();
        snapshotPanel = UIManager.Instance.topModalObject.GetComponent<PlayerSnapshotPanel>();
        yield return StartCoroutine(
            waitForButtonClick(
                snapshotPanel,
                snapshotPanel.buttonIncome,
                "The <sprite name=\"Cashflow\"> icon represents your <b>Annual Cashflow</b>. " +
                "You can click on the button to view your current <b>Income and Expenses</b>."));
        yield return waitForTopPanel<IncomeExpenseListPanel>();

    }

    public void StartTutorialScript(Action cb)
    {
        if (tutorialEnabled)
        {
            StartCoroutine(tutorialScript());
        }
        cb?.Invoke();
    }

}
