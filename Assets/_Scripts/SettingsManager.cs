using UnityEngine;
using UnityEngine.SceneManagement;

namespace BlackHole
{
    public class SettingsManager : MonoBehaviour
    {
        public static SettingsManager Instance;
        [SerializeField] private GameParams _gameParams;
        public DifficultySetting SelectedDifficulty;
        public ShipType SelectedShipType;

        public static bool IsMobileGame = false;

        // GAME PARAMETERS
        public static int BulletDamage;
        public static float BurnRate;
        public static float FirePeriod;
        public static float MagnetScale;
        public static bool TripleShotEnabled = false;
        public static bool ShieldEnabled = false;

        public static float DifficultyScoreMultiplier;

        public enum DifficultySetting
        {
            Easy = 0,
            Normal = 1,
            Hard = 2,
            Expert = 3
        }

        public enum ShipType
        {
            Basic = 0, // balanced
            Collector = 1, // bigger item magnet
            Destroyer = 2, // more firepower
            Tank = 3, // more HP NOT YET USED
            Scorer = 4 // more points NOT YET USED
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
                return;
            }

            ToggleMobile(Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer);

            ResetGameParams();
        }

        private void Start()
        {
            // Needed when directly starting game in edit mode (in real scenario, set by player in main menu)
            if (SceneManager.GetActiveScene().name == "BlackHole")
            {
                SelectedDifficulty = DifficultySetting.Normal;
                SelectedShipType = ShipType.Basic;
                PrepareGame();
            }
        }

        public void ResetGameParams()
        {
            BulletDamage = _gameParams.BulletDamage;
            BurnRate = _gameParams.FuelBurnRate;
            FirePeriod = _gameParams.FirePeriod;
            MagnetScale = _gameParams.MagnetScale;
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
            //Debug.Log("Preparing game with difficulty " + SelectedDifficulty);

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
    }
}