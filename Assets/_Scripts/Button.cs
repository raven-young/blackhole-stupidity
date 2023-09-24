using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using BlackHole;

public class Button : MonoBehaviour
{
    public static event Action<bool> BuyComplete;

    [SerializeField] private AudioClip _selectedClip, _pressedClip, _failedClip;
    //private AudioSource _audioSource;
    private Vector3 _baseScale;

    private void Awake()
    {
        //_audioSource = GetComponent<AudioSource>();
        _baseScale = gameObject.transform.localScale;
    }

    private void OnEnable()
    {
        // Reset to base scale in case button was deactivated while selected
        DecreaseScale();
    }

    public void PlayPressed(bool success = true)
    {
        if (success)
        {
            SoundManager.Instance.PlayButtonPress(failed: false);
            //SoundManager.Instance.PlaySound(_pressedClip);
            //_audioSource.PlayOneShot(_pressedClip);
        }
        else
        {
            //_audioSource.PlayOneShot(_failedClip);
            //SoundManager.Instance.PlaySound(_failedClip);
            SoundManager.Instance.PlayButtonPress(failed: true);
        }
    }

    public void PlaySelected()
    {
        //_audioSource.PlayOneShot(_selectedClip);
        SoundManager.Instance.PlayButtonSelect();
    }

    public void IncreaseScale()
    {
        gameObject.transform.localScale = 1.1f * _baseScale;
    }

    public void DecreaseScale()
    {
        gameObject.transform.localScale = _baseScale;
    }

    public void OnBuyComplete(bool doBuy)
    {
        BuyComplete?.Invoke(doBuy);
    }
}
