using UnityEngine;

namespace BlackHole
{
    [CreateAssetMenu(fileName = "PlayerStats", menuName = "ScriptableObject/PlayerStats")]
    public class PlayerStats : ScriptableObject
    {

        // The Singleton instance
        private static PlayerStats _instance;

        // Property to access the Singleton instance
        public static PlayerStats Instance
        {
            get
            {
                if (_instance == null)
                {
                    if (!ES3.KeyExists("PlayerStats"))
                    {

                        _instance = Resources.Load<PlayerStats>("_ScriptableObjects/PlayerStats");

                        // If the asset doesn't exist in Resources, create a new instance
                        if (_instance == null)
                        {
                            _instance = CreateInstance<PlayerStats>();
                        }
                        ES3.Save("PlayerStats", _instance);
                        Debug.Log("Saved non-existent PlayerStats key: " + _instance);
                    }
                    else
                    {
                        _instance = ES3.Load<PlayerStats>("PlayerStats");
                        Debug.Log("Loaded PlayerStats asset: " + _instance);
                    }
                }
                return _instance;
            }

            set => _instance = value;
        }

        [Header("Highscores")]
        private int _highScoreEasy = 0;
        private int _highScoreNormal = 0;
        private int _highScoreHard = 0;
        private int _highScoreExpert = 0;

        public void SetHighscore(int newScore)
        {
            switch (SettingsManager.Instance.SelectedDifficulty)
            {
                case SettingsManager.DifficultySetting.Easy: _highScoreEasy = newScore; break;
                case SettingsManager.DifficultySetting.Normal: _highScoreNormal = newScore; break;
                case SettingsManager.DifficultySetting.Hard: _highScoreHard = newScore; break;
                case SettingsManager.DifficultySetting.Expert: _highScoreExpert = newScore; break;
                default: Debug.LogWarning("Invalid difficulty"); break;
            }
        }

        public int GetHighscore()
        {
            switch (SettingsManager.Instance.SelectedDifficulty)
            {
                case SettingsManager.DifficultySetting.Easy: return _highScoreEasy;
                case SettingsManager.DifficultySetting.Normal: return _highScoreNormal;
                case SettingsManager.DifficultySetting.Hard: return _highScoreHard;
                case SettingsManager.DifficultySetting.Expert: return _highScoreExpert;
                default: Debug.LogWarning("Invalid difficulty"); return 0;
            }
        }

        public int GetHighscore(SettingsManager.DifficultySetting difficulty)
        {
            switch (difficulty)
            {
                case SettingsManager.DifficultySetting.Easy: return _highScoreEasy;
                case SettingsManager.DifficultySetting.Normal: return _highScoreNormal;
                case SettingsManager.DifficultySetting.Hard: return _highScoreHard;
                case SettingsManager.DifficultySetting.Expert: return _highScoreExpert;
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
}
