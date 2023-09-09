using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using DG.Tweening;

namespace BlackHole
{
    public class CanvasManager : MonoBehaviour
    {

        public static CanvasManager Instance;
        [SerializeField] private GameParams _gameParams;
        [SerializeField] private PlayerStats _playerStats;
        [SerializeField] private GameObject _gameOverScreen;
        [SerializeField] private GameObject _inputPopup;
        [SerializeField] private RectTransform _achievementPanel;
        [SerializeField] private GameObject _touchControls;

        [SerializeField] private TMP_Text _scoreText;
        [SerializeField] private TMP_Text _scoreTextGameOver, _highscoreTextGameOver;

        [SerializeField] private Slider _fuelSlider, _healthSlider;
        [SerializeField] private GameObject _alertIcon;
        private bool _isLowOnStats = false;

        private PlayerInput playerInput;
        private GameObject playerController;

        private Queue<AchievementsManager.Achievement> _newAchievementsQueue = new();

        [Header("Debug")]
        [SerializeField] private TMP_Text _achievementsListText;
        //[SerializeField] private AchievementsManager _achievementsScriptableObject;

        private void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(gameObject);
            else
                Instance = this;
        }
        // Start is called before the first frame update
        void Start()
        {
            _fuelSlider.maxValue = Ship.Instance.InitialFuel;
            _fuelSlider.value = Ship.Instance.InitialFuel;
            _healthSlider.maxValue = _gameParams.MaxHealth;
            _healthSlider.minValue = 0;
            _healthSlider.value = _gameParams.MaxHealth / 2;


            playerController = GameObject.Find("ShipController");
            playerInput = playerController.GetComponent<PlayerInput>();
            //playerInput.onControlsChanged += OnDeviceChange;

            UpdateAchievementsText();

            ToggleTouchControls(SettingsManager.IsMobileGame);
        }

        private void OnEnable()
        {
            AchievementsManager.OnAchievementUnlocked += QueueAchievement;
        }

        private void OnDisable()
        {
            AchievementsManager.OnAchievementUnlocked -= QueueAchievement;
        }

        public void UpdateHealth(float newValue)
        {
            _healthSlider.value = newValue;
            CheckIfLowStatsAndAlert();
        }

        public void UpdateFuel(float newValue)
        {
            _fuelSlider.value = newValue;
            CheckIfLowStatsAndAlert();
        }

        public void ToggleFuelSlider(bool sliderOn)
        {
            _fuelSlider.gameObject.SetActive(sliderOn);
        }

        private void CheckIfLowStatsAndAlert()
        {
            if (!_isLowOnStats && (_fuelSlider.value < 0.25f * _fuelSlider.maxValue || _healthSlider.value < 0.25f * _healthSlider.maxValue))
            {
                _isLowOnStats = true;
                _alertIcon.SetActive(true);
                // to do: blink icon in sync with sfx
                SoundManager.Instance.PlaySFX(SoundManager.SFX.AlertSFX);
            }
            else if (_isLowOnStats && _fuelSlider.value > 0.25f * _fuelSlider.maxValue && _healthSlider.value > 0.25f * _healthSlider.maxValue)
            {
                _isLowOnStats = false;
                _alertIcon.SetActive(false);
            }
        }

        public void ShowControlsPanel()
        {
            ScreenFader.FadeFromBlack(1f);
            _inputPopup.SetActive(true);

            if (Gamepad.current != null)
                _inputPopup.transform.Find("Gamepad").gameObject.SetActive(true);
            else
                _inputPopup.transform.Find("Keyboard").gameObject.SetActive(true);

            // doesnt work
            //if (playerInput.currentControlScheme == "Gamepad")
            //    _inputPopup.transform.Find("Gamepad").gameObject.SetActive(true);
            //else if (playerInput.currentControlScheme == "KeyboardMouse")
            //    _inputPopup.transform.Find("Keyboard").gameObject.SetActive(true);
        }

        public void StartGame()
        {
            _inputPopup.SetActive(false);
        }

        public void ToggleTouchControls(bool touchActive)
        {
            if (touchActive && GameManager.IsPaused)
                return;

            _touchControls.SetActive(touchActive && SettingsManager.IsMobileGame);
        }

        public void RenderGameOverScreen(bool victorious)
        {
            ToggleTouchControls(false);

            if (_newAchievementsQueue.Count > 0)
            {
                //StartCoroutine(DisplayAchievementNotification());
                _achievementPanel.GetComponent<AchievementNotification>().StartAchievementsDisplay();
            }
            
            PostGameScreen.Open();
            PostGameScreen.Instance.SwapPostGameState(victorious);
            Scoring.Instance.DisplayFinalScoreAndCash(victorious);
        }

        private void QueueAchievement(AchievementsManager.Achievement achievement)
        {
            _newAchievementsQueue.Enqueue(achievement);
            _achievementPanel.GetComponent<AchievementNotification>().EnqeueueAchievement(achievement);
        }

        public void UpdateAchievementsText()
        {
            // Debug
            _achievementsListText.text = AchievementsManager.Instance.GetAchievementsString();
        }
    }
}