using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(
        fileName = "Business Profile",
        menuName = "ScriptableObjects/Business Profile")]
    public class BusinessProfile : ScriptableObject
    {
        public string[] descriptions;
        public string label;
        public Vector2Int priceRange;
        public int priceIncrement;
        public Vector2Int minIncomeRange;
        public Vector2Int maxIncomeRange;
        public int incomeIncrement;
    }
}
