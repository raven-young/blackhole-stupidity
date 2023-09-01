using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlackHole
{
    public class DifficultySelectionMenu : Menu<DifficultySelectionMenu>
    {
        public void OnDifficultySelected(int selectedDifficulty)
        {
            SettingsManager.Instance.SelectedDifficulty = (SettingsManager.DifficultySetting)selectedDifficulty;
        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();
        }
    }
}