using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Panels.Templates
{
    public class TimedModalObject : ModalObject
    {
#pragma warning disable 0649
        [SerializeField]
        private float _waitDuration = 1f;
        [SerializeField]
        private float _fadeDuration = 0.5f;
        [SerializeField]
        private float _deltaY = 100f;
#pragma warning restore 0649

        public Action callback;

        public string text
        {
            set
            {
                _textObject.text = value;
            }
        }

        public Color color
        {
            set
            {
                _textObject.color = value;
            }
        }

        private TextMeshProUGUI _textObject;

        private void Awake()
        {
            _textObject = GetComponentInChildren<TextMeshProUGUI>(true);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            StartCoroutine(fadeOutAnimate());
        }

        private IEnumerator fadeOutAnimate()
        {
            yield return new WaitForSeconds(_waitDuration);

            float timeStart = Time.time;
            float yStart = _textObject.transform.position.y;
            float aStart = _textObject.color.a;

            while (true)
            {
                float pct = (Time.time - timeStart) / _fadeDuration;
                if (pct >= 1)
                {
                    break;
                }

                _textObject.transform.position = new Vector3(
                    _textObject.transform.position.x,
                    yStart + pct * _deltaY,
                    _textObject.transform.position.z);
                _textObject.color = new Color(
                    _textObject.color.r,
                    _textObject.color.g,
                    _textObject.color.b, aStart * (1 - pct));
                yield return null;
            }
            Destroy();
            callback?.Invoke();
        }
    }
}
