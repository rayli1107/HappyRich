using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "Luxury Item", menuName = "ScriptableObjects/Luxury Item")]
    public class LuxuryItemProfile : ScriptableObject
    {
        public string itemName;
        public Vector2Int itemPriceRange;
        public int itemIncrement;
    }
}
