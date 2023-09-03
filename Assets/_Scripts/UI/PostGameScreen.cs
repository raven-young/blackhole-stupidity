using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace BlackHole
{
    public class PostGameScreen : Menu<PostGameScreen>
    {
        public TMP_Text HighscoreText, ScoreText;
        public Image RaccoonImage;
        public GameObject VictoryImage, GameOverImage;
        [SerializeField] private RectTransform _scorePanel, _scorePanelVictoryPos, _scorePanelGameOverPos;

        public void SwapPostGameState(bool victorious)
        {
            if (victorious)
            {
                VictoryImage.SetActive(true);
                GameOverImage.SetActive(false);
                _scorePanel.localPosition = _scorePanelVictoryPos.localPosition;
            }
            else
            {
                VictoryImage.SetActive(false);
                GameOverImage.SetActive(true);
                _scorePanel.localPosition = _scorePanelGameOverPos.localPosition;
            }
        }

        public void OnRestartPressed()
        {
            GameManager.Restart();
            base.OnBackPressed();
        }

        public void OnQuitToMenuPressed()
        {
            base.OnBackPressed();
            GameManager.QuitToMenu();
        }
    }
}