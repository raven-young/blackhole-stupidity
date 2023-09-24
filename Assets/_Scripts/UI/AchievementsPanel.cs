using System;
using UnityEngine;
using TMPro;

namespace BlackHole
{
    public class AchievementsPanel : Menu<AchievementsPanel>
    {

        [SerializeField] private TMP_Text _achievementsListText;
        [SerializeField] private TMP_Text _achievementsCountText;

        protected override void OnEnable()
        {
            base.OnEnable();
            _achievementsListText.text = AchievementsManager.Instance.GetAchievementsString();
            _achievementsCountText.text = "Unlocked: " + Math.Round(100f * AchievementsManager.Instance.UnlockedAchievementsFraction) + "%";
        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();
        }
    }
}