using Actions;
using Assets;
using PlayerInfo;
using ScriptableObjects;
using UnityEngine;


public class RiskyInvestmentManager : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField]
    private string[] _startupIdeas;
#pragma warning restore 0649

    public static RiskyInvestmentManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void Initialize(System.Random random)
    {
    }
}
