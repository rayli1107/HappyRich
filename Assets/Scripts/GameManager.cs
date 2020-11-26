﻿using System.Collections;
using System.Collections.Generic;
using Transaction;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int defaultHappiness = 50;

    public static GameManager Instance { get; private set; }
    public Localization Localization { get; private set; }
    public Player player { get; private set; }

    public StateMachine.StateMachine StateMachine { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        Localization = new Localization();
        StateMachine = new StateMachine.StateMachine();
        StateMachine.Start();
    }

    public void CreatePlayer()
    {
        player = new Player(
            JobManager.Instance.FindInitialProfession(),
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
            player.AddCash(-1 * amount);
            handler.OnTransactionSuccess();
        }
        else
        {
            new Actions.TakePersonalLoan(player, amount, handler).Start();
        }
    }
}
