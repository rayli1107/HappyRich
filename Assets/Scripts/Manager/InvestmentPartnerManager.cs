using Actions;
using Assets;
using InvestmentPartnerInfo;
using PlayerInfo;
using ScriptableObjects;
using System;
using System.Collections.Generic;
using UnityEngine;

public partial class GameInstanceData
{
    public List<InvestmentPartner> investmentPartners;
}

namespace InvestmentPartnerInfo
{
    public enum RiskTolerance
    {
        kLow = 0,
        kMedium = 1,
        kHigh = 2,
    }

    [Serializable]
    public class InvestmentPartner
    {
        [SerializeField]
        private int _partnerId;
        public int partnerId => _partnerId;

        [SerializeField]
        private string _name;
        public string name => _name;

        public int cash;

        [SerializeField]
        private int _duration;
        public int duration => _duration;

        [SerializeField]
        private int _initialDuration;

        [SerializeField]
        private RiskTolerance _riskTolerance;
        public RiskTolerance riskTolerance => _riskTolerance;

        public InvestmentPartner(
            int partnerId, string name, int cash, RiskTolerance riskTolerance, int duration)
        {
            _partnerId = partnerId;
            _name = name;
            this.cash = cash;
            _riskTolerance = riskTolerance;
            _initialDuration = duration;
            RefreshDuration();
        }

        public void OnTurnStart()
        {
            --_duration;
        }

        public void RefreshDuration()
        {
            _duration = _initialDuration;
        }
    }
}

public class InvestmentPartnerManager : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField]
    private InvestmentPartnerProfile[] _profiles;
    [SerializeField]
    private string[] _names;
    [SerializeField]
    private int _defaultDuration = 10;
    [SerializeField]
    private int _increment = 50000;
    [SerializeField]
    private Vector2 _partnerCashRange = new Vector2(0.5f, 1.5f);
    [SerializeField]
    private int _defaultPartnerCount = 2;
    [SerializeField]
    private int _publicSpeakingPartnerCount = 2;
    [SerializeField]
    private int _vcPartnerCount = 1;
#pragma warning restore 0649

    public static InvestmentPartnerManager Instance { get; private set; }
    public PersistentGameData PersistentGameData =>
        GameSaveLoadManager.Instance.persistentGameData;
    public GameInstanceData GameData => PersistentGameData.gameInstanceData;
    public int partnerCount => GameData.investmentPartners.Count;

    private void Awake()
    {
        Instance = this;        
    }

    private InvestmentPartner getPartner(int lo, int hi, System.Random random)
    {
        if (GameData.investmentPartners == null)
        {
            GameData.investmentPartners = new List<InvestmentPartner>();
        }

        int cash = Mathf.Max(1, random.Next(lo, hi + 1)) * _increment;
        string name = _names[random.Next(_names.Length)];
        RiskTolerance riskTolerance =
            random.Next(2) == 0 ? RiskTolerance.kLow : RiskTolerance.kHigh;
        int partnerId = GameData.investmentPartners.Count;
        InvestmentPartner partner = new InvestmentPartner(
            partnerId, name, cash, riskTolerance, _defaultDuration);
        GameData.investmentPartners.Add(partner);
        return partner;
    }

    private List<InvestmentPartner> getPartners(
        Player player, System.Random random, int count = 0)
    {
        int value = 0;
        foreach (AbstractAsset asset in player.portfolio.managedAssets)
        {
            value += asset.value - asset.combinedLiability.amount;
        }
        value = Mathf.Max(value, new Snapshot(player).netWorth);
        value = Mathf.Max(value, _increment);
        int lo = Mathf.FloorToInt(value * _partnerCashRange.x) / _increment;
        int hi = Mathf.FloorToInt(value * _partnerCashRange.y) / _increment;

        if (count <= 0)
        {
            count = _defaultPartnerCount;
            if (player.HasSkill(SkillType.PUBLIC_SPEAKING))
            {
                count += _publicSpeakingPartnerCount;
            }
        }

        List<InvestmentPartner> partners = new List<InvestmentPartner>();
        for (int i = 0; i < count; ++i)
        {
            partners.Add(getPartner(lo, hi, random));
        }
        return partners;
    }

    private Action<Action> getActionFromPartnerList(
        Player player,
        List<InvestmentPartner> partners)
    {
        List<Action<Action>> actions = new List<Action<Action>>();
        actions.Add(
            cb =>
            {
                EventLogManager.Instance.Log("Market Event - New Investors");
                cb?.Invoke();
            });
        foreach (InvestmentPartner partner in partners)
        {
            actions.Add(cb => FindNewInvestors.FindInvestor(player, partner, cb));
        }
        return CompositeActions.GetAndAction(actions);
    }

    public Action<Action> GetMarketEvent(Player player, System.Random random)
    {
        if (player.HasSpecialist(SpecialistType.VENTURE_CAPITALIST))
        {
            List<InvestmentPartner> partners = getPartners(
                player, random, _vcPartnerCount);
            return getActionFromPartnerList(player, partners);
        }
        return null;
    }

    public Action<Action> GetAction(Player player, System.Random random)
    {
        List<InvestmentPartner> partners = getPartners(player, random);
        return getActionFromPartnerList(player, partners);
    }

    public InvestmentPartner GetPartnerById(int partnerId)
    {
        if (partnerId < GameData.investmentPartners.Count)
        {
            return GameData.investmentPartners[partnerId];
        }
        string message = string.Format(
            "Cannot find partner with id {0}", partnerId);
        Debug.LogException(new Exception(message));
        return null;
    }
}
