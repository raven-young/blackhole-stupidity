using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlackHole
{
    public class DifficultySelectionMenu : Menu<DifficultySelectionMenu>
    {
        [SerializeField] private GameObject _scoreAttackToggle;

        protected override void OnEnable()
        {
            base.OnEnable();
            _scoreAttackToggle.SetActive(SettingsManager.ScoreAttackUnlocked);
        }

        public void OnDifficultySelected(int selectedDifficulty)
        {
            SettingsManager.Instance.SelectedDifficulty = (SettingsManager.DifficultySetting)selectedDifficulty;
            UpgradeMenu.Open();
        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();
        }
    }
}