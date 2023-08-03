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
    [SerializeField] private Camera _cam;

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
        yield return new WaitForSecondsRealtime(0f);
        Time.timeScale = 1;

        SoundManager.Instance.PlaySFX(SoundManager.SFX.AlertSFX);

        if (Ship.Instance.CurrentHealth <= 0)
        {
            // Explode ship
            StartCoroutine(Ship.Instance.Die());
            yield return new WaitForSecondsRealtime(4f);
        } 
        else
        {
            // Ship falls into BH
            _shipTransform.DOMove(Vector3.zero, 1f).SetUpdate(true);

            // Camera zooms into BH

            // Add a movement tween at the beginning
            Sequence mySequence = DOTween.Sequence();

            mySequence.Append(_cam.transform.DOMoveY(3.5f, 2f).SetUpdate(true));
            mySequence.Append(_cam.transform.DOMoveY(3.4f, 9f).SetUpdate(true)); // camera teleports back after first tween so use this hacky workaround

            _cam.DOOrthoSize(1.8f, 2f).SetUpdate(true);
            yield return new WaitForSecondsRealtime(3f);
        }

        GameManager.Instance.PauseGame();
        // Game over screen
        SoundManager.Instance.PlaySound(_gameOverClip);
        CanvasManager.Instance.RenderGameOverScreen(false);
    }
}
