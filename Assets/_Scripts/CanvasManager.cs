using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;


public class CanvasManager : MonoBehaviour
{

    public static CanvasManager Instance;
    [SerializeField] private GameParams _gameParams;
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private GameObject victoryScreen;
    [SerializeField] private GameObject pauseScreen;
    [SerializeField] private TMP_Text _scoreText, _distanceLoseText, _distanceWinText, _distanceDangerZoneText;

    [SerializeField] private Slider _fuelSlider, _healthSlider;

    private int _score = 0;

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
    }

    private void Update()
    {
        _distanceLoseText.text = "Fail:"+ Math.Round(GameManager.Instance.DistanceToEventHorizon, 2);
        _distanceWinText.text = "Win:" + Math.Round(_gameParams.WinRadius - Ship.Instance.ShipPositionRadius, 2);
        _distanceDangerZoneText.text = "Danger:" + Math.Round(Ship.Instance.ShipPositionRadius-_gameParams.DangerZoneDistance, 2);
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
    }

    public void RenderPauseScreen()
    {
        pauseScreen.SetActive(true);
        //GameObject ResumeButton = pauseScreen.transform.Find("Resume Button").gameObject;
        //var eventSystem = EventSystem.current;
        //eventSystem.SetSelectedGameObject(ResumeButton, new BaseEventData(eventSystem));
    }

    public void DisablePauseScreen()
    {
        pauseScreen.SetActive(false);
    }

    public void RenderGameOverScreen(bool victorious)
    {
        if (victorious)
        {
            victoryScreen.SetActive(true);
            //GameObject ReplayButton = victoryScreen.transform.Find("Replay Button").gameObject;
            //var eventSystem = EventSystem.current;
            //eventSystem.SetSelectedGameObject(ReplayButton, new BaseEventData(eventSystem));
        }
        else
        {
            gameOverScreen.SetActive(true);
            //GameObject ReplayButton = gameOverScreen.transform.Find("Replay Button").gameObject;
            //var eventSystem = EventSystem.current;
            //eventSystem.SetSelectedGameObject(ReplayButton, new BaseEventData(eventSystem));
        }
    }
}
