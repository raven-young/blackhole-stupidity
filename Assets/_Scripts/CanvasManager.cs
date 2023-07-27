using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour
{

    public static CanvasManager Instance;
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private GameObject victoryScreen;
    [SerializeField] private GameObject pauseScreen;

    [SerializeField] private Slider _fuelSlider, _healthSlider;

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
        _healthSlider.maxValue = Ship.Instance.InitialHealth;
        _healthSlider.minValue = 0;
        _healthSlider.value = Ship.Instance.InitialHealth / 2;
    }

    public void UpdateHealth(float newValue)
    {
        _healthSlider.value = newValue;
    }

    public void UpdateFuel(float newValue)
    {
        _fuelSlider.value = newValue;
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
