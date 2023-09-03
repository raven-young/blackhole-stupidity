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
        [SerializeField] private GameObject _achievementsButton;
        [SerializeField] private AchievementNotification _achievementsNotification;
        [SerializeField] private GameObject _difficultyPanel, _upgradePanel, _extrasPanel, _achievementsPanel, _scoreAttackToggle;
        [SerializeField] private RectTransform _background;

        [SerializeField] private TMP_Text _currencyText;
        [SerializeField] private DamageNumber cashNumberPosPrefab;
        [SerializeField] private DamageNumber cashNumberNegPrefab;
        [SerializeField] private RectTransform cashNumberRectParent;
        private DamageNumber cashNumber;

        private PlayerInputActions playerInputActions;

        private void Awake()
        {
            playerInputActions = new PlayerInputActions();
            playerInputActions.Enable();
            //playerInputActions.Player.EscapeAction.performed += EscapeAction;
            //playerInputActions.Player.Answer3.performed += EscapeAction;
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
            //playerInputActions.Player.EscapeAction.performed -= EscapeAction;
            //playerInputActions.Player.Answer3.performed -= EscapeAction;
        }

        private void Start()
        {
            SaveGame.LoadGameNow();

            Time.timeScale = 1f;
            SoundManager.Instance.StartMainMenuMusic();
            _gameParams.ScreenBounds = _cam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, _cam.transform.position.z));

            // Tween all the things
            //float _buttonY = _startButton.transform.position.y;
            //_startButton.transform.DOMoveY(_buttonY + 0.7f, 1f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
            //_quitButton.transform.DOMoveY(_buttonY + 0.7f, 1f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo).SetDelay(0.2f);
            //_extrasButton.transform.DOMoveY(_extrasButton.transform.position.y + 0.7f, 1f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo).SetDelay(0.4f);

            _background.DOShapeCircle(Vector2.zero, 360, 22f).SetEase(Ease.InOutFlash).SetLoops(-1, LoopType.Yoyo);

            // Init some things
            //_activePanel = _startButton;
            //UpgradeManager.Instance.InitializeUpgrades();

            // Cash
            _currencyText.text = "$" + _bankSO.AvailableCurrency.ToString();
        }

        //public void ActivateExtrasPanel()
        //{
        //    _extrasPanel.SetActive(true);
        //    _activePanel = _extrasPanel;
        //}

        //public void ActivateAchievementsText()
        //{
        //    _achievementsListText.text = AchievementsManager.Instance.GetAchievementsString();
        //    _activePanel = _achievementsPanel;
        //}

        //public void Updateachievementstext()
        //{
        //    _achievementslisttext.text = achievementsmanager.instance.getachievementsstring();
        //}

        //public void Quit()
        //{
        //    if (UpgradeSlot.UpgradeSlots != null)
        //    {
        //        UpgradeSlot.SaveAllSlotStates();
        //    }
        //    Application.Quit();
        //}

        //private void EscapeAction(InputAction.CallbackContext context)
        //{
        //    if (context.performed)
        //    {
        //        Escape();
        //    }
        //}

        //public void Escape()
        //{
        //    _startButton.GetComponent<Button>().PlayPressed();
        //    var eventSystem = EventSystem.current;

        //    if (_activePanel == _difficultyPanel)
        //    {
        //        _startButton.GetComponent<UnityEngine.UI.Button>().interactable = true;
        //        _quitButton.GetComponent<UnityEngine.UI.Button>().interactable = true;
        //        _extrasButton.GetComponent<UnityEngine.UI.Button>().interactable = true;
        //        _difficultyPanel.SetActive(false);
        //        _activePanel = _startButton; // terrible
        //        eventSystem.SetSelectedGameObject(_startButton, new BaseEventData(eventSystem));
        //    }

        //    else if (_activePanel == _upgradePanel)
        //    {
        //        _difficultyPanel.SetActive(true);
        //        _upgradePanel.SetActive(false);
        //        _activePanel = _difficultyPanel;
        //        eventSystem.SetSelectedGameObject(_normalDifficultyButton, new BaseEventData(eventSystem));
        //    }

        //    else if (_activePanel == _achievementsPanel)
        //    {
        //        _extrasPanel.SetActive(true);
        //        _achievementsPanel.SetActive(false);
        //        _activePanel = _extrasPanel;
        //        eventSystem.SetSelectedGameObject(_achievementsButton, new BaseEventData(eventSystem));
        //    }

        //    else if (_activePanel == _extrasPanel)
        //    {
        //        _extrasPanel.SetActive(false);
        //        _activePanel = _startButton;
        //        eventSystem.SetSelectedGameObject(_startButton, new BaseEventData(eventSystem));
        //        _startButton.GetComponent<UnityEngine.UI.Button>().interactable = true;
        //        _quitButton.GetComponent<UnityEngine.UI.Button>().interactable = true;
        //        _extrasButton.GetComponent<UnityEngine.UI.Button>().interactable = true;
        //    }

        //    else if (_activePanel == _startButton)
        //        return;
        //}

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