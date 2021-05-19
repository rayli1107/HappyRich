using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(
        fileName = "Startup Exit Return Profile",
        menuName = "ScriptableObjects/Startup Exit Return Profile")]
    public class StartupExitReturnProfile : ScriptableObject
    {
        public float publicThreshold;
        public float publicReturn;
        public float acquiredThreshold;
        public float acquiredReturn;
    }
}
