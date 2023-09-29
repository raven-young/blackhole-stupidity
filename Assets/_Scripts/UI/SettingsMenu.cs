using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace BlackHole
{
    public class SettingsMenu : Menu<SettingsMenu>
    {
        public static event Action OnControlSettingsChanged;

        [SerializeField] private Slider _musicVolumeSlider;
        [SerializeField] private Slider _sfxVolumeSlider;
        [SerializeField] private Toggle _mobileStickToggle;
        [SerializeField] private Toggle _autoshootToggle;

        private Image _backgroundImage;
        private float _musicVolume, _sfxVolume;
        private bool _isAutoshooting, _mobileStickEnabled;
        public bool MobileStickEnabled { get => _mobileStickEnabled; set => _mobileStickEnabled = value; }
        private bool _firstTimeEnabled = true;

        protected override void Awake()
        {
            base.Awake();
            LoadSettings();
            _backgroundImage = GetComponent<Image>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            _backgroundImage.enabled = SceneManager.GetActiveScene().name != "MainMenu";

            // Guarantee that SettingsManager is enabled
            if (_firstTimeEnabled)
            {
                _firstTimeEnabled = false;
            }
            else 
            {
                _mobileStickToggle.gameObject.SetActive(SettingsManager.IsMobileGame);
            }
        }

        public void OnMusicVolumeChanged(float volume)
        {
            SoundManager.Instance.ChangeMusicPairVolume(volume);
            _musicVolume = volume;
        }

        public void OnSFXVolumeChanged(float volume)
        {
            SoundManager.Instance.ChangeSFXVolume(volume);
            _sfxVolume = volume;
        }

        public void OnToggleAutoshoot(bool toggle)
        {
            Shooting.IsAutoshooting = toggle;
            _isAutoshooting = toggle;
            _autoshootToggle.isOn = toggle;
        }

        public void OnToggleMobileStick(bool toggle)
        {
            _mobileStickToggle.isOn = toggle;
            if (toggle)
            {
                MobileStickEnabled = true;
                Ship.ActiveControls = Ship.ControlSetting.MobileStick;
            }
            else
            {
                MobileStickEnabled = false;
                Ship.ActiveControls = Ship.ControlSetting.TouchDrag;
            }
            OnControlSettingsChanged?.Invoke();
        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();
            SaveSettings();
        }

        public void SaveSettings()
        {
            ES3.Save("MusicVolume", _musicVolume);
            ES3.Save("SFXVolume", _sfxVolume);
            ES3.Save("IsAutoshooting", _isAutoshooting);
            ES3.Save("MobileStickEnabled", MobileStickEnabled);
        }

        public void LoadSettings()
        {
            _musicVolumeSlider.value = ES3.Load<float>("MusicVolume", 1f);
            _sfxVolumeSlider.value = ES3.Load<float>("SFXVolume", 1f);
            OnToggleAutoshoot(ES3.Load<bool>("IsAutoshooting", true));
            OnToggleMobileStick(ES3.Load<bool>("MobileStickEnabled", false));
        }
    }
}