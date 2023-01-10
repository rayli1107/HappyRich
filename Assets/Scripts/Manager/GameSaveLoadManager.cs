using PlayerInfo;
using PlayerState;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public partial class GameInstanceData
{

}

[Serializable]
public class PersistentGameData
{
    public GameInstanceData gameInstanceData;
}

public interface IGameDataVerifier
{
    void VerifyGameData(PersistentGameData data);
}

public interface IGameDataSaver
{
    void LoadGame(PersistentGameData data);
    void SaveGame(ref PersistentGameData data);
}

public class GameSaveLoadManager : MonoBehaviour
{
#pragma warning disable 0649
#pragma warning restore 0649

    public PersistentGameData persistentGameData { get; private set; }

    private List<IGameDataSaver> _gameDataSavers;

    public static GameSaveLoadManager Instance { get; private set; }

    private void Awake()
    {
        _gameDataSavers = new List<IGameDataSaver>();
        Instance = this;
    }

    public void Initialize()
    {
        persistentGameData = new PersistentGameData();
        persistentGameData.gameInstanceData = new GameInstanceData();
    }

    public void RegisterGameDataSaver(IGameDataSaver saver)
    {
        if (!_gameDataSavers.Contains(saver))
        {
            _gameDataSavers.Add(saver);
        }
    }

    public void SaveGame()
    {
        Debug.Log(JsonUtility.ToJson(persistentGameData, true));
    }
}

