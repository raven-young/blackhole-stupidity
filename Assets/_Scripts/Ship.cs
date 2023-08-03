using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DamageNumbersPro;
using UnityEngine.UI;

public class Ship : MonoBehaviour
{
    [SerializeField] private GameParams _gameParams;
    //[SerializeField] private Camera _cam;
    private PlayerInputActions _playerInputActions;
    public static Ship Instance;

    [Header("Movement")]
    private Vector2 _movement;
    [SerializeField] private Rigidbody2D _rb;
    //[SerializeField] private float _forceMagnitude = 10f;
    //[SerializeField] private float _speed = 5f;

    private float _theta = Mathf.PI/2;
    public float ShipPositionRadius; // distance from black hole

    [Header("Screen")]
    //private Vector2 screenBounds;
    [SerializeField] private float objectBoundsScale;

    [Header("Logic")]
    [SerializeField] public float InitialFuel = 100;
    public float CurrentHealth;
    public float CurrentFuel;
    private float _burnRate;

    private bool cannotMove = false;
    private bool isInvincible = false;

    [Header("Effects")]
    [SerializeField] private GameObject deathEffect;
    [SerializeField] private GameObject PlayerHitEffect;
    [SerializeField] private DamageNumber dodgePrefab;
    [SerializeField] protected FlashColor flashEffect;
    [SerializeField] private ParticleSystem _exhaustParticles;
    [SerializeField] private AudioClip _explosionClip;
    private float _exhaustEmissionRate;
    private float _exhaustSpeed;

    [Header("Debug")]
    [SerializeField] private bool _gravityOn = false;
    [Range(-5, 5)]
    [SerializeField] private float _gravityScale;

    private void Awake()
    {
        Instance = this;
        _playerInputActions = new PlayerInputActions();
        CurrentHealth = _gameParams.MaxHealth/2;
        CurrentFuel = InitialFuel;
    }

    private void OnEnable()
    {
        _playerInputActions.Player.Enable();
    }

    void OnDisable()
    {
        _playerInputActions.Player.Disable();
    }

    private void Start()
    {
        //screenBounds = _cam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, _cam.transform.position.z));
        ShipPositionRadius = transform.position.y;
        _burnRate = _gameParams.BurnRate;

        _exhaustEmissionRate = 1.5f*_exhaustParticles.emission.rateOverTime.constant;
        _exhaustSpeed = 1.5f*_exhaustParticles.main.startSpeed.constant;

        switch (SettingsManager.Instance.SelectedDifficulty)
        {
            case SettingsManager.DifficultySetting.Easy:
                _burnRate *= _gameParams.EasyMultiplier;
                break;
        }
    }

    void Update()
    {
        if (cannotMove)
            return;

        // workaround due to broken triggers
        if (ShipPositionRadius > _gameParams.WinRadius && !GameManager.Instance.GameHasEnded)
        {
            cannotMove = true;
            StartCoroutine(GameManager.Instance.GameOver(true));
            return;
        }

        if (CurrentHealth <= 0)
        {
            cannotMove = true;
            StartCoroutine(GameManager.Instance.GameOver(false));
            return;
        }

        CurrentFuel -= _burnRate * Time.deltaTime;
        CanvasManager.Instance.UpdateFuel(CurrentFuel);
        UpdateExhaustParticles();

        /// Movement
        _movement = _playerInputActions.Player.Move.ReadValue<Vector2>();

        if (_gravityOn)
            ShipPositionRadius -= _gravityScale * Time.deltaTime;

        // calculate radius based on ship health and black hole mass
        var ratio = CurrentHealth / BlackHole.Instance.CurrentForce;
        var velocity = CurrentFuel <= 0 ? -40 : ratio > 1 ? ratio : ratio == 1 ? 0 : -1 / ratio;
        ShipPositionRadius += _gameParams.VelocityScale * velocity * Time.deltaTime;

        // rotate ship
        transform.rotation = Quaternion.Euler(0, 0, _theta*Mathf.Rad2Deg-90);

    }

    // Executed on fixed frequency (default 50 Hz), good for physics as framerate is not const
    void FixedUpdate()
    {
        if (cannotMove)
            return;

        // old Movement
        // To avoid diagonal movement being sqrt(2) faster, normalize vector
        //Vector2 newpos = _rb.position + _speed * Time.fixedDeltaTime * _movement.normalized;
        //_rb.MovePosition(newpos);

        // Physics Movement
        //_rb.AddForceAtPosition(_forceMagnitude * movement.y * Vector2.right, transform.position, ForceMode2D.Force);

        // Polar Movement
        _theta -= Time.fixedDeltaTime * _gameParams.AngularVelocity * _movement.x;

        // clamp angle to screen bounds
        // if using flexible aspect ratio: max angle depends on radius and screenbounds:
        // float thetaMax = Mathf.Acos(screenBounds.x / ShipPositionRadius); //
        // else use fixed max theta
        float thetaMax = _gameParams.MaxTheta;
        _theta = Mathf.Clamp(_theta, thetaMax, Mathf.PI-thetaMax);

        Vector2 newpos = new Vector2(ShipPositionRadius * Mathf.Cos(_theta), ShipPositionRadius * Mathf.Sin(_theta));
        _rb.MovePosition(newpos);
    }

    //void LateUpdate()
    //{
    //    // Clamp ship pos to screen bounds
    //    //Vector3 viewPos = transform.position;
    //    //viewPos.x = Mathf.Clamp(viewPos.x, -screenBounds.x + objectWidth, screenBounds.x - objectWidth);
    //    //viewPos.y = Mathf.Clamp(viewPos.y, -screenBounds.y + objectHeight, screenBounds.y - objectHeight);
    //    //transform.position = viewPos;
    //}

    private void UpdateExhaustParticles()
    {
        var emission = _exhaustParticles.emission;
        emission.rateOverTime = CurrentFuel <= 0 ? 0 : _exhaustEmissionRate * CurrentHealth / _gameParams.MaxHealth;
        var main = _exhaustParticles.main;
        main.startSpeed = _exhaustSpeed * CurrentHealth / _gameParams.MaxHealth;
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible)
        {
            return;
        }

        flashEffect.Flash();

        CurrentHealth -= damage;
        CanvasManager.Instance.UpdateHealth(CurrentHealth);
        StartCoroutine(ActivateInvincibility(0.2f));
    }

    IEnumerator ActivateInvincibility(float duration)
    {
        isInvincible = true;
        yield return new WaitForSeconds(duration);
        if (!cannotMove)
            isInvincible = false;
    }

    public IEnumerator Die()
    {
        // death animation
        Vector3 explosionOffset = new(UnityEngine.Random.Range(-1.2f, 1.2f), UnityEngine.Random.Range(-1.2f, 1.2f), 0);
        GameObject effect1 = Instantiate(deathEffect, transform.position + explosionOffset, Quaternion.identity);
        Destroy(effect1, 1f);
        SoundManager.Instance.PlaySound(_explosionClip, 0.5f);
        yield return new WaitForSeconds(0.5f);

        Vector3 explosionOffset2 = new(UnityEngine.Random.Range(-1.2f, 1.2f), UnityEngine.Random.Range(-1.2f, 1.2f), 0);
        GameObject effect2 = Instantiate(deathEffect, transform.position + explosionOffset2, Quaternion.identity);
        Destroy(effect2, 1f);
        SoundManager.Instance.PlaySound(_explosionClip, 0.5f);
        yield return new WaitForSeconds(0.6f);

        Vector3 explosionOffset3 = new(UnityEngine.Random.Range(-1.2f, 1.2f), UnityEngine.Random.Range(-1.2f, 1.2f), 0);
        GameObject effect3 = Instantiate(deathEffect, transform.position + explosionOffset3, Quaternion.identity);
        Destroy(effect3, 1f);
        SoundManager.Instance.PlaySound(_explosionClip, 0.5f);
        yield return new WaitForSeconds(0.4f);

        yield return new WaitForSeconds(1.5f);

        cannotMove = true;
        isInvincible = true;
        gameObject.SetActive(false);
        GameObject finaleffect = Instantiate(deathEffect, transform.position, Quaternion.identity);
        finaleffect.transform.localScale *= 2f;
        Destroy(finaleffect, 1f);
        SoundManager.Instance.PlaySound(_explosionClip, 0.5f);
        yield return new WaitForSeconds(0.3f);
        SoundManager.Instance.PlaySound(_explosionClip, 0.5f);
        yield return new WaitForSeconds(0.3f);
        SoundManager.Instance.PlaySound(_explosionClip, 0.5f);
        yield return new WaitForSeconds(2f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.layer == 11)
            Debug.Log("enter " + collision.gameObject.layer);

        if (collision.gameObject.layer == 10) // black hole
        {
            StartCoroutine(GameManager.Instance.GameOver(victorious: false));
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 11)
            Debug.Log("exit " + collision.gameObject.layer);
    }
}
