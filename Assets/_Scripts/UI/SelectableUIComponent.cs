using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using BlackHole;
using UnityEngine.UI;
using DG.Tweening;

public class SelectableUIComponent : MonoBehaviour
{

    private Vector3 _baseScale;
    [SerializeField] private Image _image;

    private Color _imageBaseColor;

    private void Awake()
    {
        _baseScale = gameObject.transform.localScale;

        if (_image != null)
            _imageBaseColor = _image.color;
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
        }
        else
        {
            SoundManager.Instance.PlayButtonPress(failed: true);
        }
    }

    public void PlaySelected()
    {
        // Disable for now to prevent pressed and selected sounds playing simultaneously when opening new menu
        //SoundManager.Instance.PlayButtonSelect();
    }

    public void IncreaseScale()
    {
        //if (SettingsManager.IsMobileGame) { return; }
        gameObject.transform.localScale = 1.1f * _baseScale;
    }

    public void DecreaseScale()
    {
        gameObject.transform.localScale = _baseScale;
    }

    public void TweenImageColor()
    {
        _image.DOFade(1, 0.5f).SetLoops(-1, LoopType.Yoyo);
    }

    public void KillImageTween()
    {
        _image.color = _imageBaseColor;
        _image.DOKill();
    }


}
