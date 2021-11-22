using Actions;
using PlayerInfo;
using PlayerState;
using ScriptableObjects;
using System;
using System.Collections.Generic;
using UI.Panels.Templates;
using UnityEngine;

using RentalProperty = System.Tuple<
    Assets.PartialInvestment, Assets.RentalRealEstate>;
using BusinessEntity = System.Tuple<
    Assets.PartialInvestment, Assets.Business>;

namespace Events.Market
{
    public static class RentalBoomEvent
    {
        private static void handler(Player player, float modifier, Action callback)
        {
            foreach (RentalProperty rental in player.portfolio.rentalProperties)
            {
                rental.Item2.multiplier += modifier;
            }
            UI.UIManager.Instance.UpdatePlayerInfo(player);
            callback?.Invoke();
        }

        private static void run(Player player, float modifier, Action callback)
        {
            string message = string.Format(
                "Rental boom! All of your properties rental income increases " +
                "permanently by {0}.",
                Localization.Instance.GetPercent(modifier, false));
            UI.UIManager.Instance.ShowSimpleMessageBox(
                message,
                ButtonChoiceType.OK_ONLY,
                _ => handler(player, modifier, callback));
        }

        public static Action<Action> GetEvent(Player player, float modifier)
        {
            return cb => run(player, modifier, cb);
        }
    }

    public static class RentalCrashEvent
    {
        private static void handler(Player player, float modifier, Action callback)
        {
            foreach (RentalProperty rental in player.portfolio.rentalProperties)
            {
                rental.Item2.multiplier = Mathf.Max(
                    rental.Item2.multiplier - modifier, 0f);
            }
            UI.UIManager.Instance.UpdatePlayerInfo(player);
            callback?.Invoke();
        }

        private static void run(Player player, float modifier, Action callback)
        {
            string message = string.Format(
                "Rental crash! All of your properties rental income decreases " +
                "permanently by {0}.",
                Localization.Instance.GetPercent(modifier, false));
            UI.UIManager.Instance.ShowSimpleMessageBox(
                message,
                ButtonChoiceType.OK_ONLY,
                _ => handler(player, modifier, callback));
        }

        public static Action<Action> GetEvent(Player player, float modifier)
        {
            return cb => run(player, modifier, cb);
        }
    }

    public static class MarketBoomEvent
    {
        private static void handler(Player player, float modifier, Action callback)
        {
            foreach (BusinessEntity business in player.portfolio.businessEntities)
            {
                business.Item2.multiplier += modifier;
            }
            UI.UIManager.Instance.UpdatePlayerInfo(player);
            callback?.Invoke();
        }

        private static void run(Player player, float modifier, Action callback)
        {
            string message = string.Format(
                "Market boom! All of your business income increases " +
                "permanently by {0}.",
                Localization.Instance.GetPercent(modifier, false));
            UI.UIManager.Instance.ShowSimpleMessageBox(
                message,
                ButtonChoiceType.OK_ONLY,
                _ => handler(player, modifier, callback));
        }

        public static Action<Action> GetEvent(Player player, float modifier)
        {
            return cb => run(player, modifier, cb);
        }
    }

    public static class MarketCrashEvent
    {
        private static void handler(Player player, float modifier, Action callback)
        {
            foreach (BusinessEntity business in player.portfolio.businessEntities)
            {
                business.Item2.multiplier = Mathf.Max(
                    business.Item2.multiplier - modifier, 0f);
            }
            UI.UIManager.Instance.UpdatePlayerInfo(player);
            callback?.Invoke();
        }

        private static void run(Player player, float modifier, Action callback)
        {
            string message = string.Format(
                "Market crash! All of your business income decrease " +
                "permanently by {0}.",
                Localization.Instance.GetPercent(modifier, false));
            UI.UIManager.Instance.ShowSimpleMessageBox(
                message,
                ButtonChoiceType.OK_ONLY,
                _ => handler(player, modifier, callback));
        }

        public static Action<Action> GetEvent(Player player, float modifier)
        {
            return cb => run(player, modifier, cb);
        }
    }
}
