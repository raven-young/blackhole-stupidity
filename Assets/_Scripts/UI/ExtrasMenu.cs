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

    }
}