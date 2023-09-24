using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlackHole
{
    public class BlackHoleObject : MonoBehaviour
    {
        [SerializeField] private GameParams _gameParams;

        private float _initalForce;
        public float CurrentForce;

        public static BlackHoleObject Instance;

        [SerializeField] private GameObject _eventHorizonGlow;
        private Material _eventHorizonGlowMaterial;
        [SerializeField] private float _initialGlow = 15f;

        [SerializeField] private GameObject _blackHoleNormalLayer;
        [SerializeField] private GameObject _blackHoleNormalHardLayer;
        [SerializeField] private GameObject _blackHoleHardLayer;

        //[SerializeField] private List<GameObject> _layers;
        //[SerializeField] private float _angularVelocity = 45f;

        [SerializeField] private GameObject _eventHorizonDesktop;
        [SerializeField] private GameObject _eventHorizonMobile;

        public bool HasGrown = false; // for achievements

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
            }
            Instance = this;
        }

        //private void Update()
        //{
        //    for (int i = 0; i < _layers.Count; i++)
        //    {
        //        GameObject layer = _layers[i];
        //        layer.transform.Rotate(Vector3.forward, Time.deltaTime * (_angularVelocity + 5f*i*i));
        //    }
        //}

        public void InitializeBlackHole()
        {
            // At the start of the game, BH force and ship fuel are balanced
            _initalForce = Ship.Instance.CurrentHealth;
            CurrentForce = _initalForce;

            if (SettingsManager.IsMobileGame)
            {
                _eventHorizonMobile.SetActive(true);
                _eventHorizonDesktop.SetActive(false);
            }
            else
            {
                _eventHorizonGlowMaterial = _eventHorizonGlow.GetComponent<Renderer>().material;
                _eventHorizonGlowMaterial.SetFloat("_Glow", _initialGlow);
            }

            switch (SettingsManager.Instance.SelectedDifficulty)
            {
                case SettingsManager.DifficultySetting.Normal:
                    if (SettingsManager.IsMobileGame) break;
                    _blackHoleNormalLayer.SetActive(true);
                    _blackHoleNormalHardLayer.SetActive(true);
                    break;
                case SettingsManager.DifficultySetting.Hard:
                    if (SettingsManager.IsMobileGame) break;
                    _blackHoleHardLayer.SetActive(true);
                    _blackHoleNormalHardLayer.SetActive(true);
                    break;
                default:
                    if (SettingsManager.IsMobileGame) break;
                    _blackHoleNormalLayer.SetActive(false);
                    _blackHoleHardLayer.SetActive(false);
                    break;
            }

        }

        public void GrowBlackHole(float scaleMultiplier)
        {
            HasGrown = true;

            transform.localScale *= scaleMultiplier;
            CurrentForce *= scaleMultiplier;
            //_angularVelocity *= scaleMultiplier;

            if (!SettingsManager.IsMobileGame)
            {
                float glow = _eventHorizonGlowMaterial.GetFloat("_Glow");
                _eventHorizonGlowMaterial.SetFloat("_Glow", 1.02f * glow * scaleMultiplier);
            }
        }
    }
}
