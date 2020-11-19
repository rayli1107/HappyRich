using UnityEngine;

namespace UI
{
    public class ModalObject : MonoBehaviour
    {
        protected virtual void OnEnable()
        {
            UIManager.Instance.RegisterModalItem(this);
        }

        protected virtual void OnDisable()
        {
            UIManager.Instance.UnregisterModalItem(this);
        }

        public virtual void EnableInput(bool enable)
        {

        }

        public virtual void ActivePanelUpdate()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public virtual void OnClickOutsideBoundary()
        {

        }
    }
}
