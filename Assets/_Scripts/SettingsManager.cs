using UnityEngine;
using UnityEngine.SceneManagement;

namespace BlackHole
{
    [CreateAssetMenu(fileName = "SettingsManager", menuName = "ScriptableObject/SettingsManager")]
    public class SettingsManager : ScriptableObject
    {
        // The Singleton instance
        private static SettingsManager instance;

        // Property to access the Singleton instance
        public static SettingsManager Instance
        {
            get
            {
                if (instance == null)
                {
                    if (!ES3.KeyExists("SettingsManager"))
                    {

                        instance = Resources.Load<SettingsManager>("_ScriptableObjects/SettingsManager");

                        // If the asset doesn't exist in Resources, create a new instance
                        if (instance == null)
                        {
                            Debug.LogWarning("Creating fresh SettingsManager");
                            instance = CreateInstance<SettingsManager>();
                        }
                        ES3.Save("SettingsManager", instance);
                        Debug.Log("Saved non-existent SettingsManager key: " + instance);
                    }
                    else
                    {
                        instance = ES3.Load<SettingsManager>("SettingsManager");
                        Debug.Log("Loaded SettingsManager asset: " + instance);
                    }
                }

                return instance;
            }
        }

        [SerializeField] private GameParams _gameParams;
        public DifficultySetting SelectedDifficulty;
        public ShipType SelectedShipType;

        public static bool IsMobileGame = false;

        // GAME PARAMETERS
        public static int BulletDamage;
        public static float BurnRate;
        public static float FirePeriod;
        public static float MagnetScale;
        public static int ItemSpawnBonus;

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

        public enum ShipType // deprecated
        {
            Basic = 0, // balanced
            Collector = 1, // bigger item magnet
            Destroyer = 2, // more firepower
            Tank = 3, // more HP NOT YET USED
            Scorer = 4 // more points NOT YET USED
        }

        private void OnEnable() // awake not called on reloaded scritpable objects!
        {
            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
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
                SelectedShipType = ShipType.Basic;
            }
        }

        public void ResetGameParams()
        {
            Debug.Log("firerate "+_gameParams.FirePeriod);
            BulletDamage = _gameParams.BulletDamage;
            BurnRate = _gameParams.FuelBurnRate;
            FirePeriod = _gameParams.FirePeriod;
            MagnetScale = _gameParams.MagnetScale;
            ItemSpawnBonus = 0;
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