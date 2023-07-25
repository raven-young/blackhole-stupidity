using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DamageNumbersPro;

public class Ship : MonoBehaviour
{
    [SerializeField] private Camera _cam;
    [SerializeField] private SpriteRenderer gunSprite;
    [SerializeField] private GameObject deathEffect;
    [SerializeField] private GameObject PlayerHitEffect;

    [SerializeField] public bool isInvincible = false;
    private bool isDead = false;

    private PlayerInputActions playerInputActions;
    public static Ship Instance;

    [SerializeField] private DamageNumber dodgePrefab;
    [SerializeField] private float objectBoundsScale;

    private Vector2 screenBounds;
    private float objectWidth;
    private float objectHeight;

    [Header("Movement")]
    private Vector2 _movement;
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private float _forceMagnitude = 10f;
    [SerializeField] private float _speed = 5f;

    private void Awake()
    {
        Instance = this;
        playerInputActions = new PlayerInputActions();
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
        objectWidth = objectBoundsScale * transform.GetComponent<SpriteRenderer>().bounds.extents.x; //extents = size of width / 2
        objectHeight = objectBoundsScale * transform.GetComponent<SpriteRenderer>().bounds.extents.y; //extents = size of height / 2
    }

    void Update()
    {
        if (isDead)
            return;

        /// Movement
        _movement = playerInputActions.Player.Move.ReadValue<Vector2>();
    }

    // Executed on fixed frequency (default 50 Hz), good for physics as framerate is not const
    void FixedUpdate()
    {
        if (isDead)
            return;

        // old Movement
        // To avoid diagonal movement being sqrt(2) faster, normalize vector
        Vector2 newpos = _rb.position + _speed * Time.fixedDeltaTime * _movement.normalized;
        _rb.MovePosition(newpos);

        // Physics Movement
        //_rb.AddForceAtPosition(_forceMagnitude * movement.y * Vector2.right, transform.position, ForceMode2D.Force);
    }

    void LateUpdate()
    {
        // Clamp ship pos to screen bounds
        Vector3 viewPos = transform.position;
        viewPos.x = Mathf.Clamp(viewPos.x, -screenBounds.x + objectWidth, screenBounds.x - objectWidth);
        viewPos.y = Mathf.Clamp(viewPos.y, -screenBounds.y + objectHeight, screenBounds.y - objectHeight);
        transform.position = viewPos;
    }

    public void takeDamage(int firePower)
    {
        if (isInvincible)
        {
            return;
        }
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
}
