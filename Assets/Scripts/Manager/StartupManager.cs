﻿using Actions;
using Assets;
using PlayerInfo;
using ScriptableObjects;
using System;
using System.Collections.Generic;
using UI.Panels.Templates;
using UnityEngine;

using Investment = System.Tuple<InvestmentPartner, int>;
using StartupEntity = System.Tuple<
    Assets.PartialInvestment, Assets.Startup>;

public class StartupManager : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField]
    private int _maxStartupLoanLTV = 20;
    [SerializeField]
    private int _loanAgentStartupLoanLTV = 50;
    [SerializeField]
    private string[] _startupIdeas;
    [SerializeField]
    private Vector3Int _startupPriceRange = new Vector3Int(2, 10, 500000);
    [SerializeField]
    private Vector2Int _startupDuration = new Vector2Int(4, 7);
    [SerializeField]
    private int _startupFailedWeight = 14;
    [SerializeField]
    private int _startupAcquiredWeight = 5;
    [SerializeField]
    private int _startupPublicWeight = 1;
    [SerializeField]
    private Vector2Int _startupAcquiredValueMultiplier = new Vector2Int(3, 5);
    [SerializeField]
    private Vector2Int _startupPublicValueMultiplier = new Vector2Int(5, 10);
    [SerializeField]
    private Vector2Int _startupPublicIncomeMultiplier = new Vector2Int(3, 6);

#pragma warning restore 0649

    public static StartupManager Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }

    public void Initialize(System.Random random)
    {
    }

    public BuyInvestmentContext GetStartupInvestmentAction(
        Player player, System.Random random)
    {
        string idea = _startupIdeas[random.Next(_startupIdeas.Length)];
        string label = string.Format("Startup: {0}", idea);
        int cost = random.Next(
            _startupPriceRange.x, _startupPriceRange.y + 1) * _startupPriceRange.z;
        int duration = random.Next(_startupDuration.x, _startupDuration.y + 1);
        int ltv = player.specialists.Exists(
            s => s.specialistType == SpecialistType.LOAN_AGENT) ?
            _loanAgentStartupLoanLTV : _maxStartupLoanLTV;
        Startup startup = new Startup(idea, label, cost, duration, ltv, ltv);
        return new BuyInvestmentContext(
            startup, PurchaseStartupAction.GetBuyAction(player, startup));
    }

    private void resolveStartup(
        Player player,
        System.Random random,
        StartupEntity entity,
        Action callback)
    {
        int value = random.Next(
            _startupPublicWeight +
            _startupAcquiredWeight +
            _startupFailedWeight);
        if (value < _startupPublicWeight)
        {
            int multiplier = random.Next(
                _startupPublicValueMultiplier.x,
                _startupPublicValueMultiplier.y + 1);
            int newValue = multiplier * entity.Item2.originalPrice;
            multiplier = random.Next(
                _startupPublicIncomeMultiplier.x,
                _startupPublicIncomeMultiplier.y + 1);
            int income = newValue * multiplier / 100;
            StartupPublicAction.Run(player, entity, newValue, income, callback);
            return;
        }

        value -= _startupPublicWeight;
        if (value < _startupAcquiredWeight)
        {
            int multiplier = random.Next(
                _startupAcquiredValueMultiplier.x,
                _startupAcquiredValueMultiplier.y + 1);
            int price = multiplier * entity.Item2.originalPrice;
            StartupAcquiredAction.Run(player, entity, price, callback);
            return;
        }

        StartupBankruptAction.Run(entity, callback);
    }

    public Action<Action> GetResolveStartupAction(Player player, System.Random random)
    {
        List<StartupEntity> exitedStartups = player.portfolio.RemoveExitedStartups();
        List<Action<Action>> exitActions = new List<Action<Action>>();
        foreach (StartupEntity entity in exitedStartups)
        {
            exitActions.Add(cb => resolveStartup(player, random, entity, cb));
        }
        return CompositeActions.GetAndAction(exitActions);
    }
}
