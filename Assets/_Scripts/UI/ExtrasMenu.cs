using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlackHole
{
    public class ExtrasMenu : Menu<ExtrasMenu>
    {
        public void OnAchievementsPressed()
        {
            AchievementsPanel.Open();
        }

        public void OnSettingsPressed()
        {
            SettingsMenu.Open();
        }

        public void OnHighscoresPressed()
        {
            HighscoresMenu.Open();
        }

        public void OnLeaderboardPressed()
        {
            LeaderboardMenu.Open();
        }

    }
}