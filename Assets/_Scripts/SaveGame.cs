using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveGame : MonoBehaviour
{

    public static void SaveGameNow()
    {
        Debug.Log("Saving game...");

        ES3.Save("AchievementsManager", AchievementsManager.Instance);
    }
}
