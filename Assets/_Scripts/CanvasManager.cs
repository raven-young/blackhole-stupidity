using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class CanvasManager : MonoBehaviour
{

    public static CanvasManager Instance;
    [SerializeField] private GameParams _gameParams;
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private GameObject victoryScreen;
    [SerializeField] private GameObject pauseScreen;
    [SerializeField] private TMP_Text _scoreText, _distanceLoseText, _distanceWinText;
    [SerializeField] private TMP_Text _scoreTextGameOver, _highscoreTextGameOver, _scoreTextVictory, _highscoreTextVictory;

    [SerializeField] private Slider _fuelSlider, _healthSlider;

    private bool _newHighscore = false;

    private int _score = 0;
    Ship player;
    private PlayerInput playerInput;
    private GameObject playerController;

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
        player = playerController.transform.Find("Ship").GetComponent<Ship>();
        playerInput = playerController.GetComponent<PlayerInput>();
    }

    private void Update()
    {
        _distanceLoseText.text = "Fail:"+ Math.Round(GameManager.Instance.DistanceToEventHorizon, 2);
        _distanceWinText.text = "Win:" + Math.Round(_gameParams.WinRadius - Ship.Instance.ShipPositionRadius, 2);
    }

    public void UpdateHealth(float newValue)
    {
        _healthSlider.value = newValue;
    }

    public void UpdateFuel(float newValue)
    {
        _fuelSlider.value = newValue;
    }

    public void IncrementScore(int amount)
    {
        _score += amount;
        _scoreText.text = "Score: " + _score;
        if (_score > _gameParams.HighScore)
        {
            _gameParams.HighScore = _score;
            _newHighscore = true;
        }
    }

    public void RenderPauseScreen()
    {
        pauseScreen.SetActive(true);
        GameObject ResumeButton = pauseScreen.transform.Find("Resume Button").gameObject;
        var eventSystem = EventSystem.current;
        Debug.Log("event",eventSystem);
        eventSystem.SetSelectedGameObject(ResumeButton, new BaseEventData(eventSystem));
    }

    public void DisablePauseScreen()
    {
        pauseScreen.SetActive(false);
    }

    public void RenderGameOverScreen(bool victorious)
    {
        if (victorious)
        {
            _scoreTextVictory.text = _newHighscore ? "New Highscore: " + _score :  "Score: " + _score;
            _highscoreTextVictory.text = "Highscore: " + _gameParams.HighScore;
            victoryScreen.SetActive(true);
            GameObject ReplayButton = victoryScreen.transform.Find("Replay Button").gameObject;
            var eventSystem = EventSystem.current;
            eventSystem.SetSelectedGameObject(ReplayButton, new BaseEventData(eventSystem));
        }
        else
        {
            _scoreTextGameOver.text = "Score: " + _score;
            _highscoreTextGameOver.text = "Highscore: " + _gameParams.HighScore;
            gameOverScreen.SetActive(true);
            GameObject ReplayButton = gameOverScreen.transform.Find("Replay Button").gameObject;
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
