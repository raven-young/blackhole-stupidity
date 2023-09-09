using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace BlackHole
{
    class Preprocess : IPreprocessBuildWithReport
    {
        public int callbackOrder { get { return 0; } }
        public void OnPreprocessBuild(BuildReport report)
        {
            ES3.DeleteFile("SaveFile.es3");
            AchievementsManager.Instance.ResetAchievements();
            UpgradeSlotManager.Instance.ResetAllSlots();
            Bank.CashTransfer(-Bank.AvailableCurrency);
            PlayerStats.Instance.ResetHighscores();
            SettingsManager.ResetUnlocks();
        }
    }
}