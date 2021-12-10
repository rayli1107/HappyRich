using Assets;
using PlayerInfo;
using ScriptableObjects;
using System.Collections.Generic;
using DistressedProperty = System.Tuple<
    Assets.PartialInvestment, Assets.DistressedRealEstate>;
using RentalProperty = System.Tuple<
    Assets.PartialInvestment, Assets.RentalRealEstate>;
using BusinessEntity = System.Tuple<
    Assets.PartialInvestment, Assets.AbstractBusiness>;
using StartupEntity = System.Tuple<
    Assets.PartialInvestment, Assets.Startup>;

using Investment = System.Tuple<InvestmentPartner, int>;
using UnityEngine;
using PlayerState;
using Actions;
using System;

public delegate void TransactionHandler(bool success);

public static class TransactionManager
{
    private static void buyRentalTransactionHandler(
        Player player,
        PartialInvestment partialAsset,
        RentalRealEstate rentalAsset,
        TransactionHandler handler,
        bool success)
    {
        if (success &&
            player != null &&
            partialAsset != null &&
            rentalAsset != null)
        {
            player.portfolio.rentalProperties.Add(
                new RentalProperty(partialAsset, rentalAsset));
        }
        handler?.Invoke(success);
    }

    public static void BuyRentalRealEstate(
        Player player,
        PartialInvestment partialasset,
        RentalRealEstate rentalAsset,
        TransactionHandler handler)
    {
        TryDebit.Run(
            player,
            partialasset.fundsNeeded,
            (bool b) => buyRentalTransactionHandler(
                player, partialasset, rentalAsset, handler, b));
    }

    private static void buyDistressedTransactionHandler(
        Player player,
        PartialInvestment partialAsset,
        DistressedRealEstate distressedAsset,
        TransactionHandler handler,
        bool success)
    {
        if (success &&
            player != null &&
            partialAsset != null &&
            distressedAsset != null)
        {
            player.portfolio.distressedProperties.Add(
                new DistressedProperty(partialAsset, distressedAsset));
        }
        handler?.Invoke(success);
    }

    public static void BuyDistressedRealEstate(
        Player player,
        PartialInvestment partialAsset,
        DistressedRealEstate distressedAsset,
        TransactionHandler handler)
    {
        TryDebit.Run(
            player,
            partialAsset.fundsNeeded,
            (bool b) => buyDistressedTransactionHandler(
                player, partialAsset, distressedAsset, handler, b));
    }

    private static void buyBusinessHandler(
        Player player,
        PartialInvestment partialAsset,
        AbstractBusiness asset,
        TransactionHandler handler,
        bool success)
    {
        if (success &&
            player != null &&
            partialAsset != null &&
            asset != null)
        {
            player.portfolio.businessEntities.Add(
                new BusinessEntity(partialAsset, asset));
        }
        handler?.Invoke(success);
    }

    public static void BuyBusiness(
        Player player,
        PartialInvestment partialasset,
        AbstractBusiness asset,
        TransactionHandler handler)
    {
        TryDebit.Run(
            player,
            partialasset.fundsNeeded,
            (bool b) => buyBusinessHandler(
                player, partialasset, asset, handler, b));
    }

    private static void buyStartupHandler(
        Player player,
        PartialInvestment partialAsset,
        Startup asset,
        TransactionHandler handler,
        bool success)
    {
        if (success &&
            player != null &&
            partialAsset != null &&
            asset != null)
        {
            player.portfolio.startupEntities.Add(
                new StartupEntity(partialAsset, asset));
        }
        handler?.Invoke(success);
    }

    public static void BuyStartup(
        Player player,
        PartialInvestment partialasset,
        Startup asset,
        TransactionHandler handler)
    {
        TryDebit.Run(
            player,
            partialasset.fundsNeeded,
            (bool b) => buyStartupHandler(
                player, partialasset, asset, handler, b));
    }

    private static void buyLuxuryItemHandler(
        Player player,
        LuxuryItem item,
        TransactionHandler handler,
        bool success)
    {
        if (success)
        {
            player.portfolio.luxuryItems.Add(item);
            player.AddMentalState(
                new LuxuryHappinessState(
                    player, LuxuryManager.Instance.luxuryHappinessDuration));
        }
        handler?.Invoke(success);
    }

    public static void BuyLuxuryItem(
        Player player,
        LuxuryItem item,
        TransactionHandler handler)
    {
        TryDebit.Run(player, item.value, b => buyLuxuryItemHandler(player, item, handler, b));
    }


    private static void applyJobTransactionHandler(
        Player player, Profession job, TransactionHandler handler, bool success)
    {
        if (success)
        {
            player.AddJob(job);
        }
        handler?.Invoke(success);
    }

    public static void ApplyJob(
        Player player, Profession job, TransactionHandler handler)
    {
        TryDebit.Run(
            player,
            job.jobCost,
            (bool b) => applyJobTransactionHandler(player, job, handler, b));
    }

    public static void BuyStock(
        Player player,
        AbstractStock stock,
        int amount,
        TransactionHandler handler)
    {
        bool success = false;
        int cost = amount * stock.value;

        if (player.cash >= cost)
        {
            player.portfolio.AddCash(-1 * cost);
            player.portfolio.AddStock(stock, amount);
            success = true;
        }

        handler?.Invoke(success);
    }

    public static void SellStock(
        Player player,
        AbstractStock stock,
        int amount,
        TransactionHandler handler)
    {
        bool success = false;
        if (player.portfolio.TryRemoveStock(stock, amount))
        {
            player.portfolio.AddCash(amount * stock.value);
            success = true;
        }

        handler?.Invoke(success);
    }

    private static void learnSkillTransactionHandler(
        Player player, SkillInfo skill, TransactionHandler handler, bool success)
    {
        if (success)
        {
            player.AddSkill(skill);
        }
        handler?.Invoke(success);
    }

    public static void LearnSkill(
        Player player,
        SkillInfo skill,
        int cost,
        TransactionHandler handler)
    {
        TryDebit.Run(
            player,
            cost,
            (bool b) => learnSkillTransactionHandler(player, skill, handler, b));
    }

    public static void SellInvestment(
        Player player,
        PartialInvestment partialAsset,
        AbstractInvestment asset,
        int price,
        bool returnCapital)
    {
        List<Investment> returnedCapitalList =
            InvestmentManager.Instance.CalculateReturnedCapitalForSale(
                asset, partialAsset, price, returnCapital);
        foreach (Investment returnedCapital in returnedCapitalList)
        {
            InvestmentPartner partner = returnedCapital.Item1;
            int amount = returnedCapital.Item2;
            if (partner == null)
            {
                player.portfolio.AddCash(amount);
            }
            else
            {
                partner.cash += amount;
            }
        }
    }

    public static void SellRentalProperty(Player player, int index, int price)
    {
        SellInvestment(
            player,
            player.portfolio.rentalProperties[index].Item1,
            player.portfolio.rentalProperties[index].Item2,
            price,
            true);
        player.portfolio.rentalProperties.RemoveAt(index);
    }

    public static void RefinanceProperty(
        Player player,
        PartialInvestment partialAsset,
        RefinancedRealEstate refinancedAsset)
    {
        List<Investment> returnedCapitalList =
            InvestmentManager.Instance.CalculateReturnedCapitalForRefinance(
                refinancedAsset, partialAsset);
        foreach (Investment returnedCapital in returnedCapitalList)
        {
            InvestmentPartner partner = returnedCapital.Item1;
            int amount = returnedCapital.Item2;
            if (partner == null)
            {
                player.portfolio.AddCash(amount);
            }
            else
            {
                partner.cash += amount;
            }
        }
        partialAsset.Refinance(refinancedAsset);
        player.portfolio.rentalProperties.Add(
            new RentalProperty(partialAsset, refinancedAsset));
    }

    public static void ListPublicCompany(
        Player player,
        PartialInvestment partialAsset,
        PublicCompany company)
    {
        partialAsset.Restructure(company);
        player.portfolio.businessEntities.Add(
            new BusinessEntity(partialAsset, company));
    }

    private static void buyTimedInvestmentDebitHandler(
        Player player,
        AbstractTimedInvestment investment,
        TransactionHandler handler,
        bool success)
    {
        if (success)
        {
            Debug.Log("Added timed investment");
            player.portfolio.timedInvestments.Add(investment);
        }
        handler?.Invoke(success);
    }

    public static void BuyTimedInvestment(
        Player player,
        AbstractTimedInvestment investment,
        TransactionHandler handler)
    {
        int price = investment.originalPrice;
        TryDebit.Run(
            player,
            investment.originalPrice,
            (bool b) => buyTimedInvestmentDebitHandler(player, investment, handler, b));
    }

    private static void payLoanPrincipalHandler(
        bool success,
        AbstractLiability loan,
        int amount,
        TransactionHandler handler)
    {
        if (success)
        {
            loan.PayOff(amount);
        }
        handler?.Invoke(success);
    }

    public static void PayLoanPrincipal(
        Player player,
        AbstractLiability loan,
        int amount,
        TransactionHandler handler)
    {
        int payAmount = Mathf.Min(amount, loan.amount);
        TryDebit.Run(
            player,
            amount,
            b => payLoanPrincipalHandler(b, loan, amount, handler));
    }
}
