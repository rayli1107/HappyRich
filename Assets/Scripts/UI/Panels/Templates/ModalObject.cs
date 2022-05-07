using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Panels.Templates
{
    public class ModalObject : MonoBehaviour
    {
        protected bool _enableInput { get; private set; }

        protected virtual void OnEnable()
        {
            UIManager.Instance.RegisterModalItem(this);
            _enableInput = true;
        }

        protected virtual void OnDisable()
        {
            UIManager.Instance.UnregisterModalItem(this);
        }

        public virtual void EnableInput(bool enable)
        {
            _enableInput = enable;
        }

        public List<Button> DisableButtons(Button exception)
        {
            List<Button> buttonsDisabled = new List<Button>();
            foreach (Button button in GetComponentsInChildren<Button>(true))
            {
                if (button != exception)
                {
                    button.enabled = false;
                    buttonsDisabled.Add(button);
                }
            }
            return buttonsDisabled;
        }

        public void EnableButtons(List<Button> buttons)
        {
            buttons.ForEach(b => b.enabled = true);
        }

        public void Destroy()
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }
}
