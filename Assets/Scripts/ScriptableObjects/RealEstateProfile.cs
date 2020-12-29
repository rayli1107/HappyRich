using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(
        fileName = "Real Estate Profile",
        menuName = "ScriptableObjects/Real Estate Profile")]
    public class RealEstateProfile : ScriptableObject
    {
        public bool smallInvestment;
        public string description;
        public string label;
        public Vector2Int basePriceRange;
        public float priceVariance;
        public int priceIncrement;
        public Vector2Int rentalRange;
        public int rentalIncrement;
        public int[] unitCount;
        public bool commercial;
    }
}
