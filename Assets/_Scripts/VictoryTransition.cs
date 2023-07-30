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

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    public void StartVictoryTransition()
    {
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
