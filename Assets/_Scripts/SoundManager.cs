using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [SerializeField] private AudioSource _musicSource1, _musicSource2, _effectsSource;
    [SerializeField] private AudioClip _backgroundMusic, _nervousMusic, _mainMenuMusic, _dialogueMusic;

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
        StartMusicPair(_mainMenuMusic, _dialogueMusic);
    }

    private void OnEnable()
    {
        GameManager.OnEnteredDangerZone += DangerZoneCrossSwapMusic;
        GameManager.OnExitedDangerZone += DangerZoneCrossSwapMusic;
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
        _musicSource1.DOFade(volume, fadeDuration);
        _musicSource2.DOFade(volume, fadeDuration);
    }

    public void ChangeMasterVolume(float volume)
    {
        AudioListener.volume = volume;
    }

    // Loop two music clips simultaneously, can then dynamiaclly switch between them
    private void StartMusicPair(AudioClip clip1, AudioClip clip2, float fadeinDuration = 0f)
    {
        Debug.Log("start music pair");
        _musicSource1.clip = clip1;
        _musicSource2.clip = clip2;
        _musicSource1.Play();
        _musicSource2.Play();
        _musicSource1.DOFade(1, fadeinDuration).SetUpdate(true); // set update to make independent of time scale
        _musicSource2.DOFade(0, 0).SetUpdate(true);
        StartCoroutine(dummy());
    }
    IEnumerator dummy ()
    {
        yield return new WaitForSecondsRealtime(1.5f);
        Debug.Log(_musicSource1.volume);
        Debug.Log(_musicSource2.volume);
    }
    public void ChangeMusicPairSource(MusicSourceID oldMusicSourceNumber, float fadeDuration = 0f)
    {

        if (oldMusicSourceNumber == MusicSourceID.MusicSource1)
        {
            _musicSource1.DOFade(0, fadeDuration);
            _musicSource2.DOFade(1, fadeDuration);
        } else if (oldMusicSourceNumber == MusicSourceID.MusicSource2)
        {
            _musicSource1.DOFade(1, fadeDuration);
            _musicSource2.DOFade(0, fadeDuration);
        }
    }

    public void StartMainGameMusic(float fadeinDuration = 0f)
    {
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
}