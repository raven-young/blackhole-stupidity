using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System;

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
    private float _durationDelta = 0;
    private float _deltaDelta = 0;
    private MathChallenge challenge;

    [SerializeField] private AudioClip _rightAnswerclip, _bigLaserClip, _wrongAnswerclip;

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

            _durationDelta += Time.deltaTime;

            if (_durationDelta > _gameParams.QuestionDuration)
            {
                Fail();
            }
        }

        //else
        //{
        //    if (transform.position.y < _gameParams.WinRadius)
        //        Fail();
        //}
    }

    void SpawnQuestion()
    {
        OnProblemSpawned?.Invoke(AvatarReactions.ExpressionEvents.ProblemSpawned);
        SoundManager.Instance.PlayMusic(_alertClip);
        _deltaDelta = 0;
        _questionActive = true;
        _questionAsteroid.SetActive(true);
        transform.position = _spawnPoint.position;

        int difficulty = (int)GameManager.Instance.DistanceToEventHorizon / 4;
        var c = challenge.challenge(difficulty);
        //Debug.Log("challenge level: " + difficulty);
        _correctAnswer = c.Item5 + 1;
        _questionText.text = c.Item1;
        _answer1Text.text = c.Item2;
        _answer2Text.text = c.Item3;
        _answer3Text.text = c.Item4;

        // dummy
        //_correctAnswer = _correctAnswer == 1 ? 2 : (_correctAnswer == 2 ? 3 : 1);
        //_questionText.text = "exp(0) * " + _correctAnswer + " =";
    }

    // Activate when correctly answering question
    IEnumerator Success()
    {
        OnProblemSuccess?.Invoke(AvatarReactions.ExpressionEvents.ProblemSucceeded);
        _questionActive = false;
        SpawnStuff(true);
        CanvasManager.Instance.IncrementScore(_gameParams.CorrectAnswerScore);
        SoundManager.Instance.PlayMusic(_bigLaserClip);
        Debug.Log("Correct answer!");

        // spawn laser
        StartCoroutine(LaserEffect.Instance.ActivateLaser(_gameParams.LaserDuration));
        StartCoroutine(Shooting.DisableShoot(_gameParams.LaserDuration));
        yield return new WaitForSeconds(_gameParams.LaserDuration);
        Explode();
        _durationDelta = 0;

        _questionAsteroid.SetActive(false);
        SoundManager.Instance.PlayMusic(_rightAnswerclip);
        SoundManager.Instance.PlaySound(_explosionClip, 0.5f);
    }

    // Activate when incorrectly ansering question or timer runs out
    void Fail()
    {
        OnProblemFailed?.Invoke(AvatarReactions.ExpressionEvents.ProblemFailed);
        _durationDelta = 0;
        _questionActive = false;
        _questionAsteroid.SetActive(false);
        SpawnStuff(false);
        Explode();
        SoundManager.Instance.PlayMusic(_wrongAnswerclip);
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
            float randomX = 0.1f * UnityEngine.Random.Range(-1f, 1f) * GameManager.Instance.ScreenBounds.x;
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
                Fail();
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
                Fail();
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
                Fail();
        }
    }
}