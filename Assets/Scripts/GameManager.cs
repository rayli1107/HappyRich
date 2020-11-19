using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public ScriptableObjects.Profession startingProfession;
    public int defaultHappiness = 50;
    public Player player { get; private set; }

    private void Awake()
    {
    }

    // Start is called before the first frame update
    void Start()
    {
        player = new Player(startingProfession, defaultHappiness);
        /*
        List<Income> incomeList = player.getIncomeList();
        int cashflow = 0;
        foreach (Income income in incomeList)
        {
            Debug.LogFormat("{0}: {1}", income.name, income.income);
            cashflow += income.income;
        }
        Debug.LogFormat("Cashflow: {0}", cashflow);
        */
        foreach (PlayerState.PlayerStateInterface state in player.playerStates)
        {
            int happiness = state.getHappiness(player);
            if (happiness != 0)
            {
                Debug.LogFormat("{0}: {1}", state.getName(), happiness);
            }
        }
        Debug.LogFormat("Happiness: {0}", player.getHappiness());
        UI.UIManager.Instance.UpdatePlayerInfo(player);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
