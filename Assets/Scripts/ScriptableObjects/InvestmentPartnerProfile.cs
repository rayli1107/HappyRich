using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(
        fileName = "Investment Partner Profile",
        menuName = "ScriptableObjects/Investment Partner Profile")]
    public class InvestmentPartnerProfile : ScriptableObject
    {
        public Vector2Int cashRange;
        public int cashIncrement;
        public int distributionWeight;
    }
}
