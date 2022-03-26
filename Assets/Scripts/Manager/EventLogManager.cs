using PlayerInfo;
using ScriptableObjects;
using System.Collections.Generic;
using UnityEngine;

public class EventSnapshot
{
    public int cash { get; private set; }
    public int cashflow { get; private set; }
    public int networth { get; private set; }
    public int happiness { get; private set; }
    public int financialIndependenceProgress { get; private set; }

    public EventSnapshot(Player player)
    {
        Snapshot snapshot = new Snapshot(player);
        cash = snapshot.cash;
        cashflow = snapshot.actualCashflow;
        networth = snapshot.netWorth;
        happiness = snapshot.happiness;
        financialIndependenceProgress = snapshot.financialIndependenceProgress;
    }
}

public class EventLogYearContext
{
    public int age { get; private set; }
    public EventSnapshot yearStartSnapshot { get; private set; }
    public EventSnapshot yearEndSnapshot { get; private set; }
    public List<string> messages { get; private set; }

    public EventLogYearContext(Player player)
    {
        age = player.age;
        yearStartSnapshot = new EventSnapshot(player);
        messages = new List<string>();
    }

    public void OnYearEnd(Player player)
    {
        yearEndSnapshot = new EventSnapshot(player);
    }

    public void Log(string message)
    {
        Debug.Log(message);
        messages.Add(message);
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

    public void LogFormat(string format, params object[] args)
    {
        Log(string.Format(format, args));
    }

    public void Log(string message)
    {
        annualEventLogs.Last?.Value?.Log(message);
    }
}
