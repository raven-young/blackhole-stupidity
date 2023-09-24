using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Assertions;

namespace BlackHole
{
    [CreateAssetMenu(fileName = "AchievementsManager", menuName = "ScriptableObject/AchievementsManager")]
    public class AchievementsManager : ScriptableObject
    {

        // The Singleton instance
        private static AchievementsManager _instance;

        // Property to access the Singleton instance
        public static AchievementsManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    if (!ES3.KeyExists("AchievementsManager"))
                    {

                        _instance = Resources.Load<AchievementsManager>("_ScriptableObjects/AchievementsManager");

                        // If the asset doesn't exist in Resources, create a new instance
                        if (_instance == null)
                        {
                            _instance = CreateInstance<AchievementsManager>();
                        }
                        ES3.Save("AchievementsManager", _instance);
                        Debug.Log("Saved non-existent AchievementsManager key: " + _instance);
                    }
                    else
                    {
                        _instance = ES3.Load<AchievementsManager>("AchievementsManager");
                        Debug.Log("Loaded AchievementsManager asset: " + _instance);
                    }
                }
                return _instance;
            }

            set => _instance = value;
        }

        public static event Action<Achievement> AchievementUnlocked;

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

        [SerializeField] private Achievement _scoreAttackUnlocked; 
        [SerializeField] private Achievement _victoryEasy;
        [SerializeField] private Achievement _victoryNormal;
        [SerializeField] private Achievement _victoryHard;

        [SerializeField] private Achievement _victoryNoDamageEasy;
        [SerializeField] private Achievement _victoryNoDamageNormal;
        [SerializeField] private Achievement _victoryNoDamageHard;

        [SerializeField] private Achievement _victory100PercentEasy;
        [SerializeField] private Achievement _victory100PercentNormal;
        [SerializeField] private Achievement _victory100PercentHard;

        [SerializeField] private Achievement _victoryFlawlessEasy;
        [SerializeField] private Achievement _victoryFlawlessNormal;
        [SerializeField] private Achievement _victoryFlawlessHard;

        [SerializeField] private Achievement _score10k;
        [SerializeField] private Achievement _score50k;
        [SerializeField] private Achievement _score100k;
        [SerializeField] private Achievement _score250k;
        [SerializeField] private Achievement _score500k;

        [SerializeField] private Achievement _unlockAllUpgrades;
        [SerializeField] private Achievement _unlockAllAchievements;

        private List<Achievement> _allAchievements;
        private List<Achievement> AllAchievements
        {
            get
            {
                //_allAchievements ??= new(this.GetNestedFieldValuesOfType<Achievement>());
                _allAchievements = new(this.GetNestedFieldValuesOfType<Achievement>()); // due to bug, for now, always refresh list
                return _allAchievements;
            }
        }

        public float UnlockedAchievementsFraction
        {
            get => AllAchievements.Count > 0 ? (float)(_unlockedAchievementsCount) / AllAchievements.Count : 0f;
            set => Mathf.Clamp(value, 0f, 1f);
        }

        private int _unlockedAchievementsCount = 0;
        public int UnlockedAchievementsCount
        {
            get
            {
                _unlockedAchievementsCount = 0;
                foreach (Achievement a in AllAchievements) { if (a.Unlocked) _unlockedAchievementsCount++; }
                return _unlockedAchievementsCount;
            }
        }

        private void OnEnable()
        {
            if (_instance == null)
            {
                _instance = this;
            }
            else if (_instance != this)
            {
                Debug.Log("Redundant AchievementsManager instance");
                Destroy(this);
            }

            GameManager.OnVictory += UnlockVictoryAchievement;
            GameManager.OnVictory += UnlockScoreAttackAchievement;
            GameManager.OnNoDamageVictory += UnlockNoDamageVictoryAchievement;
            GameManager.On100PercentVictory += Unlock100PercentVictoryAchievement;
            GameManager.OnFlawlessVictory += UnlockFlawlessAchievement;
            Scoring.OnScored += UnlockScoringAchievements;
            UpgradeManager.OnAllUpgradesUnlocked += UnlockAllUpgradesAchievement;
            AchievementUnlocked += OnAchievementUnlocked;
        }

        private void OnDisable()
        {
            GameManager.OnVictory -= UnlockVictoryAchievement;
            GameManager.OnVictory -= UnlockScoreAttackAchievement;
            GameManager.OnNoDamageVictory -= UnlockNoDamageVictoryAchievement;
            GameManager.On100PercentVictory -= Unlock100PercentVictoryAchievement;
            GameManager.OnFlawlessVictory -= UnlockFlawlessAchievement;
            Scoring.OnScored -= UnlockScoringAchievements;
            UpgradeManager.OnAllUpgradesUnlocked -= UnlockAllUpgradesAchievement;
            AchievementUnlocked -= OnAchievementUnlocked;
        }

        private void UnlockScoreAttackAchievement()
        {
            if (_scoreAttackUnlocked.Unlocked) { return; }
            _scoreAttackUnlocked.Unlocked = true;
            AchievementUnlocked?.Invoke(_scoreAttackUnlocked);
        }
        private void UnlockVictoryAchievement()
        {
            switch (SettingsManager.Instance.SelectedDifficulty)
            {
                case SettingsManager.DifficultySetting.Easy:
                    if (_victoryEasy.Unlocked) return;
                    _victoryEasy.Unlocked = true;
                    AchievementUnlocked?.Invoke(_victoryEasy);
                    break;
                case SettingsManager.DifficultySetting.Normal:
                    if (_victoryNormal.Unlocked) return;
                    _victoryNormal.Unlocked = true;
                    AchievementUnlocked?.Invoke(_victoryNormal);
                    break;
                case SettingsManager.DifficultySetting.Hard:
                    if (_victoryHard.Unlocked) return;
                    _victoryHard.Unlocked = true;
                    AchievementUnlocked?.Invoke(_victoryHard);
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
                    AchievementUnlocked?.Invoke(_victoryNoDamageEasy);
                    break;
                case SettingsManager.DifficultySetting.Normal:
                    if (_victoryNoDamageNormal.Unlocked) return;
                    _victoryNoDamageNormal.Unlocked = true;
                    AchievementUnlocked?.Invoke(_victoryNoDamageNormal);
                    break;
                case SettingsManager.DifficultySetting.Hard:
                    if (_victoryNoDamageHard.Unlocked) return;
                    _victoryNoDamageHard.Unlocked = true;
                    AchievementUnlocked?.Invoke(_victoryNoDamageHard);
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
                    AchievementUnlocked?.Invoke(_victory100PercentEasy);
                    break;
                case SettingsManager.DifficultySetting.Normal:
                    if (_victory100PercentNormal.Unlocked) return;
                    _victory100PercentNormal.Unlocked = true;
                    AchievementUnlocked?.Invoke(_victory100PercentNormal);
                    break;
                case SettingsManager.DifficultySetting.Hard:
                    if (_victory100PercentHard.Unlocked) return;
                    _victory100PercentHard.Unlocked = true;
                    AchievementUnlocked?.Invoke(_victory100PercentHard);
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
                    AchievementUnlocked?.Invoke(_victoryFlawlessEasy);
                    break;
                case SettingsManager.DifficultySetting.Normal:
                    if (_victoryFlawlessNormal.Unlocked) return;
                    _victoryFlawlessNormal.Unlocked = true;
                    AchievementUnlocked?.Invoke(_victoryFlawlessNormal);
                    break;
                case SettingsManager.DifficultySetting.Hard:
                    if (_victoryFlawlessHard.Unlocked) return;
                    _victoryFlawlessHard.Unlocked = true;
                    AchievementUnlocked?.Invoke(_victoryFlawlessHard);
                    break;
            }
        }

        private void UnlockScoringAchievements(int score)
        {
            if (score > 10000 && !_score10k.Unlocked)
            {
                _score10k.Unlocked = true;
                AchievementUnlocked?.Invoke(_score10k);
            }
            if (score > 50000 && !_score50k.Unlocked)
            {
                _score50k.Unlocked = true;
                AchievementUnlocked?.Invoke(_score50k);
            }
            if (score > 100000 && !_score100k.Unlocked)
            {
                _score100k.Unlocked = true;
                AchievementUnlocked?.Invoke(_score100k);
            }
            if (score > 250000 && !_score250k.Unlocked)
            {
                _score250k.Unlocked = true;
                AchievementUnlocked?.Invoke(_score250k);
            }
            if (score > 500000 && !_score500k.Unlocked)
            {
                _score500k.Unlocked = true;
                AchievementUnlocked?.Invoke(_score500k);
            }
        }

        private void UnlockAllUpgradesAchievement()
        {
            if (_unlockAllUpgrades.Unlocked) { return; }
            _unlockAllUpgrades.Unlocked = true;
            AchievementUnlocked?.Invoke(_unlockAllUpgrades);
        }

        private void UnLockAllAchievementsAchievement()
        {
            if (_unlockAllAchievements.Unlocked) { return; }
            _unlockAllAchievements.Unlocked = true;
            AchievementUnlocked?.Invoke(_unlockAllAchievements);
        }

        public void OnAchievementUnlocked(Achievement achievement)
        {
            Debug.Assert(AllAchievements.Count > 0);

            if (UnlockedAchievementsCount + 1 == AllAchievements.Count)
            {
                UnLockAllAchievementsAchievement();
            }
            Debug.Log("Achievement unlocked: " + achievement.Name);
        }

        public string GetAchievementsString()
        {
            string outstring = "";
            //List<object> achievements = new(this.GetNestedFieldValuesOfType<Achievement>());

            Debug.Assert(AllAchievements.Count > 0);

            foreach (Achievement field in AllAchievements)
            {
                if (field.Unlocked)
                {
                    outstring += "<color=\"green\">" + field.Name + "</color>\n";
                }
                else
                {
                    outstring += "<color=\"grey\">" + field.Name + "</color>\n";
                }
            }
            return outstring;
        }

        public void ResetAchievements()
        {
            Debug.Assert(AllAchievements.Count > 0);

            foreach (Achievement field in AllAchievements)
            {
                field.Unlocked = false;
            }

            _unlockedAchievementsCount = 0;
        }
    }
}
