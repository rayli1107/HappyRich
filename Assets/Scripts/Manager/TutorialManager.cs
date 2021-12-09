using Actions;
using System;
using System.Collections.Generic;
using TMPro;
using UI.Panels.Templates;
using UnityEngine;


public class TutorialManager : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField]
    private TMP_SpriteAsset _spriteAsset;
    [SerializeField]
    private int _iconIndexAge = 0;
    [SerializeField]
    private int _iconIndexCash = 1;
    [SerializeField]
    private int _iconIndexCashflow = 2;
    [SerializeField]
    private int _iconIndexFire = 3;
    [SerializeField]
    private int _iconIndexHappiness = 4;
#pragma warning restore 0649

    public static TutorialManager Instance { get; private set; }
    private bool _enableTutorial;

    private void Awake()
    {
        Instance = this;
        _enableTutorial = false;
    }

    private void enableTutorialMessageHandler(ButtonType button, Action callback)
    {
        _enableTutorial = button == ButtonType.OK;
        Debug.LogFormat("enableTutorialMessageHandler {0}", _enableTutorial);
        callback?.Invoke();
    }

    public Action<Action> GetEnableTutorialAction()
    {
        return cb => UI.UIManager.Instance.ShowSimpleMessageBox(
            "Enable tutorial?",
            ButtonChoiceType.OK_CANCEL,
            b => enableTutorialMessageHandler(b, cb));
    }

    private void showTutorialMessage(List<string> messages, Action callback)
    {
        if (_enableTutorial && messages != null)
        {
            MessageAction.GetAction(messages, _spriteAsset)?.Invoke(callback);
        }
        else
        {
            callback?.Invoke();
        }
    }

    public Action<Action> GetGameActionMessageAction()
    {
        Localization local = Localization.Instance;
        List<string> messages = new List<string>()
        {
            "The <sprite name=\"Cash\"> and <sprite name=\"Cashflow\"> icons " +
            "represent your current available cash and annual cashflow.",

            "The <sprite name=\"Happiness\"> icon represents your total happiness, " +
            "the <sprite name=\"Age\"> icon represents your current age, and the " +
            "<sprite name=\"Fire\"> icon represents your progress towards " +
            "financial indepence.",

            string.Format(
                "Financial indepence is achieved once your {0} exceeds your {1}.",
                local.colorWrap("passive income", Color.green),
                local.colorWrap("total expenses", new Color(0.72f, 0.43f, 0.48f))),

            "There are four types of actions, and you can perform one action every year.",

            string.Format(
                "{0} actions allow you to look for additional job to raise your active " +
                "income. You can have up to one full-time and one part-time job at the " +
                "same time.",
                local.colorWrap("Job Search", new Color(0.25f, 0.25f, 1f))),

            string.Format(
                "{0} actions help you become a better person, both in a professional " +
                "and personal sense.",
                local.colorWrap("Self Improvement", new Color(0.25f, 0.25f, 1f))),

            string.Format(
                "{0} actions allow you to meet more people who can help you in " +
                "different ways.",
                local.colorWrap("Networking", new Color(0.25f, 0.25f, 1f))),

            string.Format(
                "{0} actions allow you to invest in different assets that can help " +
                "generate passive income.",
                local.colorWrap("Investment", new Color(0.25f, 0.25f, 1f))),

            "Good luck and have fun!"
        };
        return cb => showTutorialMessage(messages, cb);
    }

    public Action<Action> GetGameInitMessageAction()
    {
        Localization local = Localization.Instance;
        string retireAge = GameManager.Instance.retirementAge.ToString();

        List<string> messages = new List<string>()
        {
            string.Format(
                "Welcome to the {0}!",
                local.colorWrap("Game of Prosperity", new Color(1f, 0.6f, 0f))),
            string.Format(
                "The goal of the game is to reach {0} and {1} before " +
                "the age of retirement ({2}).",
                local.colorWrap("100% Happiness", Color.yellow),
                local.colorWrap("100% Financial Independence", new Color(0.72f, 0.43f, 0.48f)),
                local.colorWrap(retireAge, Color.green)),
            string.Format(
                "First, please choose your starting profession.")
        };

        return cb => showTutorialMessage(messages, cb);
    }
}
