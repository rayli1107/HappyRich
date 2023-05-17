using Actions;
using Assets;
using PlayerInfo;
using ScriptableObjects;
using System;
using System.Collections.Generic;
using UnityEngine;

public class RiskyInvestmentManager : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField]
    private string[] _startupIdeas;
    [SerializeField]
    private StartupExitReturnProfile _lowRiskReturnProfile;
    [SerializeField]
    private StartupExitReturnProfile _mediumRiskReturnProfile;
    [SerializeField]
    private StartupExitReturnProfile _highRiskReturnProfile;
    [SerializeField]
    private int _turnCount = 5;
#pragma warning restore 0649

    public static RiskyInvestmentManager Instance { get; private set; }
    private System.Random _random;

    private void Awake()
    {
        Instance = this;
    }

    private void AnalyzeReturnProfile(StartupExitReturnProfile profile, string label)
    {
        float value = profile.publicThreshold * profile.publicReturn;
        value += (profile.acquiredThreshold - profile.publicThreshold) * profile.acquiredReturn;
        Debug.LogFormat("{0} {1}", label, value);
    }

    public void Initialize(System.Random random)
    {
        _random = random;
        AnalyzeReturnProfile(_lowRiskReturnProfile, "Low Risk");
        AnalyzeReturnProfile(_mediumRiskReturnProfile, "Medium Risk");
        AnalyzeReturnProfile(_highRiskReturnProfile, "High Risk");
    }

    private TimedInvestmentData getStartupInvestmentData(
        StartupExitReturnProfile profile, int originalPrice, int turnsLeft)
    {
        TimedInvestmentData data = new TimedInvestmentData();
        float value = (float)_random.NextDouble();
        if (value < profile.publicThreshold)
        {
            data.InitializeStartupIPOInvestmentData(
                originalPrice, turnsLeft, profile.publicReturn);
        }
        else if (value < profile.acquiredThreshold)
        {
            data.InitializeStartupAcquiredInvestmentData(
                originalPrice, turnsLeft, profile.acquiredReturn);
        }
        else
        {
            data.InitializeStartupFailedInvestmentData(
                originalPrice, turnsLeft);
        }
        return data;
    }

    public Action<Action> GetMarketEvent(Player player, System.Random random)
    {
        string idea = _startupIdeas[_random.Next(_startupIdeas.Length)];
        StartupExitReturnProfile profile;
        switch (_random.Next(3))
        {
            case 0:
                profile = _lowRiskReturnProfile;
                break;
            case 1:
                profile = _mediumRiskReturnProfile;
                break;
            default:
                profile = _highRiskReturnProfile;
                break;
        }

        return cb => StartupInvestmentAction.Start(
            player,
            idea,
            _turnCount,
            (price, turns) => getStartupInvestmentData(profile, price, turns),
            cb);
    }
}
