using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "Profession", menuName = "ScriptableObjects/Profession")]
    public class Profession : ScriptableObject
    {
        public bool fullTime;
        public bool searchable;
        public string professionName;
        public int salary;
        public int jobCost;
        public int personalExpenses;
        public int autoLoan;
        public int costPerChild;
        public int startingCash;
        public int startingAge;
        public Sprite image;
    }
}
