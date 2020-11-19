using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "Profession", menuName = "ScriptableObjects/Profession")]
    public class Profession : ScriptableObject
    {
        public string professionName;
        public int salary;
        public int personalExpenses;
        public int autoLoan;
        public int studentLoan;
        public int costPerChild;
        public int startingCash;
        public int startingAge;
    }
}
