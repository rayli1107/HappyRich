using UnityEngine;

namespace ScriptableObjects
{
    public enum SpecialistType
    {
        REAL_ESTATE_BROKER
    }

    [CreateAssetMenu(fileName = "Specialist", menuName = "ScriptableObjects/Specialist")]
    public class SpecialistInfo : ScriptableObject
    {
        public SpecialistType specialistType;
        public string specialistName;
        public string specialistDescription;
    }
}
