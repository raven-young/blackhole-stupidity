using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace BlackHole
{
    public class SettingsMenu : Menu<SettingsMenu>
    {
        [SerializeField] private Slider _musicVolumeSlider;
        [SerializeField] private Slider _sfxVolumeSlider;
        private Image _backgroundImage;

        private float _musicVolume, _sfxVolume;
        private bool _isAutoshooting;

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
        }

        public void LoadSettings()
        {
            _musicVolumeSlider.value = ES3.Load<float>("MusicVolume", 1f);
            _sfxVolumeSlider.value = ES3.Load<float>("SFXVolume", 1f);
            OnToggleAutoshoot(ES3.Load<bool>("IsAutoshooting", true));
        }
    }
}