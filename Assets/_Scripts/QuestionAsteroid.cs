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

    [Header("Question")]
    [SerializeField] private GameObject _questionAsteroid;
    [SerializeField] private TMP_Text _questionText;
    [SerializeField] private TMP_Text _answer1Text;
    [SerializeField] private TMP_Text _answer2Text;
    [SerializeField] private TMP_Text _answer3Text;

    public static event Action<AvatarReactions.ExpressionEvents> OnProblemSpawned;
    public static event Action<AvatarReactions.ExpressionEvents> OnProblemFailed;
    public static event Action<AvatarReactions.ExpressionEvents> OnProblemSuccess;

    private bool _questionActive = false;
    private int _correctAnswer; // 1,2,3
    private float _deltaDelta = 0;
    private MathChallenge challenge;

    [SerializeField] private AudioClip _rightAnswerclip, _bigLaserClip, _wrongAnswerclip;

    [SerializeField] private DamageNumber _comboDamageNumberPrefab;

    private void Awake()
    {
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
    }

    private void OnDisable()
    {
        playerInputActions.Player.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, Vector3.zero, Time.deltaTime * _gameParams.QuestionAsteroidSpeed);

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

        OnProblemSpawned?.Invoke(AvatarReactions.ExpressionEvents.ProblemSpawned);
        //SoundManager.Instance.PlaySound(_alertClip);
        _deltaDelta = 0;
        _questionActive = true;
        _questionAsteroid.SetActive(true);
        transform.position = _spawnPoint.position;

        //int difficulty = (int)(5f*GameManager.Instance.DistanceToEventHorizon / (_gameParams.WinRadius - GameManager.Instance.EventHorizonRadius));
        int difficulty = (int)GameManager.Instance.DistanceToEventHorizon / 4;
        //var c = challenge.SimpleArithemticChallenge(difficulty);
        //var c = challenge.SimpleAlgebraChallenge(difficulty);
        var c = challenge.SimpleArithemticChallenge(difficulty);
        Debug.Log("challenge level: " + difficulty);
        _correctAnswer = c.Item5 + 1;
        _questionText.text = c.Item1;
        _answer1Text.text = c.Item2;
        _answer2Text.text = c.Item3;
        _answer3Text.text = c.Item4;
    }

    // Activate when correctly answering question
    IEnumerator Success()
    {
        OnProblemSuccess?.Invoke(AvatarReactions.ExpressionEvents.ProblemSucceeded);
        _questionActive = false;

        SoundManager.Instance.PlaySound(_bigLaserClip);
        Debug.Log("Correct answer!");

        // spawn laser
        StartCoroutine(LaserEffect.Instance.ActivateLaser(_gameParams.LaserDuration));
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
        CanvasManager.Instance.ComboCount = 0;
        OnProblemFailed?.Invoke(AvatarReactions.ExpressionEvents.ProblemFailed);
        SoundManager.Instance.PlayMusic(_wrongAnswerclip);
        yield return new WaitForSeconds(_gameParams.LaserDuration);
        _questionActive = false;
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
        GameObject prefab2 = _fuelPrefab;

        if (!correctlyAnswered)
            prefab1 = prefab2 = _asteroidPrefab;

        for (int i = 0; i < _gameParams.SpawnAmount; i++)
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

    // refactor this later
    void Answer1(InputAction.CallbackContext context)
    {
        if (!_questionActive)
            return;

        if (context.performed)
        {
            if (_correctAnswer == 1)
                StartCoroutine(Success());
            else
                StartCoroutine(Fail());
        }
    }

    void Answer2(InputAction.CallbackContext context)
    {
        if (!_questionActive)
            return;

        if (context.performed)
        {
            if (_correctAnswer == 2)
                StartCoroutine(Success());
            else
                StartCoroutine(Fail());
        }
    }

    void Answer3(InputAction.CallbackContext context)
    {
        if (!_questionActive)
            return;

        if (context.performed)
        {
            if (_correctAnswer == 3)
                StartCoroutine(Success());
            else
                StartCoroutine(Fail());
        }
    }
}