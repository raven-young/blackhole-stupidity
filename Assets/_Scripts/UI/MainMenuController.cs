using System;
using UnityEngine;
using DG.Tweening;
using TMPro;
using DamageNumbersPro;

namespace BlackHole
{
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField] private GameParams _gameParams;
        [SerializeField] private TMP_Text _achievementsListText;
        [SerializeField] private Camera _cam;
        [SerializeField] private GameObject _achievementsButton;
        [SerializeField] private AchievementNotification _achievementsNotification;
        [SerializeField] private RectTransform _background;

        [SerializeField] private TMP_Text _currencyText;
        [SerializeField] private DamageNumber cashNumberPosPrefab;
        [SerializeField] private DamageNumber cashNumberNegPrefab;
        [SerializeField] private RectTransform cashNumberRectParent;
        private DamageNumber cashNumber;


        private void OnEnable()
        {
            AchievementsManager.OnAchievementUnlocked += DisplayAchievement;
            Bank.OnCashTransfer += UpdateCurrencyText;
        }

        private void OnDisable()
        {
            AchievementsManager.OnAchievementUnlocked -= DisplayAchievement;
            Bank.OnCashTransfer -= UpdateCurrencyText;
        }

        private void Start()
        {
            SaveGame.LoadGameNow();

            Time.timeScale = 1f;
            SoundManager.Instance.StartMainMenuMusic();
            _gameParams.ScreenBounds = _cam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, _cam.transform.position.z));

            _background.DOShapeCircle(Vector2.zero, 360, 22f).SetEase(Ease.InOutFlash).SetLoops(-1, LoopType.Yoyo);

            // Init some things
            //UpgradeManager.Instance.InitializeUpgrades();

            // Cash
            _currencyText.text = "$" + Bank.AvailableCurrency.ToString();
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
            _currencyText.text = "$" + Bank.AvailableCurrency.ToString();
            SoundManager.Instance.PlaySFX(SoundManager.SFX.Kaching);
        }
    }
}