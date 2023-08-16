using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    [SerializeField] private AudioClip _selectedClip, _pressedClip, _failedClip;
    private AudioSource _audioSource;
    private Vector3 _baseScale;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _baseScale = gameObject.transform.localScale;
    }

    public void PlayPressed(bool success = true)
    {
        if (success)
            _audioSource.PlayOneShot(_pressedClip);
        else
            _audioSource.PlayOneShot(_failedClip);
    }

    public void PlaySelected()
    {
        _audioSource.PlayOneShot(_selectedClip);
    }

    public void IncreaseScale()
    {
        gameObject.transform.localScale = 1.1f * _baseScale;
    }

    public void DecreaseScale()
    {
        gameObject.transform.localScale = _baseScale / 1.1f;
    }
}
