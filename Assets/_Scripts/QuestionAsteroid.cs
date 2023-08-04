using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System;
using DamageNumbersPro;

public class QuestionAsteroid : MonoBehaviour
{
    private PlayerInputActions playerInputActions;
    [SerializeField] private GameParams _gameParams;

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

    public static event Action<AvatarReactions.ExpressionEvents> OnProblemSpawned;
    public static event Action<AvatarReactions.ExpressionEvents> OnProblemFailed;
    public static event Action<AvatarReactions.ExpressionEvents> OnProblemSuccess;

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

    private int _totalSpawned = 0;
    private int _correctlyAnswered = 0;
    public float Accuracy; // fraction of correctly answered questions

    public static QuestionAsteroid Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;

        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
        playerInputActions.Player.Answer1.performed += Answer1;
        playerInputActions.Player.Answer2.performed += Answer2;
        playerInputActions.Player.Answer3.performed += Answer3;
    }

    // Start is called before the first frame update
    void Start()
    {
        _questionAsteroid.SetActive(false);
        challenge = new MathChallenge();
        _questionAsteroidSpeed = _gameParams.QuestionAsteroidSpeed;

        _asteroidSpawnBonus = 0;
        switch (SettingsManager.Instance.SelectedDifficulty)
        {
            case SettingsManager.DifficultySetting.Easy:
                _questionAsteroidSpeed *= _gameParams.EasyMultiplier;
                break;
            case SettingsManager.DifficultySetting.Hard:
                _asteroidSpawnBonus += _gameParams.FailAsteroidSpawnBonus;
                break;
        }
    }

    private void OnDisable()
    {
        playerInputActions.Player.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, Vector3.zero, Time.deltaTime * _questionAsteroidSpeed * _speedModifier);

        if (!_questionActive)
        {
            _deltaDelta += Time.deltaTime;

            if (_deltaDelta > _gameParams.QuestionDelta)
            {
                SpawnQuestion();
            }
        }

        else
        {
            if (transform.position.y < 1.03*_gameParams.WinRadius)
                StartCoroutine(Fail());
        }
    }

    void SpawnQuestion()
    {
        if (GameManager.Instance.GameHasEnded)
            return;

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

        //int difficulty = (int)(5f*GameManager.Instance.DistanceToEventHorizon / (_gameParams.WinRadius - GameManager.Instance.EventHorizonRadius));
        int difficulty = (int)GameManager.Instance.DistanceToEventHorizon / 4;

        // refactor later
        var c = challenge.SimpleArithemticChallenge(difficulty);
        switch (SettingsManager.Instance.SelectedDifficulty)
        {
            case SettingsManager.DifficultySetting.Hard:
                // 90% chance of spawning linear equation, 10% chance of spawning quadratic equation
                bool spawnQuadratic = UnityEngine.Random.Range(0f, 1f) < 0.1f;
                if (spawnQuadratic)
                {
                    _speedModifier = 0.35f;
                    _itemSpawnBonus = 4;
                }
                c = challenge.SimpleAlgebraChallenge(difficulty, spawnQuadratic);
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
    IEnumerator Success()
    {
        _questionActive = false;
        _deltaDelta = 0;
        _totalSpawned++;  // don't move to spawned else penalty for asteroinds spawned right before win
        _correctlyAnswered++;

        OnProblemSuccess?.Invoke(AvatarReactions.ExpressionEvents.ProblemSucceeded);
        SoundManager.Instance.PlaySound(_bigLaserClip);
        Debug.Log("Correct answer!");

        // spawn laser
        StartCoroutine(_laserEffect.ActivateLaser(_gameParams.LaserDuration));
        StartCoroutine(Shooting.DisableShoot(_gameParams.LaserDuration));
        yield return new WaitForSeconds(_gameParams.LaserDuration);
        Explode();
        SpawnStuff(true);

        CanvasManager.Instance.ComboCount++;
        if (CanvasManager.Instance.ComboCount > 1)
            _comboDamageNumberPrefab.Spawn(transform.position, CanvasManager.Instance.ComboCount);

        CanvasManager.Instance.IncrementScore(_gameParams.CorrectAnswerScore);

        _questionAsteroid.SetActive(false);
        SoundManager.Instance.PlaySound(_rightAnswerclip);
        SoundManager.Instance.PlaySound(_explosionClip, 0.5f);
        
    }

    // Activate when incorrectly answering question or timer runs out
    IEnumerator Fail()
    {
        _questionActive = false;
        _deltaDelta = 0;
        _totalSpawned++; // don't move to spawned else penalty for asteroinds spawned right before win

        CanvasManager.Instance.ComboCount = 0;
        OnProblemFailed?.Invoke(AvatarReactions.ExpressionEvents.ProblemFailed);
        if (!GameManager.Instance.GameHasEnded)
            SoundManager.Instance.PlayMusic(_wrongAnswerclip);
        yield return new WaitForSeconds(_gameParams.LaserDuration);
        _questionAsteroid.SetActive(false);
        SpawnStuff(false);
        Explode();
        SoundManager.Instance.PlaySound(_explosionClip, 0.5f);
        Debug.Log("Wrong answer!");
        
    }

    void Explode()
    {
        GameObject effect = Instantiate(_explosionEffect, transform.position, Quaternion.identity);
        Destroy(effect, 0.3f);
    }

    void SpawnStuff(bool correctlyAnswered)
    {
        GameObject prefab1 = _scrapPrefab;
        GameObject prefab2 = _scrapPrefab;

        if (SettingsManager.Instance.SelectedDifficulty > SettingsManager.DifficultySetting.Normal)
            prefab2 = _fuelPrefab;

        int spawnAmount = _gameParams.SpawnAmount;
        if (!correctlyAnswered)
        {
            prefab1 = prefab2 = _asteroidPrefab;
            spawnAmount += _asteroidSpawnBonus;
        }

        for (int i = 0; i < spawnAmount + _itemSpawnBonus; i++)
        {
            float randomX = 0.1f * UnityEngine.Random.Range(-1f, 1f) * _gameParams.ScreenBounds.x;
            Vector2 spawnPos = new Vector2(transform.position.x + randomX, transform.position.y);
            GameObject spawnedObject = Instantiate(UnityEngine.Random.Range(0f, 1f) < 0.5 ? prefab1 : prefab2,
                                                   spawnPos, Quaternion.identity);

            float angle = UnityEngine.Random.Range(-_gameParams.MaxSpawnAngle, _gameParams.MaxSpawnAngle);
            Vector2 direction = Vector2.up.Rotate(angle);

            spawnedObject.GetComponent<Rigidbody2D>().AddForce(-UnityEngine.Random.Range(0.5f, 1f) * _gameParams.SpawnImpulse * direction,
                                                               ForceMode2D.Impulse);
        }
    }

    public float GetAccuracy()
    {
        if (_totalSpawned == 0)
            return 0;

        return (float)_correctlyAnswered / _totalSpawned;
    }

    // refactor this later
    void Answer1(InputAction.CallbackContext context)
    {
        if (!_questionActive)
            return;

        if (context.performed)
        {
            _answer1Highlight.gameObject.SetActive(true);

            if (_correctAnswer == 1)
            {
                _answer1Highlight.color = Color.green;
                StartCoroutine(Success());
            }
            else
            {
                _answer1Highlight.color = Color.red;
                StartCoroutine(Fail());
            }
        }
    }

    void Answer2(InputAction.CallbackContext context)
    {
        if (!_questionActive)
            return;

        if (context.performed)
        {
            _answer2Highlight.gameObject.SetActive(true);

            if (_correctAnswer == 2)
            {
                _answer2Highlight.color = Color.green;
                StartCoroutine(Success());
            }
            else
            {
                _answer2Highlight.color = Color.red;
                StartCoroutine(Fail());
            }
        }
    }

    void Answer3(InputAction.CallbackContext context)
    {
        if (!_questionActive)
            return;

        if (context.performed)
        {
            _answer3Highlight.gameObject.SetActive(true);

            if (_correctAnswer == 3)
            {
                _answer3Highlight.color = Color.green;
                StartCoroutine(Success());  
            }
            else
            {
                _answer3Highlight.color = Color.red;
                StartCoroutine(Fail());
            }
        }
    }
}