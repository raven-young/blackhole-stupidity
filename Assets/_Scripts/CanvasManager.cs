using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{

    public static CanvasManager Instance;
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private GameObject victoryScreen;
    [SerializeField] private GameObject pauseScreen;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RenderPauseScreen()
    {
        pauseScreen.SetActive(true);
        GameObject ResumeButton = pauseScreen.transform.Find("Resume Button").gameObject;
        //var eventSystem = EventSystem.current;
        //eventSystem.SetSelectedGameObject(ResumeButton, new BaseEventData(eventSystem));
    }

    public void DisablePauseScreen()
    {
        pauseScreen.SetActive(false);
    }

    public void RenderGameOverScreen(bool victorious)
    {
        if (victorious)
        {
            victoryScreen.SetActive(true);
            //GameObject ReplayButton = victoryScreen.transform.Find("Replay Button").gameObject;
            //var eventSystem = EventSystem.current;
            //eventSystem.SetSelectedGameObject(ReplayButton, new BaseEventData(eventSystem));
        }
        else
        {
            gameOverScreen.SetActive(true);
            //GameObject ReplayButton = gameOverScreen.transform.Find("Replay Button").gameObject;
            //var eventSystem = EventSystem.current;
            //eventSystem.SetSelectedGameObject(ReplayButton, new BaseEventData(eventSystem));
        }
    }
}
