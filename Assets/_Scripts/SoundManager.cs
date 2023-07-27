using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    [SerializeField] private AudioSource _musicSource, _effectsSource;
    [SerializeField] private AudioClip _backgroundMusic;

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
        PlayMusic(_backgroundMusic);
    }

    public void PlaySound(AudioClip clip)
    {
        _effectsSource.PlayOneShot(clip);
    }

    public void PlayMusic(AudioClip clip)
    {
        _musicSource.PlayOneShot(clip);
    }
    public void ChangeSFXVolume(float volume)
    {
        _effectsSource.volume = volume;
    }
    public void ChangeMusicVolume(float volume)
    {
        _musicSource.volume = volume;
    }
    public void ChangeMasterVolume(float volume)
    {
        AudioListener.volume = volume;
    }

    //public IEnumerator ChangeMusic(AudioClip clip, float fadeoutDuration = 0f, float fadeinDuration = 0f, float offset = 0f)
    //{
    //    _musicSource.DOFade(0, fadeoutDuration);
    //    //DOTween.To(() => _musicSource.volume, x => _musicSource.volume = x, 0, fadeoutDuration);
    //    yield return new WaitForSeconds(fadeoutDuration);
    //    yield return new WaitForSeconds(offset);
    //    _musicSource.Stop();
    //    _musicSource.PlayOneShot(clip);
    //    _musicSource.DOFade(1, fadeinDuration);
    //    yield return new WaitForSeconds(fadeinDuration);
    //}
}