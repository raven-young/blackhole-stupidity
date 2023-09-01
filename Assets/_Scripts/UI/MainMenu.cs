using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlackHole
{
    public class MainMenu : Menu<MainMenu>
    {
        public void OnPlayPressed()
        {
            if (MenuManager.Instance != null && DifficultySelectionMenu.Instance != null)
            {
                MenuManager.Instance.OpenMenu(DifficultySelectionMenu.Instance);
            }
        }

        public override void OnBackPressed()
        {
            Application.Quit();
        }

    }
}