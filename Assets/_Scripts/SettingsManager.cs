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

        private static bool _isMobile = false;
        public static bool IsMobileGame { get => _isMobile; set => _isMobile = value; }

        // GAME PARAMETERS
        private static int _bulletDamage;
        public static int BulletDamage { get => _bulletDamage; set => _bulletDamage = value; }
        private static float _burnRate;
        public static float BurnRate { get => _burnRate; set => _burnRate = value; }
        private static float _firePerid;
        public static float FirePeriod { get => _firePerid; set => _firePerid = value; }
        private static float _magnetScale;
        public static float MagnetScale { get => _magnetScale; set => _magnetScale = value; }
        private static int _itemSpawnBonus;
        public static int ItemSpawnBonus { get => _itemSpawnBonus; set => _itemSpawnBonus = value; }

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
        private static float _asteroidSpeedModifier = 1f;
        public static float AsteroidSpeedModifier { get => _asteroidSpeedModifier; set => _asteroidSpeedModifier = value; }

        public static float DifficultyScoreMultiplier;

        public enum DifficultySetting
        {
            Easy = 0,
            Normal = 1,
            Hard = 2
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
            _bulletDamage = _gameParams.BulletDamage;
            _burnRate = _gameParams.FuelBurnRate;
            _firePerid = _gameParams.FirePeriod;
            _magnetScale = _gameParams.MagnetScale;
            ItemSpawnBonus = 0;
            AsteroidSpeedModifier = 1f;

            TripleShotEnabled = false;
            ShieldEnabled = false;
            ComboSaverEnabled = false;
            AsteroidSpeedModifier = 1f;
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
            }
        }

        public static void ToggleMobile(bool isMobile)
        {
            IsMobileGame = isMobile;

            if (CanvasManager.Instance != null)
            {
                CanvasManager.Instance.ToggleMobileLayout(isMobile);
            }
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