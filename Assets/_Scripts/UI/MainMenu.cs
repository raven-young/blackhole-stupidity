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

        protected override void OnEnable()
        {
            base.OnEnable();
            float _buttonY = _startButton.transform.position.y;
            _startButton.transform.DOMoveY(_buttonY + 5.5f, 1f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
            _quitButton.transform.DOMoveY(_buttonY + 5.5f, 1f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo).SetDelay(0.2f);
            _extrasButton.transform.DOMoveY(_extrasButton.transform.position.y + 5.5f, 1f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo).SetDelay(0.4f);

        }

        protected override void OnDisable()
        {
            base.OnDisable();
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