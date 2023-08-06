using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;

[CreateAssetMenu(fileName = "AchievementsManager", menuName = "ScriptableObject/AchievementsManager")]
public class AchievementsManager : ScriptableObject
{
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

    [SerializeField] private Achievement _score420; // testing
    [SerializeField] private Achievement _score10k;
    [SerializeField] private Achievement _score50k;

    private void OnEnable()
    {
        GameManager.OnVictory += UnlockVictoryAchievement;
        CanvasManager.OnScored += UnlockScoringAchievements;
        OnAchievementUnlocked += NotifyAchievementUnlocked;
    }

    private void OnDisable()
    {
        GameManager.OnVictory -= UnlockVictoryAchievement;
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

    private void UnlockScoringAchievements(int score)
    {
        if (score > 1 && !_score420.Unlocked)
        {
            _score420.Unlocked = true;
            OnAchievementUnlocked?.Invoke(_score420);
        }
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
    }

    public void NotifyAchievementUnlocked(Achievement achievement)
    {
        Debug.Log("Achievement unlocked: " + achievement.Name);
        //PrintAchievements();
    }

    public void PrintAchievements()
    {

        List<object> achievements = new(GetNestedFieldValuesOfType<Achievement>());

        foreach (Achievement field in achievements)
        {
            if (field.Unlocked)
                Debug.Log("Unlocked: " + field.Name);
        }

        
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
}
