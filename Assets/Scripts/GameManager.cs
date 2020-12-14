using System.Collections;
using System.Collections.Generic;
using Transaction;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int defaultHappiness = 50;

    public static GameManager Instance { get; private set; }
    public Player player { get; private set; }

    public StateMachine.StateMachine StateMachine { get; private set; }
    public System.Random Random { get; private set; }

    public StockManager StockManager { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        Random = new System.Random(System.Guid.NewGuid().GetHashCode());

        StockManager = GetComponent<StockManager>();
        StockManager.Initialize(Random);

        StateMachine = new StateMachine.StateMachine();
        StateMachine.Start();
    }

    public void CreatePlayer()
    {
        player = new Player(
            JobManager.Instance.FindInitialProfession(Random),
            defaultHappiness);
    }

    // Update is called once per frame
    void Update()
    {
        StateMachine.Update();
    }

    public void TryDebit(Player player, int amount, ITransactionHandler handler)
    {
        if (player.cash >= amount)
        {
            player.portfolio.AddCash(-1 * amount);
            handler.OnTransactionSuccess();
        }
        else
        {
            new Actions.TakePersonalLoan(player, amount, handler).Start();
        }
    }
}
