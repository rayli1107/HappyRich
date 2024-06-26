﻿using System.Collections.Generic;
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

        public void Destroy()
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }
}
