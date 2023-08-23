using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace BlackHole
{
    class Preprocess : IPreprocessBuildWithReport
    {

        private AchievementsManager _achievementsSO;
        private Bank _bankSO;

        public int callbackOrder { get { return 0; } }
        public void OnPreprocessBuild(BuildReport report)
        {
            ES3.DeleteFile("SaveFile.es3");
            _achievementsSO = AchievementsManager.Instance; //(AchievementsManager)Resources.Load("_ScriptableObjects/AchievementsManager");
            _achievementsSO.ResetAchievements();

            // Reset cash
            _bankSO = (Bank)Resources.Load("_ScriptableObjects/Bank");
            _bankSO.CashTransfer(-_bankSO.AvailableCurrency);
        }
    }
}