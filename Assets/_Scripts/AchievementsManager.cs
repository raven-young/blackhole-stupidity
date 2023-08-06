using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;

[CreateAssetMenu(fileName = "AchievementsManager", menuName = "ScriptableObject/AchievementsManager")]
public class AchievementsManager : ScriptableObject
{

    // The Singleton instance
    private static AchievementsManager instance;

    // Property to access the Singleton instance
    public static AchievementsManager Instance
    {
        get
        {
            if (instance == null)
            {
                if (!ES3.KeyExists("AchievementsManager"))
                {
                    
                    instance = Resources.Load<AchievementsManager>("_ScriptableObjects/AchievementsManager");

                    // If the asset doesn't exist in Resources, create a new instance
                    if (instance == null)
                    {
                        instance = CreateInstance<AchievementsManager>();
                    }
                    ES3.Save("AchievementsManager", instance);
                    Debug.Log("saved non-existent achievements manager key: " + instance);
                } 
                else
                {
                    instance = ES3.Load<AchievementsManager>("AchievementsManager");
                    Debug.Log("loaded AM asset: " + instance);
                }
            }

            // If the instance doesn't exist, load it from Resources
            //if (instance == null)
            //{
            //    instance = Resources.Load<AchievementsManager>("_ScriptableObjects/AchievementsManager");

            //    // If the asset doesn't exist in Resources, create a new instance
            //    if (instance == null)
            //    {
            //        instance = CreateInstance<AchievementsManager>();
            //    }
            //}

            return instance;
        }
    }

    public static event Action<Achievement> OnAchievementUnlocked;

    [Serializable]
    public class Achievement
    {
        public Achievement(bool unlocked, string name, string description)
        {
            Unlocked = unlocked;
            Name = name;
            Description = description;
        }

        public bool Unlocked;
        public string Name;
        public string Description;
    }

    [SerializeField] private Achievement _victoryEasy;
    [SerializeField] private Achievement _victoryNormal;
    [SerializeField] private Achievement _victoryHard;
    [SerializeField] private Achievement _victoryExpert;

    [SerializeField] private Achievement _victoryNoDamageEasy;
    [SerializeField] private Achievement _victoryNoDamageNormal;
    [SerializeField] private Achievement _victoryNoDamageHard;
    [SerializeField] private Achievement _victoryNoDamageExpert;

    [SerializeField] private Achievement _victory100PercentEasy;
    [SerializeField] private Achievement _victory100PercentNormal;
    [SerializeField] private Achievement _victory100PercentHard;
    [SerializeField] private Achievement _victory100PercentExpert;

    [SerializeField] private Achievement _victoryFlawlessEasy;
    [SerializeField] private Achievement _victoryFlawlessNormal;
    [SerializeField] private Achievement _victoryFlawlessHard;
    [SerializeField] private Achievement _victoryFlawlessExpert;

    [SerializeField] private Achievement _score10k;
    [SerializeField] private Achievement _score50k;
    [SerializeField] private Achievement _score100k;

    private void OnEnable()
    {

        Debug.Log("AM asset instance: " + instance);

        GameManager.OnVictory += UnlockVictoryAchievement;
        GameManager.OnNoDamageVictory += UnlockNoDamageVictoryAchievement;
        GameManager.On100PercentVictory += Unlock100PercentVictoryAchievement;
        GameManager.OnFlawlessVictory += UnlockFlawlessAchievement;
        CanvasManager.OnScored += UnlockScoringAchievements;
        OnAchievementUnlocked += NotifyAchievementUnlocked;
    }

    private void OnDisable()
    {
        GameManager.OnVictory -= UnlockVictoryAchievement;
        GameManager.OnNoDamageVictory -= UnlockNoDamageVictoryAchievement;
        GameManager.On100PercentVictory -= Unlock100PercentVictoryAchievement;
        GameManager.OnFlawlessVictory -= UnlockFlawlessAchievement;
        CanvasManager.OnScored -= UnlockScoringAchievements;
        OnAchievementUnlocked -= NotifyAchievementUnlocked;
    }

    private void UnlockVictoryAchievement()
    {
        switch (SettingsManager.Instance.SelectedDifficulty)
        {
            case SettingsManager.DifficultySetting.Easy:
                if (_victoryEasy.Unlocked) return;
                _victoryEasy.Unlocked = true;
                OnAchievementUnlocked?.Invoke(_victoryEasy);
                break;
            case SettingsManager.DifficultySetting.Normal:
                if (_victoryNormal.Unlocked) return;
                _victoryNormal.Unlocked = true;
                OnAchievementUnlocked?.Invoke(_victoryNormal);
                break;
            case SettingsManager.DifficultySetting.Hard:
                if (_victoryHard.Unlocked) return;
                _victoryHard.Unlocked = true;
                OnAchievementUnlocked?.Invoke(_victoryHard);
                break;
            case SettingsManager.DifficultySetting.Expert:
                if (_victoryExpert.Unlocked) return;
                _victoryExpert.Unlocked = true;
                OnAchievementUnlocked?.Invoke(_victoryExpert);
                break;
        }
    }

    private void UnlockNoDamageVictoryAchievement()
    {
        switch (SettingsManager.Instance.SelectedDifficulty)
        {
            case SettingsManager.DifficultySetting.Easy:
                if (_victoryNoDamageEasy.Unlocked) return;
                _victoryNoDamageEasy.Unlocked = true;
                OnAchievementUnlocked?.Invoke(_victoryNoDamageEasy);
                break;
            case SettingsManager.DifficultySetting.Normal:
                if (_victoryNoDamageNormal.Unlocked) return;
                _victoryNoDamageNormal.Unlocked = true;
                OnAchievementUnlocked?.Invoke(_victoryNoDamageNormal);
                break;
            case SettingsManager.DifficultySetting.Hard:
                if (_victoryNoDamageHard.Unlocked) return;
                _victoryNoDamageHard.Unlocked = true;
                OnAchievementUnlocked?.Invoke(_victoryNoDamageHard);
                break;
            case SettingsManager.DifficultySetting.Expert:
                if (_victoryNoDamageExpert.Unlocked) return;
                _victoryNoDamageExpert.Unlocked = true;
                OnAchievementUnlocked?.Invoke(_victoryNoDamageExpert);
                break;
        }
    }

    private void Unlock100PercentVictoryAchievement()
    {
        switch (SettingsManager.Instance.SelectedDifficulty)
        {
            case SettingsManager.DifficultySetting.Easy:
                if (_victory100PercentEasy.Unlocked) return;
                _victory100PercentEasy.Unlocked = true;
                OnAchievementUnlocked?.Invoke(_victory100PercentEasy);
                break;
            case SettingsManager.DifficultySetting.Normal:
                if (_victory100PercentNormal.Unlocked) return;
                _victory100PercentNormal.Unlocked = true;
                OnAchievementUnlocked?.Invoke(_victory100PercentNormal);
                break;
            case SettingsManager.DifficultySetting.Hard:
                if (_victory100PercentHard.Unlocked) return;
                _victory100PercentHard.Unlocked = true;
                OnAchievementUnlocked?.Invoke(_victory100PercentHard);
                break;
            case SettingsManager.DifficultySetting.Expert:
                if (_victory100PercentExpert.Unlocked) return;
                _victory100PercentExpert.Unlocked = true;
                OnAchievementUnlocked?.Invoke(_victory100PercentExpert);
                break;
        }
    }
    private void UnlockFlawlessAchievement()
    {
        switch (SettingsManager.Instance.SelectedDifficulty)
        {
            case SettingsManager.DifficultySetting.Easy:
                if (_victoryFlawlessEasy.Unlocked) return;
                _victoryFlawlessEasy.Unlocked = true;
                OnAchievementUnlocked?.Invoke(_victoryFlawlessEasy);
                break;
            case SettingsManager.DifficultySetting.Normal:
                if (_victoryFlawlessNormal.Unlocked) return;
                _victoryFlawlessNormal.Unlocked = true;
                OnAchievementUnlocked?.Invoke(_victoryFlawlessNormal);
                break;
            case SettingsManager.DifficultySetting.Hard:
                if (_victoryFlawlessHard.Unlocked) return;
                _victoryFlawlessHard.Unlocked = true;
                OnAchievementUnlocked?.Invoke(_victoryFlawlessHard);
                break;
            case SettingsManager.DifficultySetting.Expert:
                if (_victoryFlawlessExpert.Unlocked) return;
                _victoryFlawlessExpert.Unlocked = true;
                OnAchievementUnlocked?.Invoke(_victoryFlawlessExpert);
                break;
        }
    }

    private void UnlockScoringAchievements(int score)
    {
        if (score > 10000 && !_score10k.Unlocked)
        {
            _score10k.Unlocked = true;
            OnAchievementUnlocked?.Invoke(_score10k);
        }
        if (score > 50000 && !_score50k.Unlocked)
        {
            _score50k.Unlocked = true;
            OnAchievementUnlocked?.Invoke(_score50k);
        }
        if (score > 100000 && !_score100k.Unlocked)
        {
            _score100k.Unlocked = true;
            OnAchievementUnlocked?.Invoke(_score100k);
        }
    }

    public void NotifyAchievementUnlocked(Achievement achievement)
    {
        Debug.Log("Achievement unlocked: " + achievement.Name);
    }

    public string GetAchievementsString()
    {
        string outstring = "";
        List<object> achievements = new(GetNestedFieldValuesOfType<Achievement>());

        foreach (Achievement field in achievements)
        {
            outstring += field.Name + ": " + field.Unlocked + "\n";
        }
        return outstring;
    }

    public void ResetAchievements()
    {
        List<object> achievements = new(GetNestedFieldValuesOfType<Achievement>());

        foreach (Achievement field in achievements)
        {
            field.Unlocked = false;
        }
    }

    // code below from chatgpt
    public List<object> GetNestedFieldValuesOfType<T>()
    {
        List<object> values = new List<object>();
        Type type = typeof(T);

        // Use BindingFlags to include both public and non-public instance fields
        FieldInfo[] fields = this.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

        foreach (FieldInfo field in fields)
        {
            if (field.FieldType == type)
            {
                object fieldValue = field.GetValue(this);
                values.Add(fieldValue);
            }
        }

        return values;
    }

    public void SaveAchievements()
    {
        ES3.Save("AchievementsManager", Instance);
    }
}
