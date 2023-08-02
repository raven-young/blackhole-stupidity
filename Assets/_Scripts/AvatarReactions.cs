using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// crap code because no time
public class AvatarReactions : MonoBehaviour
{
    public static AvatarReactions Instance;

    private Image _image;

    [SerializeField] private Sprite _idleDanger;
    [SerializeField] private Sprite _idleSafe;
    [SerializeField] private Sprite _victory;
    [SerializeField] private Sprite _problemSpawned;
    [SerializeField] private Sprite _asteroidHit;
    [SerializeField] private Sprite _problemFailed;
    [SerializeField] private Sprite _problemSuccess;

    //[SerializeField] private float _minExpressionTime = 2f;
    //private float _expressionTimer = 0f;

    //private bool _reactionActive = false;
    //private bool _idleActive = true;

    // The currently running coroutine.
    private Coroutine _reactRoutine;

    public enum ExpressionEvents
    {
        ProblemSpawned,
        ProblemFailed,
        ProblemSucceeded,
        AsteroidHit
    }

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        _image = gameObject.GetComponent<Image>();
        _image.sprite = _idleSafe;
    }

    private void OnEnable()
    {
        GameManager.OnEnteredDangerZone += SwapExpression;
        GameManager.OnExitedDangerZone += SwapExpression;
        GameManager.OnGameOver += SwapExpression;
        GameManager.OnVictory += SwapExpression;

        Asteroid.OnAsteroidHit += SwapReaction;
        QuestionAsteroid.OnProblemFailed += SwapReaction;
        QuestionAsteroid.OnProblemSpawned += SwapReaction;
        QuestionAsteroid.OnProblemSuccess += SwapReaction;
    }

    private void OnDisable()
    {
        GameManager.OnEnteredDangerZone -= SwapExpression;
        GameManager.OnExitedDangerZone -= SwapExpression;
        GameManager.OnGameOver -= SwapExpression;
        GameManager.OnVictory -= SwapExpression;

        Asteroid.OnAsteroidHit -= SwapReaction;
        QuestionAsteroid.OnProblemFailed -= SwapReaction;
        QuestionAsteroid.OnProblemSpawned -= SwapReaction;
        QuestionAsteroid.OnProblemSuccess -= SwapReaction;
    }

    void SwapReaction(ExpressionEvents expEvent)
    {
        switch (expEvent)
        {
            case ExpressionEvents.ProblemFailed:
                React(_problemFailed);
                break;
            case ExpressionEvents.ProblemSpawned:
                //React(_problemSpawned); // problems spawn too frequently, disable for now
                break;
            case ExpressionEvents.ProblemSucceeded:
                React(_problemSuccess);
                break;
            case ExpressionEvents.AsteroidHit:
                React(_asteroidHit);
                break;

        }
    }

    public void React(Sprite sprite)
    {
        if (GameManager.Instance.GameHasEnded)
            return;

        // If the routine is not null, then it is currently running.
        if (_reactRoutine != null)
        {
            // In this case, we should stop it first.
            // Multiple routines the same time would cause bugs.
            StopCoroutine(_reactRoutine);
        }

        // Start the Coroutine, and store the reference for it.
        _reactRoutine = StartCoroutine(ReactRoutine(sprite));
    }

    IEnumerator ReactRoutine(Sprite sprite)
    {
        _image.sprite = sprite;
        yield return new WaitForSeconds(2f);
        SwapExpression();
    }

    // Update is called once per frame
    //void Update()
    //{
    //    _expressionTimer += Time.deltaTime;
    //}



    void SwapExpression()
    {

        // do later
        //if (_expressionTimer < _minExpressionTime)
        //    return;

        //if (_reactionActive)
        //    return;

        if (GameManager.Instance.GameHasEnded)
        {
            _image.sprite = GameManager.Instance.GameWasWon ? _victory : _asteroidHit;
            //_expressionTimer = 0f;
            return;
        }

        if (GameManager.Instance.InDangerZone)
        {
            _image.sprite = _idleDanger;
            //_expressionTimer = 0f;
            return;
        }

        if (!GameManager.Instance.InDangerZone)
        {
            _image.sprite = _idleSafe;
            //_expressionTimer = 0f;
            return;
        }

        //_idleActive = true;
    }
}