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

    public bool GameHasEnded = false;
    public bool GameWasWon = false;
    public bool IsPaused = false;
    public bool CanPause = true;
    public float CurrentTimeScale = 1f;

    public Vector2 ScreenBounds;
    private PlayerInputActions playerInputActions;
    [SerializeField] private Camera _cam;

    [Header("Sounds")]
    [SerializeField] private AudioClip _deathClip, _victoryClip;

    public float DistanceToEventHorizon = 8f;
    public float InitialDistanceToEventHorizon { get; private set; }

    public static event Action OnEnteredDangerZone;
    public static event Action OnExitedDangerZone;
    public static event Action OnGameOver;
    public static event Action OnVictory;

    private float _dangerZoneMinTime = 6f; // danger zone theme active for at least this long
    private float _dangerzoneTimer = 0f;
    public bool InDangerZone;

    [SerializeField] private GameObject _replaybutton_paused;
    [SerializeField] private GameObject _replaybutton_gameover;
    [SerializeField] private GameObject _replaybutton_victory;
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

        DistanceToEscapeHorizon();
        InitialDistanceToEventHorizon = DistanceToEventHorizon;
        InDangerZone = DistanceToEventHorizon > _gameParams.DangerZoneDistance;

        StartGame();
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
        SoundManager.Instance.StartMainGameMusic();
        CanPause = true;
        Time.timeScale = 1;
    }

    private void EscapeAction(InputAction.CallbackContext context)
    {
        if (!IsPaused && context.performed)
            PauseGame();
        else if (IsPaused && context.performed)
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

        var eventSystem = EventSystem.current;

        //CanvasManager.Instance.SwitchActionMap();
        
        GameHasEnded = true;

        SoundManager.Instance.ChangeMusicVolume(0f);

        if (victorious)
        {
            eventSystem.SetSelectedGameObject(_replaybutton_victory, new BaseEventData(eventSystem));
            SoundManager.Instance.PlaySound(_victoryClip);
            GameWasWon = true;
            OnVictory?.Invoke();
        }
        else
        {
            eventSystem.SetSelectedGameObject(_replaybutton_gameover, new BaseEventData(eventSystem));
            SoundManager.Instance.PlaySound(_deathClip);
            OnGameOver?.Invoke();
        }

        CanvasManager.Instance.RenderGameOverScreen(victorious);
        
        PauseGame();
        yield return new WaitForSecondsRealtime(7f);
        SoundManager.Instance.StartMainGameMusic(4f);
    }

    public void PauseGame()
    {
        if (!CanPause)
            return;

        var eventSystem = EventSystem.current;
        eventSystem.SetSelectedGameObject(_replaybutton_paused, new BaseEventData(eventSystem));

        CurrentTimeScale = Time.timeScale;
        Time.timeScale = 0;
        Cursor.visible = true;
        if (!GameHasEnded)
        {
            IsPaused = true;
            CanvasManager.Instance.RenderPauseScreen();
        }
    }
    public void ResumeGame()
    {
        if (!CanPause)
            return;

        IsPaused = false;
        Time.timeScale = CurrentTimeScale;
        Cursor.visible = false;
        CanvasManager.Instance.DisablePauseScreen();
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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
        if (!Application.isPlaying) return;

        //Gizmos.DrawWireCube(new Vector3(0, ScreenBounds.y/2, 0), new Vector3(2*ScreenBounds.x, ScreenBounds.y, 1));

        // Playable cone
        if (_gameParams.MaxTheta > 0)
        {
            Gizmos.DrawLine(Vector3.zero, new Vector3(20 * Mathf.Cos(_gameParams.MaxTheta), 20 * Mathf.Sin(_gameParams.MaxTheta), 0));
            Gizmos.DrawLine(Vector3.zero, new Vector3(20 * Mathf.Cos(Mathf.PI - _gameParams.MaxTheta), 20 * Mathf.Sin(Mathf.PI - _gameParams.MaxTheta), 0));
        }

        // Escape horizon
        Gizmos.DrawWireSphere(Vector3.zero, _gameParams.WinRadius);

        // Danger zone
        //Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(Vector3.zero, Ship.Instance.ShipPositionRadius - DistanceToEventHorizon + _gameParams.DangerZoneDistance);
    }
}