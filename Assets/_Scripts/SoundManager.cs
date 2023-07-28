using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    [SerializeField] private AudioSource _musicSource, _musicSource2, _effectsSource;
    [SerializeField] private AudioClip _backgroundMusic, _nervousMusic, _mainMenuMusic;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

    }

    private void Start()
    {
        PlayMusic(_mainMenuMusic);
    }

    private void OnEnable()
    {
        GameManager.OnEnteredDangerZone += SwapBG;
        GameManager.OnExitedDangerZone += SwapBG;
    }

    private void OnDisable()
    {
        GameManager.OnEnteredDangerZone -= SwapBG;
        GameManager.OnExitedDangerZone -= SwapBG;
    }

    public void ChangeToBG()
    {
        _musicSource.Stop();
        _musicSource.PlayOneShot(_backgroundMusic);
        _musicSource2.PlayOneShot(_nervousMusic);
        _musicSource2.DOFade(0, 0);
    }

    public void PlaySound(AudioClip clip, float volumeScale = 1f)
    {
        _effectsSource.PlayOneShot(clip, volumeScale);
    }

    public void PlayMusic(AudioClip clip, float volumeScale = 1f)
    {
        _musicSource.PlayOneShot(clip, volumeScale);
    }
    public void ChangeSFXVolume(float volume)
    {
        _effectsSource.volume = volume;
    }
    public void ChangeMusicVolume(float volume, float fadeDuration = 0f)
    {
        _musicSource.DOFade(volume, fadeDuration);
        _musicSource2.DOFade(volume, fadeDuration);
    }
    public void ChangeMasterVolume(float volume)
    {
        AudioListener.volume = volume;
    }

    public void SwapBG()
    {
        float fadeDuration = 1f;

        if (GameManager.Instance.InDangerZone)
        {
            _musicSource.DOFade(0, fadeDuration);
            _musicSource2.DOFade(1, fadeDuration);
        }
        else
        {
            _musicSource.DOFade(1, fadeDuration);
            _musicSource2.DOFade(0, fadeDuration);
        }
    }

    public IEnumerator ChangeMusic(AudioClip clip, float fadeoutDuration = 0f, float fadeinDuration = 0f, float offset = 0f)
    {
        _musicSource.DOFade(0, fadeoutDuration);
        //DOTween.To(() => _musicSource.volume, x => _musicSource.volume = x, 0, fadeoutDuration);
        yield return new WaitForSeconds(fadeoutDuration);
        yield return new WaitForSeconds(offset);
        _musicSource.Stop();
        _musicSource.PlayOneShot(clip);
        _musicSource.DOFade(1, fadeinDuration);
        yield return new WaitForSeconds(fadeinDuration);
    }
}