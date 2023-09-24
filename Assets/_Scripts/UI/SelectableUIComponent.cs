using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using BlackHole;

public class SelectableUIComponent : MonoBehaviour
{

    private Vector3 _baseScale;

    private void Awake()
    {
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
}
