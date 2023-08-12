using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using DG.Tweening;

namespace BlackHole
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        private PlayerInputActions playerInputActions;
        private readonly float _dangerZoneMinTime = 6f; // danger zone theme active for at least this long
        private float _dangerzoneTimer = 0f;

        [SerializeField] private GameParams _gameParams;
        [SerializeField] private GameObject _replaybutton_paused;
        [SerializeField] private GameObject _replaybutton_gameover;
        [SerializeField] private GameObject _replaybutton_victory;

        public bool GameHasEnded { get; private set; } = false;
        public bool GameWasWon { get; private set; } = false;
        public bool IsPaused = false;
        public bool CanPause = true;
        public float DistanceToEventHorizon { get; private set; }
        public float EventHorizonRadius { get; private set; }
        //public float InitialDistanceToEventHorizon { get; private set; }
        public bool InDangerZone { get; private set; }

        public static event Action OnEnteredDangerZone;
        public static event Action OnExitedDangerZone;
        public static event Action OnGameOver;
        public static event Action OnVictory;
        public static event Action OnNoDamageVictory;
        public static event Action On100PercentVictory; // 100% correctly solved
        public static event Action OnFlawlessVictory;

        #region Unity Callbacks
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
        private void Start()
        {
            // Game starts paused with input layout pop-up
            Cursor.visible = false;
            Time.timeScale = 0;

            GetDistanceToEventHorizon();
            //InitialDistanceToEventHorizon = DistanceToEventHorizon;
            InDangerZone = DistanceToEventHorizon > _gameParams.DangerZoneDistance;

            InitializeGame();
        }
        private void Update()
        {
            GetDistanceToEventHorizon();

            if (DistanceToEventHorizon < _gameParams.DangerZoneDistance && !InDangerZone)
            {
                InDangerZone = true;
                OnEnteredDangerZone?.Invoke();
                _dangerzoneTimer = 0f;
                Debug.Log("Entered danger zone! " + DistanceToEventHorizon + " " + _gameParams.DangerZoneDistance);
            }
            else if (InDangerZone)
            {
                _dangerzoneTimer += Time.deltaTime;
                if (_dangerZoneMinTime < _dangerzoneTimer && DistanceToEventHorizon > _gameParams.DangerZoneDistance)
                {
                    InDangerZone = false;
                    OnExitedDangerZone?.Invoke();
                    _dangerzoneTimer = 0;
                    Debug.Log("Left danger zone! " + DistanceToEventHorizon + " " + _gameParams.DangerZoneDistance);
                }
            }
        }
        public void OnDrawGizmos()
        {
            // Playable cone
            if (_gameParams.MaxTheta > 0)
            {
                float coneLength = 20;
                Gizmos.DrawLine(Vector3.zero, new Vector3(coneLength * Mathf.Cos(_gameParams.MaxTheta), coneLength * Mathf.Sin(_gameParams.MaxTheta), 0));
                Gizmos.DrawLine(Vector3.zero, new Vector3(coneLength * Mathf.Cos(Mathf.PI - _gameParams.MaxTheta), coneLength * Mathf.Sin(Mathf.PI - _gameParams.MaxTheta), 0));
            }

            // Escape horizon
            Gizmos.DrawWireSphere(Vector3.zero, _gameParams.WinRadius);

            // Danger zone
            Gizmos.color = Color.red;
            if (Application.isPlaying)
            {
                Gizmos.DrawWireSphere(Vector3.zero, Ship.Instance.ShipPositionRadius - DistanceToEventHorizon + _gameParams.DangerZoneDistance);
            }
        }
        private void OnDestroy()
        {
            playerInputActions.Player.EscapeAction.performed -= EscapeAction;
        }

        #endregion

        #region Public Methods

        public IEnumerator GameOver(bool victorious = false)
        {
            if (GameHasEnded) { yield break; }

            GameHasEnded = true;

            SoundManager.Instance.StopMusic();
            SoundManager.Instance.StopSFX();

            var eventSystem = EventSystem.current;

            if (victorious)
            {
                eventSystem.SetSelectedGameObject(_replaybutton_victory, new BaseEventData(eventSystem));
                GameWasWon = true;

                // Check if new achievements unlocked
                OnVictory?.Invoke();
                if (!Ship.Instance.TakenDamage)
                {
                    OnNoDamageVictory?.Invoke();
                }
                if (QuestionAsteroid.Instance.GetAccuracy() == 1)
                {
                    On100PercentVictory?.Invoke();
                }
                if (QuestionAsteroid.Instance.GetAccuracy() == 1 && !Ship.Instance.TakenDamage && !BlackHoleObject.Instance.HasGrown)
                {
                    OnFlawlessVictory?.Invoke();
                }

                // Transition
                StartCoroutine(VictoryTransition.Instance.StartVictoryTransition());
            }
            else
            {
                eventSystem.SetSelectedGameObject(_replaybutton_gameover, new BaseEventData(eventSystem));
                GameWasWon = false;
                OnGameOver?.Invoke();
                StartCoroutine(GameOverTransition.Instance.StartGameOverTransition());
            }

            SaveGame.SaveGameNow();
        }
        public void PauseGame()
        {
            if (!CanPause) { return; }

            Time.timeScale = 0;
            Cursor.visible = true;

            if (!GameHasEnded)
            {
                IsPaused = true;
                var eventSystem = EventSystem.current;
                eventSystem.SetSelectedGameObject(_replaybutton_paused, new BaseEventData(eventSystem));
                CanvasManager.Instance.RenderPauseScreen();
            }
        }
        public void ResumeGame()
        {
            if (!CanPause) { return; }

            IsPaused = false;
            Time.timeScale = 1;
            Cursor.visible = false;
            CanvasManager.Instance.DisablePauseScreen();
        }
        public void Restart()
        {
            DOTween.KillAll();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        public static void QuitToMenu()
        {
            DOTween.KillAll();
            SceneManager.LoadScene("MainMenu");
        }
        public void Quit()
        {
            SaveGame.SaveGameNow();
            Application.Quit();
        }

        #endregion

        #region Private Methods

        private void InitializeGame()
        {
            SettingsManager.Instance.PrepareGame();
            Ship.Instance.InitializeShip();
            StartCoroutine(StartGame());
        }
        IEnumerator StartGame()
        {
            SoundManager.Instance.StartMainGameMusic();
            CanvasManager.Instance.ShowControlsPanel();

            while (true)
            {
                if (Input.anyKey)
                    break;
                yield return null;
            }

            SoundManager.Instance.PlayButtonPress(failed: false);
            CanvasManager.Instance.StartGame();
            CanPause = true;
            Time.timeScale = 1;
        }
        private void EscapeAction(InputAction.CallbackContext context)
        {
            if (!IsPaused && context.performed)
            {
                PauseGame();
            }
            else if (IsPaused && context.performed)
            {
                ResumeGame();
            }
        }
        private void GetDistanceToEventHorizon()
        {
            RaycastHit2D hit = Physics2D.Raycast(Ship.Instance.ShipPositionRadius * Vector2.up, -Vector2.up, 30, LayerMask.GetMask("BlackHole"));
            DistanceToEventHorizon = hit.distance;
            EventHorizonRadius = Ship.Instance.ShipPositionRadius - DistanceToEventHorizon;
        }
        #endregion
    }
}