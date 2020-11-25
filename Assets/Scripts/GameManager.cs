using System.Collections;
using System.Collections.Generic;
using Transaction;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int defaultHappiness = 50;

    public static GameManager Instance { get; private set; }
    public Localization Localization { get; private set; }
    public Player player { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        Localization = new Localization();
        player = new Player(
            JobManager.Instance.FindInitialProfession(),
            defaultHappiness);
        StartCoroutine(WaitAndUpdateUI());
    }

    private IEnumerator WaitAndUpdateUI()
    {
        while (!UI.UIManager.Instance.ready)
        {
            yield return null;
        }
        UI.UIManager.Instance.UpdatePlayerInfo(player);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TryDebit(Player player, int amount, ITransactionHandler handler)
    {
        if (player.cash >= amount)
        {
            player.AddCash(-1 * amount);
            handler.OnTransactionSuccess();
        }
        else
        {
            new Actions.TakePersonalLoan(player, amount, handler).Start();
        }
    }
}
