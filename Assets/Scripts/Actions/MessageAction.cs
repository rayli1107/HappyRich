using Assets;
using PlayerInfo;
using System;
using System.Collections.Generic;
using TMPro;
using UI;
using UI.Panels.Templates;
using UnityEngine;

namespace Actions
{
    public static class MessageAction
    {
        private static void showMessage(
            string message,
            int fontSize,
            TMP_SpriteAsset spriteAsset,
            Action callback)
        {
            SimpleTextMessageBox msgBox = UIManager.Instance.ShowSimpleMessageBox(
                message, ButtonChoiceType.OK_ONLY, _ => callback?.Invoke());
            msgBox.GetComponentInChildren<TextMeshProUGUI>().fontSizeMax = fontSize;
            if (spriteAsset != null)
            {
                msgBox.text.spriteAsset = spriteAsset;
            }
        }

        public static Action<Action> GetAction(
            List<string> messages,
            int fontSize,
            TMP_SpriteAsset spriteAsset = null)
        {
            List<Action<Action>> actions = new List<Action<Action>>();
            foreach (string message in messages)
            {
                actions.Add(cb => showMessage(message, fontSize, spriteAsset, cb));
            }
            return CompositeActions.GetAndAction(actions);
        }
    }
}
