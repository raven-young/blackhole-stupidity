using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlackHole
{
    public class MainMenu : Menu<MainMenu>
    {
        public void OnPlayPressed()
        {
            UpgradeManager.Instance.InitializeUpgrades();
            DifficultySelectionMenu.Open();
        }

        public override void OnBackPressed()
        {
            Application.Quit();
        }

    }
}