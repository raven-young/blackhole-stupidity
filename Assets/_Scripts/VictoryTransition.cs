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
    [SerializeField] private ParticleSystem _exhaustParticles;

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
        Ship.Instance.CannotMove = true;
        Ship.Instance.IsInvincible = true;
        _shipMaterial.DOFloat(1, "_HologramBlend", 4f).SetDelay(0f);
        _shipMaterial.DOFloat(18, "_ShakeUvSpeed", 4f).SetDelay(0f);
        _shipMaterial.DOFloat(4, "_ShakeUvX", 4f).SetDelay(0f);
        _shipMaterial.DOFloat(4, "_ShakeUvY", 4f).SetDelay(0.1f);

        var emission = _exhaustParticles.emission;
        emission.rateOverTime = 3* _exhaustParticles.emission.rateOverTime.constant;
        var main = _exhaustParticles.main;
        main.startSpeed = 3f * _exhaustParticles.main.startSpeed.constant;

        SoundManager.Instance.PlaySFX(SoundManager.SFX.Powerup);
        yield return new WaitForSecondsRealtime(1f);
        SoundManager.Instance.PlaySFX(SoundManager.SFX.VictoryFanfare);
        yield return new WaitForSecondsRealtime(5f);

        // Ship escapes
        SoundManager.Instance.PlaySFX(SoundManager.SFX.ShipTakeoff);
        _shipTransform.DOMove(3f * _shipTransform.position, 1f).SetUpdate(true);
        yield return new WaitForSecondsRealtime(2.7f);

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
