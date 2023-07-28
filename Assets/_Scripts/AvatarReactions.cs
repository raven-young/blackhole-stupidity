using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AvatarReactions : MonoBehaviour
{

    private Sprite _sprite;

    [SerializeField] private Sprite _idleDanger;
    [SerializeField] private Sprite _idleSafe;
    [SerializeField] private Sprite _gameOver;
    [SerializeField] private Sprite _victory;
    [SerializeField] private Sprite _newProblem;
    [SerializeField] private Sprite _shipHit;
    [SerializeField] private Sprite _failedProblem;

    [SerializeField] private float _minExpressionTime = 2f;
    private float _expressionTimer = 0f;

    private enum ExpressionStates
    {
        IdleDanger,
        IdleSafe,
        GameOver,
        Victory,
        NewProblem,
        ShipHit,
        FailedProblem
    }

    // Start is called before the first frame update
    void Start()
    {
        _sprite = gameObject.GetComponent<Image>().sprite;
        _sprite = _idleSafe;
        StartCoroutine(test());
    }

    IEnumerator test()
    {
        Debug.Log(gameObject.GetComponent<Image>().sprite);
        yield return new WaitForSeconds(3f);
        _sprite = _newProblem;
        Debug.Log(gameObject.GetComponent<Image>().sprite);
    }

    private void OnEnable()
    {
        GameManager.OnEnteredDangerZone += SwapExpression;
        GameManager.OnExitedDangerZone += SwapExpression;
    }

    private void OnDisable()
    {
        GameManager.OnEnteredDangerZone -= SwapExpression;
        GameManager.OnExitedDangerZone -= SwapExpression;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SwapExpression()
    {

    }
}
