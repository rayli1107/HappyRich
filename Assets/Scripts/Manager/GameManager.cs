using PlayerInfo;
using PlayerState;
using ScriptableObjects;
using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField]
    private int _defaultHappiness = 50;
    [SerializeField]
    private int _requiredHappiness = 50;
    [SerializeField]
    private int _retirementAge = 60;
    [SerializeField]
    private bool _cheatMode = false;
#pragma warning restore 0649

    public int defaultHappiness => _defaultHappiness;
    public int requiredHappiness => _requiredHappiness;
    public int retirementAge => _retirementAge;
    public bool cheatMode => _cheatMode;

    public static GameManager Instance { get; private set; }

    public PersistentGameData PersistentGameData =>
        GameSaveLoadManager.Instance.persistentGameData;
    public GameInstanceData GameData => PersistentGameData.gameInstanceData;
    public Player player =>
        GameSaveLoadManager.Instance.persistentGameData.gameInstanceData.playerData;

    public StateMachine.StateMachine StateMachine { get; private set; }
    public System.Random Random { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        Random = new System.Random(System.Guid.NewGuid().GetHashCode());

        StateMachine = new StateMachine.StateMachine();
        StateMachine.Start(null);
    }

    public void CreatePlayer(Profession profession)
    {
        GameData.playerData = new Player(profession, defaultHappiness);
        player.personality = MentalStateManager.Instance.GetPersonality(player, Random);
    }

    // Update is called once per frame
    void Update()
    {
        StateMachine.Update();
    }
}
