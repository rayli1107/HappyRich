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
    [SerializeField]
    private int _numberOfChoices = 2;
#pragma warning restore 0649

    public static SkillManager Instance { get; private set; }

    private Dictionary<SkillType, SkillInfo> _skillInfo;
    public List<SkillInfo> currentAvailableSkills { get; private set; }

    private void Awake()
    {
        Instance = this;

        _skillInfo = new Dictionary<SkillType, SkillInfo>();
        foreach (SkillInfo skillInfo in _skills)
        {
            _skillInfo[skillInfo.skillType] = skillInfo;
        }
        currentAvailableSkills = new List<SkillInfo>();
    }

    public SkillInfo GetSkillInfo(SkillType skillType)
    {
        return _skillInfo[skillType];
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
