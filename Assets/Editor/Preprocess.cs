using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace BlackHole
{
    class Preprocess : IPreprocessBuildWithReport
    {

        private AchievementsManager _achievementsSO;

        public int callbackOrder { get { return 0; } }
        public void OnPreprocessBuild(BuildReport report)
        {
            ES3.DeleteFile("SaveFile.es3");
            _achievementsSO = AchievementsManager.Instance; //(AchievementsManager)Resources.Load("_ScriptableObjects/AchievementsManager");
            _achievementsSO.ResetAchievements();

            Debug.LogWarning("REMEMBER TO RESET UPGRADES BEFORE BUILD");
            //UpgradeManager.Instance.ResetAllUpgrades(); // this requires upgrade slots to be active
            // to do: reset the slots via slot state in slotmanager SO

            // Reset cash
            Bank.CashTransfer(-Bank.AvailableCurrency);
        }
    }
}