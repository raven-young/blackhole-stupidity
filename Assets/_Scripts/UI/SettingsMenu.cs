using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BlackHole
{
    public class SettingsMenu : Menu<SettingsMenu>
    {
        [SerializeField] private Slider _musicVolumeSlider;
        [SerializeField] private Slider _sfxVolumeSlider;

        private float _musicVolume, _sfxVolume;
        private bool _isAutoshooting;

        protected override void Awake()
        {
            base.Awake();
            LoadSettings();
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
            _musicVolumeSlider.value = ES3.Load<float>("MusicVolume");
            _sfxVolumeSlider.value = ES3.Load<float>("SFXVolume");
            OnToggleAutoshoot(ES3.Load<bool>("IsAutoshooting"));
        }
    }
}