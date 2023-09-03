using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace BlackHole
{
    public class AchievementsPanel : Menu<AchievementsPanel>
    {

        [SerializeField] private TMP_Text _achievementsListText;

        private void OnEnable()
        {
            _achievementsListText.text = AchievementsManager.Instance.GetAchievementsString();
        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();
        }
    }
}