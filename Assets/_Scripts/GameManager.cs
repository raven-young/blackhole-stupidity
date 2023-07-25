using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public float restartDelay = 2f;
    public bool gameHasEnded = false;
    public bool isPaused = false;
    public bool canPause = true;
    public float currentTimeScale = 1f;

    [SerializeField] private int secondsTillVictory = 180;

    public static float timePassed = 0f; // total time since start

    private PlayerInputActions playerInputActions;

    [SerializeField] private GameObject weaponDropdown;

    private void Awake()
    {
        Instance = this;
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
    }

    private void Start()
    {
        Cursor.visible = false;
    }

    public void Quit()
    {
        Application.Quit();
    }
}