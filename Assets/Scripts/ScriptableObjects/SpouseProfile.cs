using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(
        fileName = "Spouse Profile",
        menuName = "ScriptableObjects/SpouseProfile")]
    public class SpouseProfile : ScriptableObject
    {
        public Vector2Int salaryRange;
        public int salaryIncrement;
        public Vector2 expenseRange;
        public int expenseBase;
        public int expenseIncrement;
        public int happiness;
    }
}
