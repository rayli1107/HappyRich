using Assets;
using PlayerInfo;
using System;
using System.Collections.Generic;
using UI;
using UI.Panels.Templates;
using UnityEngine;

using BusinessEntity = System.Tuple<
    Assets.PartialInvestment, Assets.AbstractBusiness>;
using StartupEntity = System.Tuple<
    Assets.PartialInvestment, Assets.Startup>;

namespace Actions
{
    public static class StartupPublicAction
    {
        private const int detailFontSize = 36;
/*
        private static void detailhandler(
            StartupEntity entity,
            PublicCompany publicCompany)
        {
            Localization local = Localization.Instance;
            int ownerIncome = Mathf.FloorToInt(
                entity.Item1.equity * publicCompany.income);
            List<string> messages = new List<string>()
            {
                string.Format(
                    "Company Valuation: {0}",
                    local.GetCurrency(publicCompany.value)),
                string.Format(
                    "Total Revenue: {0}",
                     local.GetCurrency(publicCompany.)),
                string.Format(
                    "Original Loan: {0}",
                    local.GetCurrency(publicCompany.originalLoanAmount, true)),
                string.Format(
                    "Accrued Interest: {0}",
                    local.GetCurrency(publicCompany.originalInterest, true)),
                string.Format(
                    "Refinanced Loan: {0}",
                    local.GetCurrency(publicCompany.combinedLiability.amount, true)),
                string.Format(
                    "Annual Interest: {0}",
                    local.GetCurrency(publicCompany.combinedLiability.expense, true)),
                string.Format(
                    "Annual Profit: {0}",
                    local.GetCurrency(publicCompany.income)),
                string.Format(
                    "Your Ownership Equity: {0}",
                    local.GetPercent(entity.Item1.equity, false)),
                string.Format(
                    "Your Annual Profit: {0}",
                    local.GetCurrency(ownerIncome))
            };

            SimpleTextMessageBox msgBox = UIManager.Instance.ShowSimpleMessageBox(
                string.Join("\n", messages), ButtonChoiceType.OK_ONLY, null);
            msgBox.text.fontSizeMax = detailFontSize;
        }*/

        public static void Run(
            Player player,
            StartupEntity entity,
            int value,
            int income,
            Action callback)
        {
            Startup startup = entity.Item2;
            BusinessData data = new BusinessData();
            data.InitializePublicCompany(startup, value, income);
            startup.ClearPrivateLoan();

            PublicCompany company = new PublicCompany(data);
            PartialInvestment partialEntity = entity.Item1;
            TransactionManager.ListPublicCompany(player, partialEntity, company);

            Localization local = Localization.Instance;
            string message = string.Format(
                "Congratulations! Your startup {0} went public! " +
                "It's valued at {1} with a total revenue of {2}.",
                local.GetBusinessDescription(entity.Item2.label),
                local.GetCurrency(value),
                local.GetCurrency(income));
            UIManager.Instance.ShowSimpleMessageBox(
                message,
                ButtonChoiceType.OK_ONLY,
                _ => callback?.Invoke(),
                () => partialEntity.OnDetail(player, null));
//                () => detailhandler(entity, company));
        }

    }
    public static class StartupAcquiredAction
    {
        private const int detailFontSize = 36;

        private static void detailHandler(
            StartupEntity entity,
            int price)
        {
            Localization local = Localization.Instance;
            int totalLoan = entity.Item2.combinedLiability.amount;
            int interest = entity.Item2.accruedDelayedInterest;
            int returns = price - totalLoan - interest;
            int ownerReturn = Mathf.FloorToInt(entity.Item1.equity * returns);
            List<string> messages = new List<string>()
            {
                string.Format(
                    "Acquisition Price: {0}",
                    local.GetCurrency(price)),
                string.Format(
                    "Current Loans: {0}",
                    local.GetCurrency(totalLoan, true)),
                string.Format(
                    "Interest Fees: {0}",
                    local.GetCurrency(interest, true)),
                string.Format(
                    "Total Return: {0}",
                    local.GetCurrency(returns)),
                string.Format(
                    "Your Ownership Equity: {0}",
                    local.GetPercent(entity.Item1.equity, false)),
                string.Format(
                    "Your Total Return: {0}",
                    local.GetCurrency(ownerReturn))
            };
            SimpleTextMessageBox msgBox = UIManager.Instance.ShowSimpleMessageBox(
                string.Join("\n", messages), ButtonChoiceType.OK_ONLY, null);
            msgBox.text.fontSizeMax = detailFontSize;
        }

        private static void messageHandler(
            Player player,
            StartupEntity entity,
            int price,
            Action callback)
        {
            price -= entity.Item2.accruedDelayedInterest;
            TransactionManager.SellInvestment(
                player, entity.Item1, entity.Item2, price, false);
            callback?.Invoke();
        }

        public static void Run(
            Player player,
            StartupEntity entity,
            int price,
            Action callback)

        {
            Localization local = Localization.Instance;

            string message = string.Format(
                "Congratulations! Your startup {0} got acquired for {1}!",
                local.GetBusinessDescription(entity.Item2.label),
                local.GetCurrency(price));
            UI.UIManager.Instance.ShowSimpleMessageBox(
                message,
                ButtonChoiceType.OK_ONLY,
                _ => messageHandler(player, entity, price, callback),
                () => detailHandler(entity, price));
        }
    }

    public static class StartupBankruptAction
    {
        public static void Run(
            StartupEntity entity,
            Action callback)
        {
            string message = string.Format(
                "Unfortunately your startup {0} ran out of funds and went bankrupt.",
                Localization.Instance.GetBusinessDescription(entity.Item2.label));
            UIManager.Instance.ShowSimpleMessageBox(
                message, ButtonChoiceType.OK_ONLY, _ => callback?.Invoke());
        }
    }
}
