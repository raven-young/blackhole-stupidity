using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class VictoryTransition : MonoBehaviour
{
    public static VictoryTransition Instance;

    [SerializeField] private Sprite _victorySprite;
    [SerializeField] private Image _raccoonImage;
    [SerializeField] private AudioClip _victoryClip;
    [SerializeField] private Transform _shipTransform;


    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    public IEnumerator StartVictoryTransition()
    {
        // Freeze game
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(0f);
        Time.timeScale = 1;

        // Ship escapes
        _shipTransform.DOMove(1.5f * _shipTransform.position, 0.5f).SetUpdate(true);
        yield return new WaitForSecondsRealtime(1f);

        GameManager.Instance.PauseGame();
        // Victory screen
        SoundManager.Instance.PlaySound(_victoryClip);
        CanvasManager.Instance.RenderGameOverScreen(true);
        StartCoroutine(RaccoonBlink());
    }

    IEnumerator RaccoonBlink()
    {
        // Time with victory sound effect
        yield return new WaitForSecondsRealtime(2.9f);
        _raccoonImage.sprite = _victorySprite;
        _raccoonImage.transform.DOScale(1.1f * _raccoonImage.transform.localScale, 0.3f).SetEase(Ease.InOutSine).SetLoops(2, LoopType.Yoyo).SetUpdate(true);
    }
}
