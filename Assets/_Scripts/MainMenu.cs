using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour
{

    [SerializeField] private GameParams _gameParams;
    [SerializeField] private Camera _cam;
    [SerializeField] private GameObject _startButton, _quitButton, _normalDifficultyButton, _basicShipButton;
    [SerializeField] private SpriteRenderer _blackPanel;

    [SerializeField] private GameObject _difficultyPanel, _shipPanel;
    private GameObject _activePanel;

    private PlayerInputActions playerInputActions;

    private void Awake()
    {
        playerInputActions = new PlayerInputActions();
        playerInputActions.Enable();
        playerInputActions.Player.EscapeAction.performed += EscapeAction;
        playerInputActions.Player.Answer3.performed += EscapeAction;
    }

    private void OnDisable()
    {
        playerInputActions.Disable();
        playerInputActions.Player.EscapeAction.performed -= EscapeAction;
        playerInputActions.Player.Answer3.performed -= EscapeAction;
    }

    private void Start()
    {
        Time.timeScale = 1f;
        SoundManager.Instance.StartMainMenuMusic();
        _gameParams.ScreenBounds = _cam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, _cam.transform.position.z));
        float _buttonY = _startButton.transform.position.y;
        _startButton.transform.DOMoveY(_buttonY + 0.7f, 1f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
        _quitButton.transform.DOMoveY(_buttonY + 0.7f, 1f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo).SetDelay(0.2f);
    }

    private void Update()
    {
        // when clicking with mouse, controller cannot be used anymore, hence set selected gameobject
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            if (_activePanel == _startButton)
                EventSystem.current.SetSelectedGameObject(_startButton, new BaseEventData(EventSystem.current));
            else if (_activePanel == _difficultyPanel)
                EventSystem.current.SetSelectedGameObject(_normalDifficultyButton, new BaseEventData(EventSystem.current));
            else if (_activePanel == _shipPanel)
                EventSystem.current.SetSelectedGameObject(_basicShipButton, new BaseEventData(EventSystem.current));
        }
    }

    public void ActivateDifficultyPanel()
    {
        _difficultyPanel.gameObject.SetActive(true);
        _activePanel = _difficultyPanel;
    }

    // Wrapper for button
    public void SetDifficulty(int selectedDifficulty)
    {
        SettingsManager.Instance.SelectedDifficulty = (SettingsManager.DifficultySetting)selectedDifficulty;
        _difficultyPanel.gameObject.SetActive(false);
        _shipPanel.gameObject.SetActive(true);
        _activePanel = _shipPanel;
    }

    public void SetShipAndStart(int selectedShip)
    {
        SettingsManager.Instance.SelectedShipType = (SettingsManager.ShipType)selectedShip;
        StartCoroutine(StartGameRoutine());
    }

    private IEnumerator StartGameRoutine()
    {
        SettingsManager.Instance.CalculateGameParams();
        _blackPanel.DOFade(1f, 1f);
        yield return new WaitForSecondsRealtime(1f);
        DOTween.KillAll();
        SceneManager.LoadScene("CutsceneIntro");
    }
    public void Quit()
    {
        Application.Quit();
    }

    // horrible
    private void EscapeAction(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _startButton.GetComponent<Button>().PlayPressed();
            var eventSystem = EventSystem.current;

            if (_activePanel == _difficultyPanel)
            {
                _startButton.GetComponent<UnityEngine.UI.Button>().interactable = true;
                _quitButton.GetComponent<UnityEngine.UI.Button>().interactable = true;
                _difficultyPanel.gameObject.SetActive(false);
                _activePanel = _startButton; // terrible
                eventSystem.SetSelectedGameObject(_startButton, new BaseEventData(eventSystem));
            }

            else if (_activePanel == _shipPanel)
            {
                _difficultyPanel.gameObject.SetActive(true);
                _shipPanel.gameObject.SetActive(false);
                _activePanel = _difficultyPanel;
                eventSystem.SetSelectedGameObject(_normalDifficultyButton, new BaseEventData(eventSystem));
            }

            else if (_activePanel == _startButton)
                return;
        }
    }
}
