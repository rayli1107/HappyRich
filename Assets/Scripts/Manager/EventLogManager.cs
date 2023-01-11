using PlayerInfo;
using ScriptableObjects;
using System;
using System.Collections.Generic;
using UnityEngine;

using LogContext = System.Tuple<System.Action, string>;

public class EventSnapshot
{
    public int cash { get; private set; }
    public int happiness { get; private set; }
    public Snapshot snapshot { get; private set; }

    public EventSnapshot(Player player)
    {
        snapshot = new Snapshot(player);
        cash = player.cash;
        happiness = player.happiness;
    }
}

public class EventLogYearContext
{
    public int age { get; private set; }
    public EventSnapshot yearStartSnapshot { get; private set; }
    public EventSnapshot yearEndSnapshot { get; private set; }
    public List<LogContext> messages { get; private set; }

    public EventLogYearContext(Player player)
    {
        age = player.age;
        yearStartSnapshot = new EventSnapshot(player);
        messages = new List<LogContext>();
    }

    public void OnYearEnd(Player player)
    {
        yearEndSnapshot = new EventSnapshot(player);
    }

    public void Log(Action clickAction, string message)
    {
        Debug.Log(message);
        messages.Add(new LogContext(clickAction, message));
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
        Log(null, string.Format(format, args));
    }

    public void LogFormat(Action clickAction, string format, params object[] args)
    {
        Log(clickAction, string.Format(format, args));
    }

    public void Log(string message)
    {
        Log(null, message);
    }

    public void Log(Action clickAction, string message)
    {
        annualEventLogs.Last?.Value?.Log(clickAction, message);
    }
}
