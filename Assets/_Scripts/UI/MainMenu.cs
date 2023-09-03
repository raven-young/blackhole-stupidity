using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace BlackHole
{
    public class MainMenu : Menu<MainMenu>
    {
        private GameObject _startButton, _quitButton, _extrasButton;

        protected override void Awake()
        {
            base.Awake();
            _startButton = transform.Find("StartButton").gameObject;
            _quitButton = transform.Find("QuitButton").gameObject;
            _extrasButton = transform.Find("ExtrasButton").gameObject;
        }

        private void OnEnable()
        {
            float _buttonY = _startButton.transform.position.y;
            _startButton.transform.DOMoveY(_buttonY + 4f, 1f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
            _quitButton.transform.DOMoveY(_buttonY + 4f, 1f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo).SetDelay(0.2f);
            _extrasButton.transform.DOMoveY(_extrasButton.transform.position.y + 4f, 1f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo).SetDelay(0.4f);

        }

        private void OnDisable()
        {
            _startButton.transform.DOKill();
            _quitButton.transform.DOKill();
            _extrasButton.transform.DOKill();
        }

        public void OnPlayPressed()
        {
            UpgradeManager.Instance.InitializeUpgrades();
            DifficultySelectionMenu.Open();
        }

        public void OnExtrasPressed()
        {
            ExtrasMenu.Open();
        }

        public override void OnBackPressed()
        {
            Application.Quit();
        }

    }
}