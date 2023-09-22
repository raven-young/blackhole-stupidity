using UnityEngine;
using UnityEngine.SceneManagement;

namespace BlackHole
{
    [CreateAssetMenu(fileName = "SettingsManager", menuName = "ScriptableObject/SettingsManager")]
    public class SettingsManager : ScriptableObject
    {
        // The Singleton instance
        private static SettingsManager _instance;

        // Property to access the Singleton instance
        public static SettingsManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    if (!ES3.KeyExists("SettingsManager"))
                    {

                        _instance = Resources.Load<SettingsManager>("_ScriptableObjects/SettingsManager");

                        // If the asset doesn't exist in Resources, create a new instance
                        if (_instance == null)
                        {
                            Debug.LogWarning("Creating fresh SettingsManager");
                            _instance = CreateInstance<SettingsManager>();
                        }
                        ES3.Save("SettingsManager", _instance);
                        Debug.Log("Saved non-existent SettingsManager key: " + _instance);
                    }
                    else
                    {
                        _instance = ES3.Load<SettingsManager>("SettingsManager");
                        Debug.Log("Loaded SettingsManager asset: " + _instance);
                    }
                }

                return _instance;
            }

            set => _instance = value;
        }

        [SerializeField] private GameParams _gameParams;
        public DifficultySetting SelectedDifficulty;

        public static bool IsMobileGame = false;

        // GAME PARAMETERS
        public static int BulletDamage;
        public static float BurnRate;
        public static float FirePeriod;
        private static float _magnetScale;
        public static float MagnetScale { get => _magnetScale; set { Debug.Log($"setting to {value}"); _magnetScale = value; } }
        public static int ItemSpawnBonus;

        public void PrintParams()
        {
            Debug.Log("Magnet scale:" + MagnetScale);
        }

        public static bool ScoreAttackUnlocked = false;
        public static bool ScoreAttackEnabled = false;

        // Upgrades
        public static bool TripleShotEnabled = false;
        public static bool ShieldEnabled = false;
        public static bool ComboSaverEnabled = false;
        public static float AsteroidSpeedModifier = 1f;

        public static float DifficultyScoreMultiplier;

        public enum DifficultySetting
        {
            Easy = 0,
            Normal = 1,
            Hard = 2,
            Expert = 3
        }

        private void OnEnable() // awake not called on reloaded scritpable objects!
        {
            if (_instance == null)
            {
                _instance = this;
            }
            else if (_instance != this)
            {
                Debug.Log("Redundant SettingsManager instance");
                Destroy(this);
            }

            ToggleMobile(Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer);

            ResetGameParams();

            // Needed when directly starting game in edit mode (in real scenario, set by player in main menu)
            if (SceneManager.GetActiveScene().name == "BlackHole")
            {
                Debug.Log("Initializing settings manager from BlackHole scene");
                SelectedDifficulty = DifficultySetting.Normal;
            }
        }

        public void ResetGameParams()
        {
            Debug.Log("pre resetting:" + MagnetScale);
            BulletDamage = _gameParams.BulletDamage;
            BurnRate = _gameParams.FuelBurnRate;
            FirePeriod = _gameParams.FirePeriod;
            _magnetScale = _gameParams.MagnetScale;
            ItemSpawnBonus = 0;
            AsteroidSpeedModifier = 1f;
            Debug.Log("post resetting:" + MagnetScale);
        }

        // Calculate game params based on difficulty and ship selected
        public void CalculateGameParams()
        { 
            // BURN RATE
            if (SelectedDifficulty < DifficultySetting.Hard)
            {
                BurnRate = 0;
            }
        }

        public void PrepareGame()
        {
            Debug.Log("Preparing game with difficulty " + SelectedDifficulty);

            CalculateGameParams();

            switch (SelectedDifficulty)
            {
                case DifficultySetting.Easy:
                    CanvasManager.Instance.ToggleFuelSlider(false);
                    DifficultyScoreMultiplier = _gameParams.EasyScoreMultiplier;
                    break;
                case DifficultySetting.Normal:
                    CanvasManager.Instance.ToggleFuelSlider(false);
                    DifficultyScoreMultiplier = _gameParams.NormalScoreMultiplier;
                    break;
                case DifficultySetting.Hard:
                    CanvasManager.Instance.ToggleFuelSlider(true);
                    DifficultyScoreMultiplier = _gameParams.HardScoreMultiplier;
                    break;
                case DifficultySetting.Expert:
                    CanvasManager.Instance.ToggleFuelSlider(true);
                    DifficultyScoreMultiplier = _gameParams.ExpertScoreMultiplier;
                    break;
            }
        }

        public static void ToggleMobile(bool isMobile)
        {
            IsMobileGame = isMobile;
        }

        public static void UnlockScoreAttack()
        {
            ScoreAttackUnlocked = true;
            ScoreAttackEnabled = true;
        }

        public static void ResetUnlocks()
        {
            ScoreAttackUnlocked = false;
            ScoreAttackEnabled = false;
        }

        public void ToggleScoreAttack(bool toggle)
        {
            if (!ScoreAttackUnlocked) { return; }
            ScoreAttackEnabled = toggle;
        }
    }
}