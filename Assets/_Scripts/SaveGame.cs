using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlackHole
{
    public class SaveGame
    {

        public static void SaveGameNow()
        {
            Debug.Log("Saving game...");

            ES3.Save("AchievementsManager", AchievementsManager.Instance);
            ES3.Save("UpgradeManager", UpgradeManager.Instance);
            ES3.Save("SettingsManager", SettingsManager.Instance);
            ES3.Save("PlayerStats", PlayerStats.Instance);
            ES3.Save("AvailableCurrency", Bank.AvailableCurrency);
            ES3.Save("UpgradeSlotStateDict", UpgradeSlotManager.Instance.UpgradeSlotStates);
        }

        public static void LoadGameNow()
        {
            if (ES3.KeyExists("AchievementsManager")) {
                Debug.Log("Loading game...");
                AchievementsManager.Instance = ES3.Load<AchievementsManager>("AchievementsManager");
                UpgradeManager.Instance = ES3.Load<UpgradeManager>("UpgradeManager");
                SettingsManager.Instance = ES3.Load<SettingsManager>("SettingsManager");
                PlayerStats.Instance = ES3.Load<PlayerStats>("PlayerStats");
                Bank.AvailableCurrency = ES3.Load<int>("AvailableCurrency");

                if (ES3.KeyExists("UpgradeSlotStateDict"))
                {
                    UpgradeSlotManager.Instance.UpgradeSlotStates = ES3.Load<Dictionary<int, UpgradeSlotManager.UpgradeSlotState>>("UpgradeSlotStateDict");
                }
            }
            else
            {
                Debug.Log("First time starting app");
                Bank.AvailableCurrency = 0;
            }
        }
    }
}
