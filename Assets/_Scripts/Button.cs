using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    [SerializeField] private AudioClip _selectedClip, _pressedClip, _failedClip;
    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
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
}
