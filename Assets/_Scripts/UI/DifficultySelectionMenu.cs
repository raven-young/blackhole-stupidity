using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlackHole
{
    public class DifficultySelectionMenu : Menu<DifficultySelectionMenu>
    {
        [SerializeField] private GameObject _scoreAttackToggle;

        public void OnDifficultySelected(int selectedDifficulty)
        {
            SettingsManager.Instance.SelectedDifficulty = (SettingsManager.DifficultySetting)selectedDifficulty;
            _scoreAttackToggle.SetActive(SettingsManager.ScoreAttackUnlocked);
            UpgradeMenu.Open();
        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();
        }
    }
}