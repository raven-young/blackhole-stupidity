using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlackHole
{
    public class SaveGame : MonoBehaviour
    {

        public static void SaveGameNow()
        {
            Debug.Log("Saving game...");

            ES3.Save("AchievementsManager", AchievementsManager.Instance);
            ES3.Save("UpgradeManager", UpgradeManager.Instance);
            ES3.Save("SettingsManager", SettingsManager.Instance);
        }

        public static void LoadGameNow()
        {
            Debug.Log("Loading game...");

            // These scriptable objects are loaded by the getter of the singleton
            var dummy = AchievementsManager.Instance.name;
            dummy = UpgradeManager.Instance.name;
            dummy = SettingsManager.Instance.name;
        }
    }
}
