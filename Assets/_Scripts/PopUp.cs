using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PopUp : MonoBehaviour
{
    public static void SpawnPopUp(string message)
    {
        GameObject popupPrefab = Resources.Load<GameObject>("_Prefabs/GenericPopUp");
        RectTransform canvas = GameObject.FindGameObjectWithTag("Canvas").GetComponent<RectTransform>();
        GameObject popup = Instantiate(popupPrefab, canvas);
        popup.transform.Find("Body").GetComponent<TMP_Text>().text = message;
    }

    // Called by back button on popup prefab
    public void Destroy()
    {
        Destroy(gameObject);
    }
}
