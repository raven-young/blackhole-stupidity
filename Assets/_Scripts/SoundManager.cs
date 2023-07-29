using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    [SerializeField] private AudioSource _musicSource1, _musicSource2, _effectsSource;
    [SerializeField] private AudioClip _backgroundMusic, _nervousMusic, _mainMenuMusic, _dialogueMusic;

    private AudioClip _activeClip;

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
        _musicSource1.PlayOneShot(_mainMenuMusic);
        _musicSource2.PlayOneShot(_dialogueMusic);
        _musicSource2.DOFade(0, 0);
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

    public IEnumerator ChangeToBG()
    {
        _musicSource2.DOFade(0, 3f);
        yield return new WaitForSeconds(3f);
        _musicSource1.clip = _backgroundMusic;
        _musicSource2.clip = _nervousMusic;
        _musicSource1.Play();
        _musicSource2.Play();
        _musicSource1.DOFade(1, 3f);
        _musicSource2.DOFade(0, 0);
    }

    public void ChangeToDialogueTheme()
    {
        //_musicSource.Stop();
        //_musicSource.PlayOneShot(_dialogueMusic);
        _musicSource1.DOFade(0, 3f);
        _musicSource2.DOFade(1, 3f);
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
        _musicSource1.DOFade(volume, fadeDuration);
        _musicSource2.DOFade(volume, fadeDuration);
    }

    public void ChangeActiveMusicVolume(float volume, float fadeDuration = 0f)
    {
        var source = _activeClip == _nervousMusic ? _musicSource2 : _musicSource1;
        source.DOFade(volume, fadeDuration);
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
            _musicSource1.DOFade(0, fadeDuration);
            _musicSource2.DOFade(1, fadeDuration);
            _activeClip = _nervousMusic;
        }
        else
        {
            _musicSource1.DOFade(1, fadeDuration);
            _musicSource2.DOFade(0, fadeDuration);
            _activeClip = _backgroundMusic;
        }
    }

    public IEnumerator ChangeMusic(AudioClip clip, float fadeoutDuration = 0f, float fadeinDuration = 0f, float offset = 0f)
    {
        _musicSource1.DOFade(0, fadeoutDuration);
        //DOTween.To(() => _musicSource.volume, x => _musicSource.volume = x, 0, fadeoutDuration);
        yield return new WaitForSeconds(fadeoutDuration);
        yield return new WaitForSeconds(offset);
        _musicSource1.Stop();
        _musicSource1.PlayOneShot(clip);
        _musicSource1.DOFade(1, fadeinDuration);
        yield return new WaitForSeconds(fadeinDuration);
    }
}