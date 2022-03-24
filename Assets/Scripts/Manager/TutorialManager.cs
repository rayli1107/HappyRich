using Actions;
using System;
using System.Collections.Generic;
using TMPro;
using UI.Panels.Templates;
using UnityEngine;

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
    private bool _enableTutorial;

    public TutorialAction GameInitOnce { get; private set; }
    public TutorialAction GameStartOnce { get; private set; }
    public TutorialAction InvestmentOnce { get; private set; }
    public TutorialAction JobSearchOnce { get; private set; }
    public TutorialAction SelfImprovementOnce { get; private set; }
    public TutorialAction NetworkingOnce { get; private set; }

    private void Awake()
    {
        Instance = this;
        _enableTutorial = false;

        GameInitOnce = new TutorialAction(GetGameInitMessageAction);
        GameStartOnce = new TutorialAction(GetGameStartMessageAction);
        InvestmentOnce = new TutorialAction(GetInvestmentAction);
        JobSearchOnce = new TutorialAction(GetJobSearchAction);
        SelfImprovementOnce = new TutorialAction(GetSelfImprovementAction);
        NetworkingOnce = new TutorialAction(GetNetworkingAction);
    }

    private void enableTutorialMessageHandler(ButtonType button, Action callback)
    {
        _enableTutorial = button == ButtonType.OK;
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
        bool enable = force || _enableTutorial;
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
}
