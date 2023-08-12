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

        // GAME PARAMETERS
        public static int BulletDamage;
        public static float BurnRate;
        public static float FirePeriod;
        public static float MagnetScale;

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

        // Calculate game params based on difficulty and ship selected
        public void CalculateGameParams()
        {

            // BULLET DAMAGE
            BulletDamage = _gameParams.BulletDamage;
            switch (SelectedShipType)
            {
                case ShipType.Destroyer: BulletDamage += _gameParams.DestroyerBulletDamageBonus; break;
            }

            // BURN RATE
            BurnRate = _gameParams.FuelBurnRate;
            if (SelectedDifficulty < DifficultySetting.Hard)
                BurnRate = 0;
            switch (SelectedShipType)
            {
                case ShipType.Basic: BurnRate /= _gameParams.BasicShipMultiplier; break;
            }

            // FIRE RATE
            FirePeriod = _gameParams.FirePeriod;
            switch (SelectedShipType)
            {
                case ShipType.Collector: FirePeriod *= _gameParams.CollectorFirePeriodMultiplier; break;
            }

            // MAGNET SCALE
            MagnetScale = _gameParams.MagnetScale;
            switch (SelectedShipType)
            {
                case ShipType.Collector: MagnetScale *= _gameParams.CollectorMagnetScaleMultiplier; break;
                case ShipType.Basic: MagnetScale *= _gameParams.BasicShipMultiplier; break;
                case ShipType.Destroyer: MagnetScale *= _gameParams.DestroyerMagnetScaleMultiplier; break;
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
    }
}