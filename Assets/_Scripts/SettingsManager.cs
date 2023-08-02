using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance;

    public DifficultySetting SelectedDifficulty;

    public enum DifficultySetting
    {
        Easy = 0,
        Normal = 1,
        Hard = 2,
        Expert = 3
    }

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
}
