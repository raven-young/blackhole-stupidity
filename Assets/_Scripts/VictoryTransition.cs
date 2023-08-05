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

    private Material _shipMaterial;


    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    private void Start()
    {
        _shipMaterial = _shipTransform.gameObject.GetComponent<SpriteRenderer>().material;
        //_shipMaterial = _shipTransform.transform.Find("ItemMagnet").GetComponent<SpriteRenderer>().material;
    }

    public IEnumerator StartVictoryTransition()
    {
        // Freeze game
        //Time.timeScale = 0;
        Ship.Instance.CannotMove = true;
        Ship.Instance.IsInvincible = true;
        _shipMaterial.DOFloat(1, "_HologramBlend", 4f).SetDelay(0f);
        SoundManager.Instance.PlaySFX(SoundManager.SFX.Powerup);
        yield return new WaitForSecondsRealtime(1f);
        SoundManager.Instance.PlaySFX(SoundManager.SFX.VictoryFanfare);
        yield return new WaitForSecondsRealtime(5f);
        //DOTween.To(() => Time.timeScale, x => Time.timeScale = x, 1, 0.5f).SetEase(Ease.InQuad).SetUpdate(true);

        // Ship escapes
        _shipTransform.DOMove(1.5f * _shipTransform.position, 0.5f).SetUpdate(true);
        yield return new WaitForSecondsRealtime(2.5f);

        GameManager.Instance.PauseGame();
        // Victory screen
        SoundManager.Instance.PlaySound(_victoryClip);
        CanvasManager.Instance.RenderGameOverScreen(true);
        StartCoroutine(RaccoonBlink());

        yield return new WaitForSecondsRealtime(7f);
        SoundManager.Instance.StartMainGameMusic(4f);
    }

    IEnumerator RaccoonBlink()
    {
        // Time with victory sound effect
        yield return new WaitForSecondsRealtime(2.9f);
        _raccoonImage.sprite = _victorySprite;
        _raccoonImage.transform.DOScale(1.1f * _raccoonImage.transform.localScale, 0.3f).SetEase(Ease.InOutSine).SetLoops(2, LoopType.Yoyo).SetUpdate(true);
    }
}
