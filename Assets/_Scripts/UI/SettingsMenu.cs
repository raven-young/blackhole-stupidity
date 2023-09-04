using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlackHole
{
    public class SettingsMenu : Menu<SettingsMenu>
    {

        public void OnMusicVolumeChanged(float volume)
        {
            SoundManager.Instance.ChangeMusicPairVolume(volume);
        }

        public void OnSFXVolumeChanged(float volume)
        {
            SoundManager.Instance.ChangeSFXVolume(volume);
        }

        public void OnToggleAutoshoot(bool toggle)
        {
            Shooting.IsAutoshooting = toggle;
        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();
        }
    }
}