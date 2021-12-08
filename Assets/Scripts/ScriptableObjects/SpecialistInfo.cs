using UnityEngine;

namespace ScriptableObjects
{
    public enum SpecialistType
    {
        REAL_ESTATE_BROKER,
        ENTREPRENEUR,
        VENTURE_CAPITALIST,
        LOAN_AGENT
    }

    [CreateAssetMenu(fileName = "Specialist", menuName = "ScriptableObjects/Specialist")]
    public class SpecialistInfo : ScriptableObject
    {
        public SpecialistType specialistType;
        public string specialistName;
        public string specialistDescription;
    }
}
