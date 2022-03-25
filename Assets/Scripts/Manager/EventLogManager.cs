using PlayerInfo;
using ScriptableObjects;
using System.Collections.Generic;
using UnityEngine;

public class EventLogYearContext
{
    public int age { get; private set; }
    public int startingCash { get; private set; }
    public int startingCashflow { get; private set; }
    public int endCash { get; private set; }
    public int endCashflow { get; private set; }

    public bool yearEnded { get; private set; }

    public EventLogYearContext(Player player)
    {
        Snapshot snapshot = new Snapshot(player);
        age = player.age;
        startingCash = snapshot.cash;
        startingCashflow = snapshot.actualCashflow;
        yearEnded = false;
    }

    public void OnYearEnd(Player player)
    {
        Snapshot snapshot = new Snapshot(player);
        endCash = snapshot.cash;
        endCashflow = snapshot.actualCashflow;
        yearEnded = true;
    }
}

public class EventLogManager : MonoBehaviour
{
    public static EventLogManager Instance { get; private set; }
    public LinkedList<EventLogYearContext> annualEventLogs { get; private set; }

    private void Awake()
    {
        Instance = this;
        annualEventLogs = new LinkedList<EventLogYearContext>();
    }

    public void OnTurnStart(Player player)
    {
        annualEventLogs.AddLast(new EventLogYearContext(player));
    }

    public void OnTurnEnd(Player player)
    {
        annualEventLogs.Last.Value.OnYearEnd(player);
    }
}
