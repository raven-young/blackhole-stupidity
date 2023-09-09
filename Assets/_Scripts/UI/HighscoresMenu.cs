using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace BlackHole
{
    public class HighscoresMenu : Menu<HighscoresMenu>
    {

        [SerializeField] private TMP_Text _easyScoreText;
        [SerializeField] private TMP_Text _normalScoreText;
        [SerializeField] private TMP_Text _hardScoreText;

        protected override void OnEnable()
        {
            base.OnEnable();
            _easyScoreText.text = "EASY\n" + PlayerStats.Instance.GetHighscore(SettingsManager.DifficultySetting.Easy);
            _normalScoreText.text = "NORMAL\n" + PlayerStats.Instance.GetHighscore(SettingsManager.DifficultySetting.Normal);
            _hardScoreText.text = "HARD\n" + PlayerStats.Instance.GetHighscore(SettingsManager.DifficultySetting.Hard);
        }

    }
}