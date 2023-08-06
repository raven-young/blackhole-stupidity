using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

class Preprocess : IPreprocessBuildWithReport
{

    private AchievementsManager _achievementsSO;

    public int callbackOrder { get { return 0; } }
    public void OnPreprocessBuild(BuildReport report)
    {
        _achievementsSO = (AchievementsManager)Resources.Load("_ScriptableObjects/AchievementsManager");
        _achievementsSO.ResetAchievements();
    }
}