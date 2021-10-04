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

    private void Awake()
    {
        Instance = this;

        _specialistInfo = new Dictionary<SpecialistType, SpecialistInfo>();
        foreach (SpecialistInfo info in _specialists)
        {
            _specialistInfo[info.specialistType] = info;
        }
    }

    public SpecialistInfo GetSpecialistInfo(SpecialistType specialistType)
    {
        return _specialistInfo[specialistType];
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
