using PlayerInfo;
using ScriptableObjects;
using System.Collections.Generic;
using UnityEngine;

public class SpecialistManager : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField]
    private SpecialistInfo[] _specialists;
#pragma warning restore 0649

    public static SpecialistManager Instance { get; private set; }

    private Dictionary<SpecialistType, SpecialistInfo> _specialistInfo;
    private Dictionary<string, SpecialistInfo> _specialistInfoByLabel;

    private void Awake()
    {
        Instance = this;

        _specialistInfo = new Dictionary<SpecialistType, SpecialistInfo>();
        _specialistInfoByLabel = new Dictionary<string, SpecialistInfo>();
        foreach (SpecialistInfo info in _specialists)
        {
            _specialistInfo[info.specialistType] = info;
            _specialistInfoByLabel[GetSpecialistLabel(info)] = info;
        }
    }

    public string GetSpecialistLabel(SpecialistInfo info)
    {
        return System.Enum.GetName(
            typeof(SpecialistType), info.specialistType);
    }

    public SpecialistInfo GetSpecialistInfo(SpecialistType specialistType)
    {
        SpecialistInfo result;
        if (_specialistInfo.TryGetValue(specialistType, out result))
        {
            return result;
        }
        string message = string.Format(
            "Cannot find Specialist: {0}", specialistType);
        Debug.LogException(new System.Exception(message));
        return null;
    }

    public SpecialistInfo GetSpecialistInfoByLabel(string label)
    {
        SpecialistInfo result;
        if (_specialistInfoByLabel.TryGetValue(label, out result))
        {
            return result;
        }
        string message = string.Format(
            "Cannot find Specialist: {0}", label);
        Debug.LogException(new System.Exception(message));
        return null;
    }


    public SpecialistInfo GetNewSpecialist(Player player, System.Random random)
    {
        List<SpecialistInfo> newSpecialists = new List<SpecialistInfo>();
        foreach (SpecialistInfo specialistInfo in _specialists)
        {
            if (!player.HasSpecialist(specialistInfo.specialistType))
            {
                newSpecialists.Add(specialistInfo);
            }
        }

        return newSpecialists.Count == 0 ? null : newSpecialists[random.Next(newSpecialists.Count)];
    }

    public bool HasNewSpecialistsAvailable(Player player)
    {
        return player.specialists.Count < _specialists.Length;
    }
}
