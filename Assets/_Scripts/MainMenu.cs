using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using TMPro;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameParams _gameParams;
    [SerializeField] private AchievementsManager _achievementsScriptableObject;
    [SerializeField] private TMP_Text _achievementsListText;
    [SerializeField] private Camera _cam;
    [SerializeField] private GameObject _startButton, _quitButton, _extrasButton, _normalDifficultyButton, _basicShipButton, _achievementsButton;
    [SerializeField] private SpriteRenderer _blackPanel;

    [SerializeField] private GameObject _difficultyPanel, _shipPanel, _extrasPanel, _achievementsPanel;
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
        _extrasButton.transform.DOMoveY(_extrasButton.transform.position.y + 0.7f, 1f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo).SetDelay(0.4f);
        _activePanel = _startButton;
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
            else if (_activePanel == _extrasPanel)
                EventSystem.current.SetSelectedGameObject(_achievementsButton, new BaseEventData(EventSystem.current));
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

    public void ActivateExtrasPanel()
    {
        _extrasPanel.gameObject.SetActive(true);
        _activePanel = _extrasPanel;
    }

    public void UpdateAchievementsText()
    {
        _achievementsListText.text = _achievementsScriptableObject.GetAchievementsString();
        _activePanel = _achievementsPanel;
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
                _extrasButton.GetComponent<UnityEngine.UI.Button>().interactable = true;
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

            else if (_activePanel == _achievementsPanel)
            {
                _extrasPanel.gameObject.SetActive(true);
                _achievementsPanel.gameObject.SetActive(false);
                _activePanel = _extrasPanel;
                eventSystem.SetSelectedGameObject(_achievementsButton, new BaseEventData(eventSystem));
            }

            else if (_activePanel == _extrasPanel)
            {
                _extrasPanel.gameObject.SetActive(false);
                _activePanel = _startButton;
                eventSystem.SetSelectedGameObject(_startButton, new BaseEventData(eventSystem));
                _startButton.GetComponent<UnityEngine.UI.Button>().interactable = true;
                _quitButton.GetComponent<UnityEngine.UI.Button>().interactable = true;
                _extrasButton.GetComponent<UnityEngine.UI.Button>().interactable = true;
            }

            else if (_activePanel == _startButton)
                return;
        }
    }
}
