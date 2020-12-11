using UnityEngine;

namespace UI.Panels.Templates
{
    public class ItemListPanel : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField]
        private GameObject _content;
#pragma warning restore 0649
        public GameObject content { get { return _content; } }
    }
}
