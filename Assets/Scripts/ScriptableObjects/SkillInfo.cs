using UnityEngine;

namespace ScriptableObjects
{
    public enum SkillType
    {
        REAL_ESTATE_VALUATION,
        BUSINESS_OPERATIONS,
        PUBLIC_SPEAKING,
        STOCK_EVALUATION,
        ASSET_MANAGEMENT,
    }

    [CreateAssetMenu(fileName = "Skill Info", menuName = "ScriptableObjects/Skill Info")]
    public class SkillInfo : ScriptableObject
    {
        public SkillType skillType;
        public string skillName;
        public string skillDescription;
    }
}
