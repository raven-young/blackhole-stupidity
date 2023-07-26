using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class QuestionAsteroid : MonoBehaviour
{
    private PlayerInputActions playerInputActions;

    [SerializeField] private GameObject _scrapPrefab;
    [SerializeField] private GameObject _fuelPrefab;
    [SerializeField] private GameObject _asteroidPrefab;

    [Header("Parameters")]
    [SerializeField] private float _questionDuration; // time to answer the question
    [SerializeField] private float _questionDelta; // time until next question
    [SerializeField] private int _spawnAmount = 3; // how many things to spawn after answering question
    [Range(0,10)]
    [SerializeField] private float _spawnImpulse; // max impulse applied to spawned objects
    [Range(0,360)]
    [SerializeField] private float _maxSpawnAngle;

    [Header("Question")]
    [SerializeField] private GameObject _questionAsteroid;
    [SerializeField] private TMP_Text _questionText;
    [SerializeField] private TMP_Text _answer1Text;
    [SerializeField] private TMP_Text _answer2Text;
    [SerializeField] private TMP_Text _answer3Text;

    private bool _questionActive = false;
    private int _correctAnswer; // 1,2,3
    private float _durationDelta = 0;
    private float _deltaDelta = 0;
    private MathChallenge challenge;


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
        if (!_questionActive)
        {
            _deltaDelta += Time.deltaTime;

            if (_deltaDelta > _questionDelta)
            {   
                SpawnQuestion();
            }
        }

        else
        {
            _durationDelta += Time.deltaTime;

            if (_durationDelta > _questionDuration)
            {
                Fail();
            }
        }
    }

    void SpawnQuestion()
    {
        _deltaDelta = 0;
        _questionActive = true;
        _questionAsteroid.SetActive(true);

        var c = challenge.challenge((int)Ship.Instance.ShipPositionRadius/4);
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
    void Success()
    {
        _durationDelta = 0;
        _questionActive = false;
        _questionAsteroid.SetActive(false);
        SpawnStuff(true);
        Debug.Log("Correct answer!");

        // spawn fuel & scrap
    }

    // Activate when incorrectl ansering question or timer runs out
    void Fail()
    {
        _durationDelta = 0;
        _questionActive = false;
        _questionAsteroid.SetActive(false);
        SpawnStuff(false);
        Debug.Log("Wrong answer!");

        // spawn debris
    }

    void SpawnStuff(bool correctlyAnswered)
    {
        GameObject prefab1 = _scrapPrefab;
        GameObject prefab2 = _fuelPrefab;

        if (!correctlyAnswered) 
            prefab1 = prefab2 = _asteroidPrefab;

        for (int i = 0; i < _spawnAmount; i++)
        {
            float randomX = 0.3f * GameManager.Instance.ScreenBounds.x * (Random.Range(0, 1) < 0.5 ? 1 : -1);
            Vector2 spawnPos = new Vector2(transform.position.x + randomX, transform.position.y);
            GameObject spawnedObject = Instantiate(Random.Range(0f, 1f) < 0.5 ? prefab1 : prefab2,
                                                   spawnPos, Quaternion.identity);

            Vector2 direction = Vector2.up.Rotate(Random.Range(-_maxSpawnAngle, _maxSpawnAngle));

            spawnedObject.GetComponent<Rigidbody2D>().AddForce(-Random.Range(0.5f, 1f) * _spawnImpulse * direction, 
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
                Success();
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
                Success();
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
                Success();
            else
                Fail();
        }
    }
}
