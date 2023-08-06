using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStats", menuName = "ScriptableObject/PlayerStats")]
public class PlayerStats : ScriptableObject
{

    //public class Stats
    //{
    //    public Stats(int highscore, int maxcombo)
    //    {
    //        Highscore = highscore;
    //        Maxcombo = maxcombo;
    //    }

    //    public int Highscore;
    //    public int Maxcombo;
    //}

    //// unity does not support serializing dictionaries
    //// there must be a better way
    //[SerializeField] public Dictionary<SettingsManager.DifficultySetting, 
    //    Dictionary<SettingsManager.ShipType, Stats>> StatsDict = new();

    //public void ResetStatsDict()
    //{
    //    Debug.Log("resetting stats dict");

    //    foreach (int diff in Enum.GetValues(typeof(SettingsManager.DifficultySetting)))
    //    {
    //        StatsDict[(SettingsManager.DifficultySetting)diff] = new Dictionary<SettingsManager.ShipType, Stats>();
    //        foreach (int ship in Enum.GetValues(typeof(SettingsManager.ShipType)))
    //        {
    //            StatsDict[(SettingsManager.DifficultySetting)diff]
    //                [(SettingsManager.ShipType)ship] = new Stats(0, 0);
    //        }
    //    }
    //}

    //public int newGetHighscore()
    //{
    //    if (StatsDict.Count < 1)
    //        ResetStatsDict();

    //    return StatsDict[SettingsManager.Instance.SelectedDifficulty][SettingsManager.Instance.SelectedShipType].Highscore;
    //}

    //public void newSetHighscore(int newscore)
    //{
    //    if (StatsDict.Count < 1)
    //        ResetStatsDict();

    //    StatsDict[SettingsManager.Instance.SelectedDifficulty][SettingsManager.Instance.SelectedShipType].Highscore = newscore;
    //}

    // old below

    [Header("Highscores")]
    public int HighScoreEasy = 0;
    public int HighScoreNormal = 0;
    public int HighScoreHard = 0;
    public int HighScoreExpert = 0;

    public void SetHighscore(int newScore)
    {
        switch (SettingsManager.Instance.SelectedDifficulty)
        {
            case SettingsManager.DifficultySetting.Easy: HighScoreEasy = newScore; break;
            case SettingsManager.DifficultySetting.Normal: HighScoreNormal = newScore; break;
            case SettingsManager.DifficultySetting.Hard: HighScoreHard = newScore; break;
            case SettingsManager.DifficultySetting.Expert: HighScoreExpert = newScore; break;
        }
    }

    public int GetHighscore()
    {
        switch (SettingsManager.Instance.SelectedDifficulty)
        {
            case SettingsManager.DifficultySetting.Easy: return HighScoreEasy;
            case SettingsManager.DifficultySetting.Normal: return HighScoreNormal;
            case SettingsManager.DifficultySetting.Hard: return HighScoreHard;
            case SettingsManager.DifficultySetting.Expert: return HighScoreExpert;
            default: Debug.LogWarning("Invalid difficulty"); return 0;
        }
    }

    //[Header("MaxCombos")]
    //public int MaxComboEasy = 0;
    //public int MaxComboNormal = 0;
    //public int MaxComboHard = 0;
    //public int MaxComboExpert = 0;

    //[Header("MaxAccuracy")]
    //public float MaxAccuracyEasy = 0;
    //public float MaxAccuracyNormal = 0;
    //public float MaxAccuracyHard = 0;
    //public float MaxAccuracyExpert = 0;

    //[Header("Misc.")]
    //public int VicotriesEasy = 0;
    //public int VicotriesNormal = 0;
    //public int VicotriesHard = 0;
    //public int VicotriesExpert = 0;

    //public int DefeatsEasy = 0;
    //public int DefeatsNormal = 0;
    //public int DefeatsHard = 0;
    //public int DefeatsExpert = 0;
}
