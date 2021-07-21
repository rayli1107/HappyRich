using Actions;
using Assets;
using PlayerInfo;
using PlayerState;
using ScriptableObjects;
using System.Collections.Generic;
using UnityEngine;

public class InvestmentManager : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField]
    private int _defaultAvailableInvestments = 2;
    [SerializeField]
    private int _hustledAvailableInvestments = 3;
#pragma warning restore 0649

    public static InvestmentManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public int getAvailableInvestments(Player player)
    {
        foreach (AbstractPlayerState state in player.states)
        {
            if (state is Hustling)
            {
                return _hustledAvailableInvestments;
            }
        }
        return _defaultAvailableInvestments;
    }
}
