using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlackHole {
    public class Shield : MonoBehaviour
    {
        public static Shield Instance;
        public bool ShieldActive = true;

        private SpriteRenderer _spriteRenderer;
        private float _regenPeriod = 10f;
        private float _regenTimer = 0;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
            }

            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void OnEnable()
        {
            if (!SettingsManager.ShieldEnabled)
            {
                gameObject.SetActive(false);
            }
        }

        void Update()
        {
            if (!ShieldActive)
            {
                _regenTimer += Time.deltaTime;
                if (_regenTimer > _regenPeriod)
                {
                    _regenTimer = 0;
                    EnableShield();
                }
            }
        }

        public void EnableShield()
        {
            _spriteRenderer.enabled = true;
            ShieldActive = true;
        }

        public void DisableShield()
        {
            _spriteRenderer.enabled = false;
            ShieldActive = false;
        }
    }
}