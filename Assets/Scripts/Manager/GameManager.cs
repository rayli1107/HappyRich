using PlayerInfo;
using ScriptableObjects;
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
    private bool _tutorialMode = false;
#pragma warning restore 0649

    public int defaultHappiness => _defaultHappiness;
    public int requiredHappiness => _requiredHappiness;
    public int retirementAge => _retirementAge;
    public bool tutorialMode => _tutorialMode;

    public static GameManager Instance { get; private set; }
    public Player player { get; private set; }

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
        player = new Player(profession, defaultHappiness);
    }

    // Update is called once per frame
    void Update()
    {
        StateMachine.Update();
    }
}
