using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] private GameParams _gameParams;

    public float restartDelay = 2f;
    public bool gameHasEnded = false;
    public bool isPaused = false;
    public bool canPause = true;
    public float currentTimeScale = 1f;

    public static float timePassed = 0f; // total time since start

    public Vector2 ScreenBounds;
    private PlayerInputActions playerInputActions;
    [SerializeField] private Camera _cam;

    [Header("Logic")]
    [SerializeField] private float _initialFuel = 20f; // seconds

    [Header("Sounds")]
    [SerializeField] private AudioClip _deathClip, _victoryClip;

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
        Time.timeScale = 1;
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

    public IEnumerator GameOver(bool victorious = false, float _delay = 0f)
    {
        if (victorious) 
            SoundManager.Instance.PlaySound(_victoryClip);
        else
            SoundManager.Instance.PlaySound(_deathClip);
        gameHasEnded = true;
        canPause = false;
        CanvasManager.Instance.RenderGameOverScreen(victorious);
        yield return new WaitForSeconds(_delay);
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

    public void Quit()
    {
        Application.Quit();
    }

    public void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(new Vector3(0, ScreenBounds.y/2, 0), new Vector3(2*ScreenBounds.x, ScreenBounds.y, 1));

        // playable cone
        if (_gameParams.MaxTheta > 0) {
            Gizmos.DrawLine(Vector3.zero, new Vector3(20*Mathf.Cos(_gameParams.MaxTheta), 20 * Mathf.Sin(_gameParams.MaxTheta), 0));
            Gizmos.DrawLine(Vector3.zero, new Vector3(20 * Mathf.Cos(Mathf.PI-_gameParams.MaxTheta), 20 * Mathf.Sin(Mathf.PI-_gameParams.MaxTheta), 0));
        }
    }
}