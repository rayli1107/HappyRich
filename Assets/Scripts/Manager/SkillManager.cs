using PlayerInfo;
using ScriptableObjects;
using System;
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
    [SerializeField]
    private int _numberOfChoices = 2;
#pragma warning restore 0649

    public static SkillManager Instance { get; private set; }

    private Dictionary<SkillType, SkillInfo> _skillInfo;
    private Dictionary<string, SkillInfo> _skillInfoByLabel;

    public List<SkillInfo> currentAvailableSkills { get; private set; }

    private void Awake()
    {
        Instance = this;

        _skillInfo = new Dictionary<SkillType, SkillInfo>();
        _skillInfoByLabel = new Dictionary<string, SkillInfo>();
        foreach (SkillInfo skillInfo in _skills)
        {
            Debug.Assert(!_skillInfo.ContainsKey(skillInfo.skillType));
            _skillInfo[skillInfo.skillType] = skillInfo;
            _skillInfoByLabel[GetSkillLabel(skillInfo)] = skillInfo;
        }
        currentAvailableSkills = new List<SkillInfo>();
    }

    public string GetSkillLabel(SkillInfo info)
    {
        return Enum.GetName(typeof(SkillType), info.skillType);
    }

    public SkillInfo GetSkillByLabel(string label)
    {
        SkillInfo result;
        if (_skillInfoByLabel.TryGetValue(label, out result))
        {
            return result;
        }
        string message = string.Format(
            "Cannot find Skill: {0}", label);
        Debug.LogException(new Exception(message));
        return null;
    }

    public SkillInfo GetSkillInfo(SkillType skillType)
    {
        SkillInfo result;
        if (_skillInfo.TryGetValue(skillType, out result))
        {
            return result;
        }
        string message = string.Format(
            "Cannot find Skill: {0}", skillType);
        Debug.LogException(new Exception(message));
        return null;
    }

    public int GetCost(Player player)
    {
        return _initialCost + _additionalCost * player.skills.Count;
    }

    public void OnPlayerTurnStart(Player player, System.Random random)
    {
        currentAvailableSkills.Clear();

        List<SkillInfo> availableSkills = new List<SkillInfo>();
        foreach (SkillInfo skillInfo in _skills)
        {
            if (!player.HasSkill(skillInfo.skillType))
            {
                availableSkills.Add(skillInfo);
            }
        }

        while (currentAvailableSkills.Count < _numberOfChoices && availableSkills.Count > 0) {
            int index = random.Next(availableSkills.Count);
            currentAvailableSkills.Add(availableSkills[index]);
            availableSkills.RemoveAt(index);
        }
    }
}
