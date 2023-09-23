using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System;
using DamageNumbersPro;

namespace BlackHole
{
    public class QuestionAsteroid : MonoBehaviour
    {
        public static QuestionAsteroid Instance;

        public float SolveAccuracy
        {
            get => _totalSpawned > 0 ? (float)_correctlyAnswered / _totalSpawned : 0f;
            set => Mathf.Clamp(value, 0f, 1f);
        }

        public static event Action<AvatarReactions.ExpressionEvents> OnProblemSpawned;
        public static event Action<AvatarReactions.ExpressionEvents> OnProblemFailed;
        public static event Action<AvatarReactions.ExpressionEvents> OnProblemSuccess;

        private PlayerInputActions playerInputActions;
        [SerializeField] private GameParams _gameParamsSO;

        [SerializeField] private GameObject _scrapPrefab;
        [SerializeField] private GameObject _fuelPrefab;
        [SerializeField] private GameObject _asteroidPrefab;
        [SerializeField] private GameObject _explosionEffect;
        [SerializeField] private Transform _spawnPoint;
        [SerializeField] private AudioClip _explosionClip, _alertClip;
        [SerializeField] private LaserEffect _laserEffect;

        [Header("Question")]
        [SerializeField] private GameObject _questionAsteroid;
        [SerializeField] private TMP_Text _questionText;
        [SerializeField] private TMP_Text _answer1Text;
        [SerializeField] private TMP_Text _answer2Text;
        [SerializeField] private TMP_Text _answer3Text;
        [SerializeField] private SpriteRenderer _answer1Highlight, _answer2Highlight, _answer3Highlight;

        private bool _questionActive = false;
        private int _correctAnswer; // 1,2,3
        private float _deltaDelta = 0;
        private int _asteroidSpawnBonus;
        private float _speedModifier = 1f; // temporarily modify speed for special asteroids, e.g. ultra-hard question
        private int _itemSpawnBonus = 0;
        private float _questionAsteroidSpeed;
        private MathChallenge challenge;

        [SerializeField] private AudioClip _rightAnswerclip, _bigLaserClip, _wrongAnswerclip;

        [SerializeField] private DamageNumber _comboDamageNumberPrefab;
        [SerializeField] private DamageNumber _quickSolveDamageNumberPrefab;

        private int _currentProblemDifficulty; // difficulty of current math problem, higher levels yield higher score
        private int _totalSpawned = 0;
        private int _correctlyAnswered = 0;
        private bool _quickSolveFailed = false;

        private void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(gameObject);
            else
                Instance = this;

            playerInputActions = new PlayerInputActions();
            playerInputActions.Player.Enable();
            playerInputActions.Player.Answer1.performed += AnswerInputAction;
            playerInputActions.Player.Answer2.performed += AnswerInputAction;
            playerInputActions.Player.Answer3.performed += AnswerInputAction;
        }
        private void Start()
        {
            _questionAsteroid.SetActive(false);
            challenge = new MathChallenge();
            _questionAsteroidSpeed = _gameParamsSO.QuestionAsteroidSpeed;

            _asteroidSpawnBonus = 0;
            switch (SettingsManager.Instance.SelectedDifficulty)
            {
                case SettingsManager.DifficultySetting.Easy:
                    _questionAsteroidSpeed *= _gameParamsSO.EasyMultiplier;
                    break;
                case SettingsManager.DifficultySetting.Hard:
                    _asteroidSpawnBonus += _gameParamsSO.FailAsteroidSpawnBonus;
                    break;
            }

            _questionAsteroidSpeed *= SettingsManager.AsteroidSpeedModifier;
        }

        private void OnDisable()
        {
            playerInputActions.Player.Answer1.performed -= AnswerInputAction;
            playerInputActions.Player.Answer2.performed -= AnswerInputAction;
            playerInputActions.Player.Answer3.performed -= AnswerInputAction;
            playerInputActions.Player.Disable();
        }
        private void Update()
        {

            if (GameManager.GameHasEnded)
            {
                if (!_questionAsteroid.activeSelf) { return; }

                Explode();
                return;
            }


            transform.position = Vector3.MoveTowards(transform.position, Vector3.zero, Time.deltaTime * _questionAsteroidSpeed * _speedModifier);

            if (!_questionActive)
            {
                _deltaDelta += Time.deltaTime;

                if (_deltaDelta > _gameParamsSO.QuestionDelta)
                {
                    SpawnQuestion();
                }
            }

            else
            {
                _quickSolveFailed = transform.position.y < _gameParamsSO.QuickSolveRadius;

                if (transform.position.y < 1.03 * _gameParamsSO.WinRadius)
                {
                    StartCoroutine(Fail());
                }
            }
        }

        private void SpawnQuestion()
        {
            if (GameManager.GameHasEnded) { return; }

            _answer1Highlight.gameObject.SetActive(false);
            _answer2Highlight.gameObject.SetActive(false);
            _answer3Highlight.gameObject.SetActive(false);

            OnProblemSpawned?.Invoke(AvatarReactions.ExpressionEvents.ProblemSpawned);
            //SoundManager.Instance.PlaySound(_alertClip);
            _questionActive = true;
            _questionAsteroid.SetActive(true);
            transform.position = _spawnPoint.position;
            _speedModifier = 1f;
            _itemSpawnBonus = 0;
            _quickSolveFailed = false;

            //int difficulty = (int)(5f*GameManager.Instance.DistanceToEventHorizon / (_gameParams.WinRadius - GameManager.Instance.EventHorizonRadius));
            _currentProblemDifficulty = Mathf.Max(1, (int)(GameManager.DistanceToEventHorizon / 2.7f));

            // refactor later
            var c = challenge.SimpleArithemticChallenge(_currentProblemDifficulty);
            switch (SettingsManager.Instance.SelectedDifficulty)
            {
                case SettingsManager.DifficultySetting.Hard:
                    // 90% chance of spawning linear equation, 10% chance of spawning quadratic equation
                    bool spawnQuadratic = false;// UnityEngine.Random.Range(0f, 1f) < 0.1f;
                    if (spawnQuadratic)
                    {
                        _speedModifier = 0.35f;
                        _itemSpawnBonus = 4;
                    }
                    c = challenge.SimpleAlgebraChallenge(_currentProblemDifficulty, spawnQuadratic);
                    break;
            }

            //Debug.Log("challenge level: " + SettingsManager.Instance.SelectedDifficulty + " " + difficulty);
            _correctAnswer = c.Item5 + 1;
            _questionText.text = c.Item1;
            _answer1Text.text = c.Item2;
            _answer2Text.text = c.Item3;
            _answer3Text.text = c.Item4;
        }

        // Activate when correctly answering question
        private IEnumerator Success()
        {
            _questionActive = false;
            _deltaDelta = 0;
            _totalSpawned++;  // don't move to spawned else penalty for asteroinds spawned right before win
            _correctlyAnswered++;

            OnProblemSuccess?.Invoke(AvatarReactions.ExpressionEvents.ProblemSucceeded);
            SoundManager.Instance.PlaySound(_bigLaserClip);
            Debug.Log("Correct answer!");

            // spawn laser
            StartCoroutine(_laserEffect.ActivateLaser(_gameParamsSO.LaserDuration));
            StartCoroutine(Shooting.DisableShoot(_gameParamsSO.LaserDuration));
            yield return new WaitForSeconds(_gameParamsSO.LaserDuration);

            SpawnStuff(true);

            Scoring.Instance.ComboCount++;
            if (Scoring.Instance.ComboCount > 1)
            {
                _comboDamageNumberPrefab.Spawn(transform.position, Scoring.Instance.ComboCount);
            }

            Debug.Log("diff " + _currentProblemDifficulty + " score: " + _gameParamsSO.CorrectAnswerScore * _currentProblemDifficulty);

            int quickSolveBonus = _quickSolveFailed ? 1 : 2;

            if (!_quickSolveFailed)
            {
                _quickSolveDamageNumberPrefab.Spawn(5 * Vector3.right + transform.position);
            }

            Scoring.Instance.IncrementScore(_gameParamsSO.CorrectAnswerScore * _currentProblemDifficulty * quickSolveBonus);

            SoundManager.Instance.PlaySound(_rightAnswerclip);
            AnswerExit();
        }

        // Activate when incorrectly answering question or timer runs out
        private IEnumerator Fail()
        {
            _questionActive = false;
            _deltaDelta = 0;
            _totalSpawned++; // don't move to spawned else penalty for asteroinds spawned right before win

            if (SettingsManager.ComboSaverEnabled)
            {
                SettingsManager.ComboSaverEnabled = false;
            }
            else
            {
                Scoring.Instance.ComboCount = 0;
            }

            OnProblemFailed?.Invoke(AvatarReactions.ExpressionEvents.ProblemFailed);
            if (!GameManager.GameHasEnded)
            {
                SoundManager.Instance.PlaySound(_wrongAnswerclip);
            }
            yield return new WaitForSeconds(_gameParamsSO.LaserDuration);
            SpawnStuff(false);
            Debug.Log("Wrong answer!");
            AnswerExit();
        }

        private void AnswerExit()
        {
            _questionAsteroidSpeed *= _gameParamsSO.QuestionAsteroidAcceleration;
            Explode();
        }

        private void Explode()
        {
            SoundManager.Instance.PlaySound(_explosionClip, 0.5f);
            GameObject effect = Instantiate(_explosionEffect, transform.position, Quaternion.identity);
            Destroy(effect, 0.3f);
            _questionAsteroid.SetActive(false);
        }

        private void SpawnStuff(bool correctlyAnswered)
        {
            GameObject prefab1 = _scrapPrefab;
            GameObject prefab2 = _scrapPrefab;

            if (SettingsManager.Instance.SelectedDifficulty > SettingsManager.DifficultySetting.Normal)
            {
                prefab2 = _fuelPrefab;
            }

            int spawnAmount = _gameParamsSO.SpawnAmount;
            if (!correctlyAnswered)
            {
                prefab1 = prefab2 = _asteroidPrefab;
                spawnAmount += _asteroidSpawnBonus;
            } 
            else
            {
                spawnAmount += SettingsManager.ItemSpawnBonus;
            }

            for (int i = 0; i < spawnAmount + _itemSpawnBonus; i++)
            {
                float randomX = 0.1f * UnityEngine.Random.Range(-1f, 1f) * _gameParamsSO.ScreenBounds.x;
                Vector2 spawnPos = new Vector2(transform.position.x + randomX, transform.position.y);
                GameObject spawnedObject = Instantiate(UnityEngine.Random.Range(0f, 1f) < 0.5 ? prefab1 : prefab2,
                                                       spawnPos, Quaternion.identity);

                float angle = UnityEngine.Random.Range(-_gameParamsSO.MaxSpawnAngle, _gameParamsSO.MaxSpawnAngle);
                Vector2 direction = Vector2.up.Rotate(angle);

                spawnedObject.GetComponent<Rigidbody2D>().AddForce(-UnityEngine.Random.Range(0.5f, 1f) * _gameParamsSO.SpawnImpulse * direction,
                                                                   ForceMode2D.Impulse);
            }
        }

        // refactor this later
        private void AnswerInputAction(InputAction.CallbackContext context)
        {
            if (!_questionActive || GameManager.IsPaused) { return; }

            if (context.performed)
            {
                int chosenAnswer;
                string name = context.action.name;
                SpriteRenderer answerHighlight;
                
                switch (name)
                {
                    case "Answer1":
                        chosenAnswer = 1;
                        answerHighlight = _answer1Highlight;
                        break;
                    case "Answer2":
                        chosenAnswer = 2;
                        answerHighlight = _answer2Highlight;
                        break;
                    case "Answer3":
                        chosenAnswer = 3;
                        answerHighlight = _answer3Highlight;
                        break;
                    default:
                        Debug.LogError("Invalid answer input");
                        chosenAnswer = 0;
                        answerHighlight = _answer1Highlight;
                        break;
                }

                answerHighlight.gameObject.SetActive(true);
                if (chosenAnswer == _correctAnswer)
                {
                    answerHighlight.color = Color.green;
                    StartCoroutine(Success());
                }
                else
                {
                    answerHighlight.color = Color.red;
                    StartCoroutine(Fail());
                }
            }
        }
    }
}