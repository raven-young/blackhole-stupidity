using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using DG.Tweening;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] private GameParams _gameParams;

    public bool gameHasEnded = false;
    public bool isPaused = false;
    public bool canPause = true;
    public float currentTimeScale = 1f;

    public static float timePassed = 0f; // total time since start

    public Vector2 ScreenBounds;
    private PlayerInputActions playerInputActions;
    [SerializeField] private Camera _cam;

    [Header("Sounds")]
    [SerializeField] private AudioClip _deathClip, _victoryClip;

    public float DistanceToEventHorizon = 8f;
    public float InitialDistanceToEventHorizon { get; private set; }

    public static event Action OnEnteredDangerZone;
    public static event Action OnExitedDangerZone;
    //public event EventHandler OnCloseToGameOver;

    private float _dangerZoneMinTime = 6f; // danger zone theme active for at least this long
    private float _dangerzoneTimer = 0f;
    public bool InDangerZone;

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
        SoundManager.Instance.ChangeToBG();

        DistanceToEscapeHorizon();
        InitialDistanceToEventHorizon = DistanceToEventHorizon;

        InDangerZone = DistanceToEventHorizon > _gameParams.DangerZoneDistance;

        Time.timeScale = 0;
        canPause = false;
    }

    private void Update()
    {
        
        DistanceToEscapeHorizon();
        if (DistanceToEventHorizon < _gameParams.DangerZoneDistance && !InDangerZone)
        {
            InDangerZone = true;
            OnEnteredDangerZone?.Invoke();
            _dangerzoneTimer = 0f;
            Debug.Log("entered danger zone! "+ DistanceToEventHorizon + " " + _gameParams.DangerZoneDistance);
        } else if (InDangerZone)
        {
            _dangerzoneTimer += Time.deltaTime;
            if (_dangerZoneMinTime < _dangerzoneTimer && DistanceToEventHorizon > _gameParams.DangerZoneDistance)
            {
                InDangerZone = false;
                OnExitedDangerZone?.Invoke();
                Debug.Log("left danger zone! " + DistanceToEventHorizon + " " + _gameParams.DangerZoneDistance);
                _dangerzoneTimer = 0;
            }
        }
    }

    public void StartGame()
    {
        canPause = true;
        Time.timeScale = 1;
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

    void DistanceToEscapeHorizon()
    {
        RaycastHit2D hit = Physics2D.Raycast(Ship.Instance.ShipPositionRadius * Vector2.up, -Vector2.up, 30, LayerMask.GetMask("BlackHole"));
        DistanceToEventHorizon = hit.distance;
    }

    public IEnumerator GameOver(bool victorious = false)
    {
        SoundManager.Instance.ChangeMusicVolume(0f);
        if (victorious)
            SoundManager.Instance.PlaySound(_victoryClip);
        else 
            SoundManager.Instance.PlaySound(_deathClip);
        gameHasEnded = true;
        canPause = false;
        CanvasManager.Instance.RenderGameOverScreen(victorious);

        yield return new WaitForSecondsRealtime(0.5f);
        SoundManager.Instance.ChangeActiveMusicVolume(1f, 2f);
        
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

    public static void QuitToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void OnDrawGizmos()
    {
        //Gizmos.DrawWireCube(new Vector3(0, ScreenBounds.y/2, 0), new Vector3(2*ScreenBounds.x, ScreenBounds.y, 1));

        // Playable cone
        if (_gameParams.MaxTheta > 0) {
            Gizmos.DrawLine(Vector3.zero, new Vector3(20*Mathf.Cos(_gameParams.MaxTheta), 20 * Mathf.Sin(_gameParams.MaxTheta), 0));
            Gizmos.DrawLine(Vector3.zero, new Vector3(20 * Mathf.Cos(Mathf.PI-_gameParams.MaxTheta), 20 * Mathf.Sin(Mathf.PI-_gameParams.MaxTheta), 0));
        }

        // Escape horizon
        Gizmos.DrawWireSphere(Vector3.zero, _gameParams.WinRadius);

        // Danger zone
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(Vector3.zero, Ship.Instance.ShipPositionRadius-DistanceToEventHorizon+_gameParams.DangerZoneDistance);
    }
}