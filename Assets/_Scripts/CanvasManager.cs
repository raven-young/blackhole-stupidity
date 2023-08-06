using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using DG.Tweening;

public class CanvasManager : MonoBehaviour
{

    public static CanvasManager Instance;
    [SerializeField] private GameParams _gameParams;
    [SerializeField] private PlayerStats _playerStats;
    [SerializeField] private GameObject _gameOverScreen;
    [SerializeField] private GameObject _victoryScreen;
    [SerializeField] private GameObject _pauseScreen;
    [SerializeField] private GameObject _inputPopup;

    [SerializeField] private TMP_Text _scoreText, _distanceLoseText, _distanceWinText;
    [SerializeField] private TMP_Text _scoreTextGameOver, _highscoreTextGameOver, _scoreTextVictory, _highscoreTextVictory;

    [SerializeField] private Slider _fuelSlider, _healthSlider;
    [SerializeField] private GameObject _alertIcon;
    private bool _isLowOnStats = false;

    private bool _newHighscore = false;
    private int _score = 0;
    private PlayerInput playerInput;
    private GameObject playerController;
    public int ComboCount = 0;

    public static event Action<int> OnScored;

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
    }

    
    //public void OnDeviceChange(PlayerInput playerInput)
    //{
    //    Debug.Log(playerInput.currentControlScheme);
    //        //.currentControlScheme.Equals("Gamepad");
    //}

    //private void Update()
    //{
    //    _distanceLoseText.text = "Fail:"+ Math.Round(GameManager.Instance.DistanceToEventHorizon, 2);
    //    _distanceWinText.text = "Win:" + Math.Round(_gameParams.WinRadius - Ship.Instance.ShipPositionRadius, 2);
    //}

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
        if (!_isLowOnStats && (_fuelSlider.value < 0.25f*_fuelSlider.maxValue || _healthSlider.value < 0.25f * _healthSlider.maxValue))
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

    public void IncrementScore(int amount)
    {
        _score += Mathf.Max(1, ComboCount) * amount;
        _scoreText.text = "Score: " + _score;
        OnScored?.Invoke(_score);
        if (_score > _playerStats.GetHighscore())
        {
            _playerStats.SetHighscore(_score);
            _newHighscore = true;
        }
    }

    public void ShowControlsPanel()
    {
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

    public void RenderPauseScreen()
    {
        _pauseScreen.SetActive(true);
        GameObject ResumeButton = _pauseScreen.transform.Find("Resume Button").gameObject;
        var eventSystem = EventSystem.current;
        eventSystem.SetSelectedGameObject(ResumeButton, new BaseEventData(eventSystem));
    }

    public void DisablePauseScreen()
    {
        _pauseScreen.SetActive(false);
    }

    public void RenderGameOverScreen(bool victorious)
    {
        if (victorious)
        {
            string acc = "\nSolved: " + Math.Round(100*QuestionAsteroid.Instance.GetAccuracy()) + "%";
            _scoreTextVictory.text = _newHighscore ? "New Highscore: " + _score + acc :  "Score: " + _score + acc;
            _highscoreTextVictory.text = "Highscore: " + _playerStats.GetHighscore();
            _victoryScreen.SetActive(true);
            GameObject ReplayButton = _victoryScreen.transform.Find("Replay Button").gameObject;
            var eventSystem = EventSystem.current;
            eventSystem.SetSelectedGameObject(ReplayButton, new BaseEventData(eventSystem));
        }
        else
        {
            _scoreTextGameOver.text = "Score: " + _score + "\nSolved: " + Math.Round(100*QuestionAsteroid.Instance.GetAccuracy()) + "%";
            _highscoreTextGameOver.text = "Highscore: " + _playerStats.GetHighscore();
            _gameOverScreen.SetActive(true);
            GameObject ReplayButton = _gameOverScreen.transform.Find("Replay Button").gameObject;
            var eventSystem = EventSystem.current;
            eventSystem.SetSelectedGameObject(ReplayButton, new BaseEventData(eventSystem));
        }
    }

    public void SwitchActionMap()
    {
        Debug.Log(playerInput.currentActionMap.ToString());
        if (playerInput.currentActionMap.ToString() == "PlayerInputActions (UnityEngine.InputSystem.InputActionAsset):Player")
        {
            Debug.Log("swtiching to UI");
            playerInput.SwitchCurrentActionMap("UI");
        }

        else if (playerInput.currentActionMap.ToString() == "PlayerInputActions (UnityEngine.InputSystem.InputActionAsset):UI")
        {
            Debug.Log("swtiching to player");
            playerInput.SwitchCurrentActionMap("Player");
        }
        else Debug.LogWarning("Unknown action map");
    }
}
