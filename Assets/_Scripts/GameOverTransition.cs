using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GameOverTransition : MonoBehaviour
{
    public static GameOverTransition Instance;

    [SerializeField] private AudioClip _gameOverClip;
    [SerializeField] private Transform _shipTransform;


    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    public IEnumerator StartGameOverTransition()
    {
        // Freeze game
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(0.5f);
        Time.timeScale = 1;

        if (Ship.Instance.CurrentHealth <= 0)
        {
            // Explode ship
            StartCoroutine(Ship.Instance.Die());
        } 
        else
        {
            // Ship falls into BH
            _shipTransform.DOMove(Vector3.zero, 1f).SetUpdate(true);
        }

        yield return new WaitForSecondsRealtime(5f);

        GameManager.Instance.PauseGame();
        // Game over screen
        SoundManager.Instance.PlaySound(_gameOverClip);
        CanvasManager.Instance.RenderGameOverScreen(false);
    }
}
