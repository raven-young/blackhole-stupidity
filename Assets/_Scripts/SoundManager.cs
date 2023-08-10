using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

// by @kurtdekker - to make a Unity singleton that has some
// prefab-stored, data associated with it, eg a music manager
//
// To use: access with SingletonViaPrefab.Instance
//
// To set up:
//	- Copy this file (duplicate it)
//	- rename class SingletonViaPrefab to your own classname
//	- rename CS file too
//	- create the prefab asset associated with this singleton
//		NOTE: read docs on Resources.Load() for where it must exist!!
//
// DO NOT DRAG THE PREFAB INTO A SCENE! THIS CODE AUTO-INSTANTIATES IT!
//
// I do not recommend subclassing unless you really know what you're doing.

public class SoundManager : MonoBehaviour
{
    // This is really the only blurb of code you need to implement a Unity singleton
    private static SoundManager _Instance;
    public static SoundManager Instance
    {
        get
        {
            if (!_Instance)
            {
                // NOTE: read docs to see directory requirements for Resources.Load!
                var prefab = Resources.Load<GameObject>("_Prefabs/SoundManager");
                // create the prefab in your scene
                var inScene = Instantiate<GameObject>(prefab);
                // try find the instance inside the prefab
                _Instance = inScene.GetComponentInChildren<SoundManager>();
                // guess there isn't one, add one
                if (!_Instance) _Instance = inScene.AddComponent<SoundManager>();
                // mark root as DontDestroyOnLoad();
                DontDestroyOnLoad(_Instance.transform.root.gameObject);
            }
            return _Instance;
        }
    }

    // NOTE: alternatively to a prefab, you could use a ScriptableObject derived asset,
    // make a reference to it here, and populated that reference at the Resources.Load
    // line above.

    // implement your Awake, Start, Update, or other methods here... (optional)
    
    [SerializeField] private AudioSource _musicSource1, _musicSource2, _effectsSource;

    [Header("Music")]
    [SerializeField] private AudioClip _backgroundMusic;
    [SerializeField] private AudioClip _nervousMusic, _mainMenuMusic, _dialogueMusic;

    [Header("SFX")]
    [SerializeField] private AudioClip _alertClip;
    [SerializeField] private AudioClip _victoryFanfare, _powerup, _shipTakeoff;

    [Header("UI")]
    [SerializeField] private AudioClip _buttonPress;
    [SerializeField] private AudioClip _failedButtonPress, _buttonSelect;

    private void OnEnable()
    {
        GameManager.OnEnteredDangerZone += DangerZoneCrossSwapMusic;
        GameManager.OnExitedDangerZone += DangerZoneCrossSwapMusic;
        _effectsSource.enabled = true;
    }

    private void OnDisable()
    {
        GameManager.OnEnteredDangerZone -= DangerZoneCrossSwapMusic;
        GameManager.OnExitedDangerZone -= DangerZoneCrossSwapMusic;
    }

    public enum MusicSourceID
    {
        MusicSource1,
        MusicSource2
    }

    public void PlaySound(AudioClip clip, float volumeScale = 1f)
    {
        _effectsSource.PlayOneShot(clip, volumeScale);
    }

    public void PlayMusic(AudioClip clip, float volumeScale = 1f)
    {
        _musicSource1.PlayOneShot(clip, volumeScale);
    }
    public void ChangeSFXVolume(float volume)
    {
        _effectsSource.volume = volume;
    }
    public void ChangeMusicVolume(float volume, float fadeDuration = 0f)
    {
        _musicSource1.DOFade(volume, fadeDuration).SetUpdate(true);
        _musicSource2.DOFade(volume, fadeDuration).SetUpdate(true);
    }

    public void StopSFX()
    {
        _effectsSource.Stop();
    }
    public void StopMusic()
    {
        _musicSource1.Stop();
        _musicSource2.Stop();
    }
    public void ChangeMasterVolume(float volume)
    {
        AudioListener.volume = volume;
    }

    // Loop two music clips simultaneously, can then dynamiaclly switch between them
    private void StartMusicPair(AudioClip clip1, AudioClip clip2, float fadeinDuration = 0f)
    {
        _musicSource1.clip = clip1;
        _musicSource2.clip = clip2;
        _musicSource1.Play();
        _musicSource2.Play();
        _musicSource1.DOFade(1, fadeinDuration).SetUpdate(true); // set update to make independent of time scale
        _musicSource2.DOFade(0, 0).SetUpdate(true);
    }

    public void ChangeMusicPairSource(MusicSourceID oldMusicSourceNumber, float fadeDuration = 0f)
    {

        if (oldMusicSourceNumber == MusicSourceID.MusicSource1)
        {
            _musicSource1.DOFade(0, fadeDuration).SetUpdate(true);
            _musicSource2.DOFade(1, fadeDuration).SetUpdate(true);
        } else if (oldMusicSourceNumber == MusicSourceID.MusicSource2)
        {
            _musicSource1.DOFade(1, fadeDuration).SetUpdate(true);
            _musicSource2.DOFade(0, fadeDuration).SetUpdate(true);
        }
    }

    public void ChangeMusicSourcePitch(float newPitch, float fadeDuration = 0f)
    {
        _musicSource1.DOPitch(newPitch, fadeDuration);
        _musicSource2.DOPitch(newPitch, fadeDuration);
    }

    public void StartMainMenuMusic()
    {
        StopMusic();
        StopSFX();
        StartMusicPair(_mainMenuMusic, _dialogueMusic);
    }
    public void StartMainGameMusic(float fadeinDuration = 0f)
    {
        StopMusic();
        StopSFX();
        StartMusicPair(_backgroundMusic, _nervousMusic, fadeinDuration);
    }

    public void DangerZoneCrossSwapMusic()
    {
        float fadeDuration = 1f;

        if (GameManager.Instance.InDangerZone)
        {
            ChangeMusicPairSource(MusicSourceID.MusicSource1, fadeDuration);
        }
        else
        {
            ChangeMusicPairSource(MusicSourceID.MusicSource2, fadeDuration);
        }
    }

    public enum SFX
    {
        AlertSFX,
        ButtonPress,
        VictoryFanfare,
        Powerup,
        ShipTakeoff
    }
    public void PlaySFX(SFX sfx) 
    {
        switch (sfx)
        {
            case SFX.AlertSFX: PlaySound(_alertClip); break;
            case SFX.ButtonPress: PlaySound(_buttonPress); break;
            case SFX.VictoryFanfare: PlaySound(_victoryFanfare); break;
            case SFX.Powerup: PlaySound(_powerup); break;
            case SFX.ShipTakeoff: PlaySound(_shipTakeoff); break;
        }
        
    }

    public void PlayButtonPress(bool failed = false)
    {
        if (failed)
            PlaySound(_failedButtonPress);
        else
            PlaySound(_buttonPress);
    }

    public void PlayButtonSelect()
    {
        PlaySound(_buttonSelect);
    }
}