using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DamageNumbersPro;

public class Ship : MonoBehaviour
{
    [SerializeField] private Camera _cam;
    private PlayerInputActions playerInputActions;
    public static Ship Instance;

    [Header("Movement")]
    private Vector2 _movement;
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private float _forceMagnitude = 10f;
    [SerializeField] private float _speed = 5f;

    private float _theta = Mathf.PI/2;
    [SerializeField] private float _angularVelocity = 1f;
    [SerializeField] public float Radius = 2f; // distance from black hole

    [Header("Screen")]
    private Vector2 screenBounds;
    //private float objectWidth;
    //private float objectHeight;
    [SerializeField] private float objectBoundsScale;

    [Header("Logic")]
    [SerializeField] private float _initialHealth = 3;
    private float _currentHealth;
    private bool isDead = false;
    private bool isInvincible = false;

    [Header("Effects")]
    [SerializeField] private GameObject deathEffect;
    [SerializeField] private GameObject PlayerHitEffect;
    [SerializeField] private DamageNumber dodgePrefab;
    [SerializeField] protected FlashColor flashEffect;

    [Header("Debug")]
    [SerializeField] private bool _gravityOn = false;
    [Range(-5, 5)]
    [SerializeField] private float _gravityScale;

    private void Awake()
    {
        Instance = this;
        playerInputActions = new PlayerInputActions();
        _currentHealth = _initialHealth;
    }

    private void OnEnable()
    {
        playerInputActions.Player.Enable();
    }

    void OnDisable()
    {
        playerInputActions.Player.Disable();
    }

    private void Start()
    {
        //canvas = GameObject.FindObjectOfType<CanvasManager>().GetComponent<CanvasManager>();
        screenBounds = _cam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, _cam.transform.position.z));
        //objectWidth = objectBoundsScale * transform.GetComponent<SpriteRenderer>().bounds.extents.x; //extents = size of width / 2
        //objectHeight = objectBoundsScale * transform.GetComponent<SpriteRenderer>().bounds.extents.y; //extents = size of height / 2
    }

    void Update()
    {
        if (isDead)
            return;

        /// Movement
        _movement = playerInputActions.Player.Move.ReadValue<Vector2>();

        if (_gravityOn)
            Radius -= _gravityScale * Time.deltaTime;

    }

    // Executed on fixed frequency (default 50 Hz), good for physics as framerate is not const
    void FixedUpdate()
    {
        if (isDead)
            return;

        // old Movement
        // To avoid diagonal movement being sqrt(2) faster, normalize vector
        //Vector2 newpos = _rb.position + _speed * Time.fixedDeltaTime * _movement.normalized;
        //_rb.MovePosition(newpos);

        // Physics Movement
        //_rb.AddForceAtPosition(_forceMagnitude * movement.y * Vector2.right, transform.position, ForceMode2D.Force);

        // Polar Movement
        _theta -= _angularVelocity * _movement.x;

        // clamp angle to screen bounds
        // max angle depends on radius and screenbounds
        float thetaMax = Mathf.Acos(screenBounds.x / Radius);
        _theta = Mathf.Clamp(_theta, thetaMax, Mathf.PI-thetaMax);

        Vector2 newpos = new Vector2(Radius * Mathf.Cos(_theta), Radius * Mathf.Sin(_theta));
        _rb.MovePosition(newpos);
    }

    void LateUpdate()
    {
        // Clamp ship pos to screen bounds
        //Vector3 viewPos = transform.position;
        //viewPos.x = Mathf.Clamp(viewPos.x, -screenBounds.x + objectWidth, screenBounds.x - objectWidth);
        //viewPos.y = Mathf.Clamp(viewPos.y, -screenBounds.y + objectHeight, screenBounds.y - objectHeight);
        //transform.position = viewPos;
    }

    public void takeDamage(int damage)
    {
        if (isInvincible)
        {
            return;
        }

        flashEffect.Flash();

        Radius *= 0.9f;

        //_currentHealth -= damage;

        //if (_currentHealth <= 0)
        //{
        //    die();
        //}
    }

    IEnumerator activateInvincibility(float duration)
    {
        isInvincible = true;
        yield return new WaitForSeconds(duration);
        if (!isDead)
            isInvincible = false;
    }

    void die()
    {
        // death animation
        GameObject effect = Instantiate(deathEffect, transform.position, Quaternion.identity);
        Destroy(effect, 1f);
        gameObject.GetComponent<Renderer>().enabled = false;
        isDead = true;
        isInvincible = true;
        //StartCoroutine(GameManager.Instance.GameOver());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 10) // black hole
        {
            die();
        }
    }
}
