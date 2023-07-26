using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public float restartDelay = 2f;
    public bool gameHasEnded = false;
    public bool isPaused = false;
    public bool canPause = true;
    public float currentTimeScale = 1f;

    public static float timePassed = 0f; // total time since start

    public Vector2 ScreenBounds;
    private PlayerInputActions playerInputActions;
    [SerializeField] private Camera _cam;

    [Header("UI")]
    [SerializeField] private Slider _fuelSlider;

    [Header("Logic")]
    [SerializeField] private float _initialFuel = 20f; // seconds

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;

        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
        playerInputActions.Player.EscapeAction.performed += EscapeAction;
    }

    private void OnDestroy()
    {
        playerInputActions.Player.EscapeAction.performed -= EscapeAction;
    }

    private void Start()
    {
        Cursor.visible = false;
        _fuelSlider.maxValue = _initialFuel;
        _fuelSlider.value = _initialFuel;

        // need to execute always
        ScreenBounds = _cam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, _cam.transform.position.z));

    }

    private void EscapeAction(InputAction.CallbackContext context)
    {
        if (!isPaused && context.performed)
            PauseGame();
        else if (isPaused && context.performed)
        {
            ResumeGame();
        }
        else if (Time.timeScale == 0 && context.performed)
        {
            ResumeGame();
            Debug.LogWarning("Abnormal unfreezing game");
        }
    }

    public IEnumerator GameOver(bool victorious = false)
    {
        gameHasEnded = true;
        canPause = false;
        CanvasManager.Instance.RenderGameOverScreen(victorious);
        yield return new WaitForSeconds(4f);
        canPause = true;
        PauseGame();
    }
    public void PauseGame()
    {
        if (!canPause)
            return;
        //CanvasManager.Instance.SwitchActionMap();
        currentTimeScale = Time.timeScale;
        Time.timeScale = 0;
        Cursor.visible = true;
        if (!gameHasEnded)
        {
            isPaused = true;
            CanvasManager.Instance.RenderPauseScreen();
        }
    }
    public void ResumeGame()
    {
        if (!canPause)
            return;
        //CanvasManager.Instance.SwitchActionMap();
        isPaused = false;
        Time.timeScale = currentTimeScale;
        Cursor.visible = false;
        CanvasManager.Instance.DisablePauseScreen();
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1;
        timePassed = 0f;
    }

    private void Update()
    {
        _fuelSlider.value -= Time.deltaTime;
    }
    public void Quit()
    {
        Application.Quit();
    }

    public void OnDrawGizmos()
    {
        Gizmos.DrawCube(new Vector3(0, ScreenBounds.y/2, 0), new Vector3(2*ScreenBounds.x, ScreenBounds.y, 1));
    }
}