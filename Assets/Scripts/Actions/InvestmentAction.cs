using Assets;
using PlayerInfo;
using System;
using UI;
using UI.Panels.Templates;

namespace Actions
{
    /*
     *public enum InvestmentState
        {
            kStart,
            kPurchasing,
            kCancelling
        }
        */
    public static class BuyInvestmentUtiliy
    {
        public static void messageBoxHandler(
            ButtonType buttonType,
            PartialInvestment partialAsset,
            Action<bool> callback)
        {
            if (buttonType == ButtonType.OK)
            {
                partialAsset.OnPurchase();
                callback?.Invoke(true);
            }
            else
            {
                partialAsset.OnPurchaseCancel();
                callback?.Invoke(false);
            }
        }

        public static void GetBuyAction(
            Player player,
            AbstractInvestment asset,
            Action<PartialInvestment> showPurchasePanelFn)
        {
            PartialInvestment partialAsset = new PartialInvestment(
                asset,
                player.GetEquityPartners(),
                RealEstateManager.Instance.defaultEquitySplit,
                RealEstateManager.Instance.maxEquityShares);
            showPurchasePanelFn(partialAsset);
        }
    }

    public static class BuyRentalRealEstateAction
    {
        public static Action<Action<bool>> GetBuyAction(
            Player player,
            RentalRealEstate asset)
        {
            Action<TransactionHandler, PartialInvestment> startTransaction =
                (handler, partial) => TransactionManager.BuyRentalRealEstate(
                    player, partial, asset, handler);
            Action<PartialInvestment, Action<bool>> showFn =
                (partialAsset, cb) => UIManager.Instance.ShowRentalRealEstatePurchasePanel(
                    asset,
                    partialAsset,
                    b => BuyInvestmentUtiliy.messageBoxHandler(b, partialAsset, cb),
                    handler => startTransaction(handler, partialAsset),
                    false);
            return cb => BuyInvestmentUtiliy.GetBuyAction(
                player, asset, p => showFn(p, cb));
        }
    }

    public static class BuyDistressedRealEstateAction
    {
        public static Action<Action<bool>> GetBuyAction(
            Player player,
            DistressedRealEstate asset)
        {
            Action<TransactionHandler, PartialInvestment> startTransaction =
                (handler, partialAsset) => TransactionManager.BuyDistressedRealEstate(
                    player, partialAsset, asset, handler);
            Action<PartialInvestment, Action<bool>> showFn =
                (partialAsset, cb) => UIManager.Instance.ShowDistressedRealEstatePurchasePanel(
                    asset,
                    partialAsset,
                    b => BuyInvestmentUtiliy.messageBoxHandler(b, partialAsset, cb),
                    handler => startTransaction(handler, partialAsset),
                    false);
            return cb => BuyInvestmentUtiliy.GetBuyAction(
                player, asset, p => showFn(p, cb));
        }
    }

    public static class PurchaseSmallBusinessAction
    {
        private static void nameInputCallback(
            Business business,
            TransactionHandler handler,
            string name)
        {
            business.SetName(name);

            Localization local = Localization.Instance;
            string message = string.Format(
                "After stabilizing business operations, {0} started generating " +
                "a total revenue of {1}",
                local.GetBusinessDescription(name),
                local.GetCurrency(business.totalIncome));
            UIManager.Instance.ShowSimpleMessageBox(
                message, ButtonChoiceType.OK_ONLY, _ => handler?.Invoke(true));
        }

        private static string confirmNameCallback(string name)
        {
            Localization local = Localization.Instance;
            return string.Format(
                "Name your business {0}?",
                local.GetBusinessDescription(name));
        }

        private static void startNameInput(
            Business business,
            TransactionHandler handler,
            bool transactionSuccess)
        {
            if (!transactionSuccess)
            {
                handler?.Invoke(false);
                return;
            }

            Localization local = Localization.Instance;
            string message = string.Format(
                "Choose a name for your {0} business.",
                local.GetBusinessDescription(business.description));
            UIManager.Instance.ShowSimpleTextPrompt(
                message,
                name => nameInputCallback(business, handler, name),
                confirmNameCallback,
                false,
                true);
        }

        public static Action<Action<bool>> GetBuyAction(
            Player player,
            Business business)
        {
            Action<TransactionHandler, PartialInvestment> startTransaction =
                (handler, partialAsset) => TransactionManager.BuyBusiness(
                    player,
                    partialAsset,
                    business,
                success => startNameInput(business, handler, success));

            Action<PartialInvestment, Action<bool>> showFn =
                (partialAsset, cb) => UIManager.Instance.ShowSmallBusinessPurchasePanel(
                    business,
                    partialAsset,
                    b => BuyInvestmentUtiliy.messageBoxHandler(b, partialAsset, cb),
                    handler => startTransaction(handler, partialAsset),
                    false);

            return cb => BuyInvestmentUtiliy.GetBuyAction(
                player, business, p => showFn(p, cb));
        }
    }

    public static class JoinFranchiseAction
    {
        private static void transactionHandler(
            Business business,
            TransactionHandler handler,
            bool transactionSuccess)
        {
            if (!transactionSuccess)
            {
                handler?.Invoke(false);
                return;
            }

            Localization local = Localization.Instance;
            string message = string.Format(
                "After stabilizing business operations, your {0} store " +
                "started generating a total revenue of {1}",
                local.GetBusinessDescription(business.description),
                local.GetCurrency(business.totalIncome));
            UIManager.Instance.ShowSimpleMessageBox(
                message, ButtonChoiceType.OK_ONLY, (_) => handler?.Invoke(true));
        }

        public static Action<Action<bool>> GetBuyAction(
            Player player,
            Business business)
        {
            Action<TransactionHandler, PartialInvestment> startTransaction =
                (handler, partialAsset) => TransactionManager.BuyBusiness(
                    player,
                    partialAsset,
                    business,
                    success => transactionHandler(business, handler, success));
            Action<PartialInvestment, Action<bool>> showFn =
                (partialAsset, cb) => UIManager.Instance.ShowFranchiseJoinPanel(
                    business,
                    partialAsset,
                    b => BuyInvestmentUtiliy.messageBoxHandler(b, partialAsset, cb),
                    handler => startTransaction(handler, partialAsset),
                    false);
            return cb => BuyInvestmentUtiliy.GetBuyAction(
                player, business, p => showFn(p, cb));
        }
    }

    public static class PurchaseStartupAction
    {
        private static void nameInputCallback(
            Startup startup,
            TransactionHandler handler,
            string name)
        {
            startup.SetName(name);
            handler?.Invoke(true);
        }

        private static string confirmNameCallback(string name)
        {
            Localization local = Localization.Instance;
            return string.Format(
                "Name your startup {0}?",
                local.GetBusinessDescription(name));
        }

        private static void startNameInput(
            Startup startup,
            TransactionHandler handler,
            bool transactionSuccess)
        {
            if (!transactionSuccess)
            {
                handler?.Invoke(false);
                return;
            }

            Localization local = Localization.Instance;
            string message = string.Format(
                "Choose a name for your {0} startup.",
                local.GetBusinessDescription(startup.description));
            UIManager.Instance.ShowSimpleTextPrompt(
                message,
                name => nameInputCallback(startup, handler, name),
                confirmNameCallback,
                false,
                true);
        }

        public static Action<Action<bool>> GetBuyAction(
            Player player,
            Startup startup)
        {
            Action<TransactionHandler, PartialInvestment> startTransaction =
                (handler, partialAsset) => TransactionManager.BuyStartup(
                    player,
                    partialAsset,
                    startup,
                success => startNameInput(startup, handler, success));

            Action<PartialInvestment, Action<bool>> showFn =
                (partialAsset, cb) => UIManager.Instance.ShowStartupPurchasePanel(
                    startup,
                    partialAsset,
                    b => BuyInvestmentUtiliy.messageBoxHandler(b, partialAsset, cb),
                    handler => startTransaction(handler, partialAsset),
                    false);

            return cb => BuyInvestmentUtiliy.GetBuyAction(
                player, startup, p => showFn(p, cb));
        }
    }
}
