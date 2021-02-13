using PlayerInfo;
using ScriptableObjects;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField]
    private SkillInfo[] _skills;
    [SerializeField]
    private int _initialCost = 20000;
    [SerializeField]
    private int _additionalCost = 10000;
#pragma warning restore 0649

    public static SkillManager Instance { get; private set; }

    private Dictionary<SkillType, SkillInfo> _skillInfo;

    private void Awake()
    {
        Instance = this;

        _skillInfo = new Dictionary<SkillType, SkillInfo>();
        foreach (SkillInfo skillInfo in _skills)
        {
            _skillInfo[skillInfo.skillType] = skillInfo;
        }
    }

    public SkillInfo GetSkillInfo(SkillType skillType)
    {
        return _skillInfo[skillType];
    }

    public int GetCost(Player player)
    {
        return _initialCost + _additionalCost * player.skills.Count;
    }

    public SkillInfo GetSkill(Player player, System.Random random)
    {
        List<SkillInfo> newSkills = new List<SkillInfo>();
        foreach (SkillInfo skillInfo in _skills)
        {
            if (!player.HasSkill(skillInfo.skillType))
            {
                newSkills.Add(skillInfo);
            }
        }

        return newSkills.Count == 0 ? null : newSkills[random.Next(newSkills.Count)];
    }
}
