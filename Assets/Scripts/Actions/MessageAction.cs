using Assets;
using PlayerInfo;
using System;
using System.Collections.Generic;
using UI;
using UI.Panels.Templates;

namespace Actions
{
    public static class MessageAction
    {
        public static Action<Action> GetAction(List<string> messages)
        {
            List<Action<Action>> actions = new List<Action<Action>>();
            foreach (string message in messages)
            {
                actions.Add(
                    cb => UIManager.Instance.ShowSimpleMessageBox(
                        message, ButtonChoiceType.OK_ONLY, _ => cb?.Invoke()));
            }
            return CompositeActions.GetAndAction(actions);
        }
    }

    public static class TutorialMessageAction
    {
        public static Action<Action> GetAction(List<string> messages)
        {
            if (GameManager.Instance.tutorialMode)
            {
                return MessageAction.GetAction(messages);
            }
            else
            {
                return cb => cb?.Invoke();
            }
        }

    }
}
