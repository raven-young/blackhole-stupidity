using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using TMPro;
using DamageNumbersPro;

namespace BlackHole
{
    public class MainMenuOld : MonoBehaviour
    {
        [SerializeField] private GameParams _gameParams;
        [SerializeField] private Bank _bankSO;
        //[SerializeField] private AchievementsManager _achievementsScriptableObject;
        [SerializeField] private TMP_Text _achievementsListText;
        [SerializeField] private Camera _cam;
        [SerializeField] private GameObject _startButton, _quitButton, _extrasButton, _normalDifficultyButton, _basicShipButton, _achievementsButton;
        [SerializeField] private Image _blackPanel;
        [SerializeField] private AchievementNotification _achievementsNotification;
        [SerializeField] private GameObject _difficultyPanel, _upgradePanel, _extrasPanel, _achievementsPanel, _scoreAttackToggle;
        [SerializeField] private RectTransform _background;

        [SerializeField] private TMP_Text _currencyText;
        [SerializeField] private DamageNumber cashNumberPosPrefab;
        [SerializeField] private DamageNumber cashNumberNegPrefab;
        [SerializeField] private RectTransform cashNumberRectParent;
        private DamageNumber cashNumber;

        private GameObject _activePanel;
        private PlayerInputActions playerInputActions;

        private void Awake()
        {
            playerInputActions = new PlayerInputActions();
            playerInputActions.Enable();
            playerInputActions.Player.EscapeAction.performed += EscapeAction;
            playerInputActions.Player.Answer3.performed += EscapeAction;
        }

        private void OnEnable()
        {
            AchievementsManager.OnAchievementUnlocked += DisplayAchievement;
            Bank.OnCashTransfer += UpdateCurrencyText;
        }

        private void OnDisable()
        {
            AchievementsManager.OnAchievementUnlocked -= DisplayAchievement;
            Bank.OnCashTransfer -= UpdateCurrencyText;

            playerInputActions.Disable();
            playerInputActions.Player.EscapeAction.performed -= EscapeAction;
            playerInputActions.Player.Answer3.performed -= EscapeAction;
        }

        private void Start()
        {
            SaveGame.LoadGameNow();

            Time.timeScale = 1f;
            SoundManager.Instance.StartMainMenuMusic();
            _gameParams.ScreenBounds = _cam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, _cam.transform.position.z));

            // Tween all the things
            float _buttonY = _startButton.transform.position.y;
            _startButton.transform.DOMoveY(_buttonY + 0.7f, 1f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
            _quitButton.transform.DOMoveY(_buttonY + 0.7f, 1f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo).SetDelay(0.2f);
            _extrasButton.transform.DOMoveY(_extrasButton.transform.position.y + 0.7f, 1f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo).SetDelay(0.4f);

            _background.DOShapeCircle(Vector2.zero, 360, 22f).SetEase(Ease.InOutFlash).SetLoops(-1, LoopType.Yoyo);

            // Init some things
            _activePanel = _startButton;
            UpgradeManager.Instance.InitializeUpgrades();

            // Cash
            _currencyText.text = "$" + _bankSO.AvailableCurrency.ToString();
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
                else if (_activePanel == _upgradePanel)
                    EventSystem.current.SetSelectedGameObject(_basicShipButton, new BaseEventData(EventSystem.current));
                else if (_activePanel == _extrasPanel)
                    EventSystem.current.SetSelectedGameObject(_achievementsButton, new BaseEventData(EventSystem.current));
            }
        }

        public void ActivateDifficultyPanel()
        {
            _difficultyPanel.SetActive(true);
            _activePanel = _difficultyPanel;
            Debug.Log("unlocked: " + SettingsManager.ScoreAttackUnlocked);
            if (SettingsManager.ScoreAttackUnlocked)
            {
                _scoreAttackToggle.SetActive(true);
            }
            else
            {
                _scoreAttackToggle.SetActive(false);
            }
        }

        // Wrapper for button
        public void SetDifficulty(int selectedDifficulty)
        {
            SettingsManager.Instance.SelectedDifficulty = (SettingsManager.DifficultySetting)selectedDifficulty;
            _difficultyPanel.SetActive(false);
            _upgradePanel.SetActive(true);
            _activePanel = _upgradePanel;
        }

        public void SetShipAndStart(int selectedShip)
        {
            SettingsManager.Instance.SelectedShipType = (SettingsManager.ShipType)selectedShip;
            UpgradeSlot.SaveAllSlotStates();
            StartCoroutine(StartGameRoutine());
        }

        public void ActivateExtrasPanel()
        {
            _extrasPanel.SetActive(true);
            _activePanel = _extrasPanel;
        }

        public void ActivateAchievementsText()
        {
            _achievementsListText.text = AchievementsManager.Instance.GetAchievementsString();
            _activePanel = _achievementsPanel;
        }

        public void UpdateAchievementsText()
        {
            _achievementsListText.text = AchievementsManager.Instance.GetAchievementsString();
        }

        private IEnumerator StartGameRoutine()
        {
            _blackPanel.DOFade(1f, 1f);
            yield return new WaitForSecondsRealtime(1f);
            DOTween.KillAll();
            SceneManager.LoadScene("CutsceneIntro");
        }
        public void Quit()
        {
            if (UpgradeSlot.UpgradeSlots != null)
            {
                UpgradeSlot.SaveAllSlotStates();
            }
            Application.Quit();
        }

        private void EscapeAction(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                Escape();
            }
        }

        public void Escape()
        {
            _startButton.GetComponent<Button>().PlayPressed();
            var eventSystem = EventSystem.current;

            if (_activePanel == _difficultyPanel)
            {
                _startButton.GetComponent<UnityEngine.UI.Button>().interactable = true;
                _quitButton.GetComponent<UnityEngine.UI.Button>().interactable = true;
                _extrasButton.GetComponent<UnityEngine.UI.Button>().interactable = true;
                _difficultyPanel.SetActive(false);
                _activePanel = _startButton; // terrible
                eventSystem.SetSelectedGameObject(_startButton, new BaseEventData(eventSystem));
            }

            else if (_activePanel == _upgradePanel)
            {
                _difficultyPanel.SetActive(true);
                _upgradePanel.SetActive(false);
                _activePanel = _difficultyPanel;
                eventSystem.SetSelectedGameObject(_normalDifficultyButton, new BaseEventData(eventSystem));
            }

            else if (_activePanel == _achievementsPanel)
            {
                _extrasPanel.SetActive(true);
                _achievementsPanel.SetActive(false);
                _activePanel = _extrasPanel;
                eventSystem.SetSelectedGameObject(_achievementsButton, new BaseEventData(eventSystem));
            }

            else if (_activePanel == _extrasPanel)
            {
                _extrasPanel.SetActive(false);
                _activePanel = _startButton;
                eventSystem.SetSelectedGameObject(_startButton, new BaseEventData(eventSystem));
                _startButton.GetComponent<UnityEngine.UI.Button>().interactable = true;
                _quitButton.GetComponent<UnityEngine.UI.Button>().interactable = true;
                _extrasButton.GetComponent<UnityEngine.UI.Button>().interactable = true;
            }

            else if (_activePanel == _startButton)
                return;
        }

        void DisplayAchievement(AchievementsManager.Achievement achievement)
        {
            _achievementsNotification.EnqeueueAchievement(achievement);
            _achievementsNotification.StartAchievementsDisplay();
        }

        public void UpdateCurrencyText(int cash)
        {
            cashNumber = cash >= 0 ? cashNumberPosPrefab.Spawn(Vector3.zero, cash) : cashNumberNegPrefab.Spawn(Vector3.zero, -cash);
            cashNumber.SetAnchoredPosition(cashNumberRectParent, new Vector2(0, 0));
            _currencyText.text = "$" + _bankSO.AvailableCurrency.ToString();
            SoundManager.Instance.PlaySFX(SoundManager.SFX.Kaching);
        }
    }
}