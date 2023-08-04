using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{

    [SerializeField] protected GameParams _gameParams;
    [SerializeField] private Rigidbody2D _rb;

    [SerializeField] private AudioClip _pickupClip;

    private bool _collected = false;

    private void Start()
    {
        // random flip
        gameObject.GetComponent<SpriteRenderer>().flipY = UnityEngine.Random.Range(0f, 1f) < 0.5f;

        var spin = UnityEngine.Random.Range(0f, 1f) < 0.5f ? 1 : -1;
        // random torque
        _rb.AddTorque(10f * spin, ForceMode2D.Impulse);
    }
    private void Update()
    {
        // Despawn if below x axis
        if (transform.position.y < -1)
            Destroy(gameObject);
    }

    private void FixedUpdate()
    {
        if (_collected)
        {
            transform.position = Vector2.MoveTowards(transform.position, Ship.Instance.transform.position, 10 * Time.deltaTime);
            return;
        }

        // force in direction of black hole
        float force = BlackHole.Instance.CurrentForce;
        _rb.AddForce(-transform.position * force * Time.fixedDeltaTime, ForceMode2D.Force);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.layer == 9) // ship's collector
        {
            _collected = true;
        }

        if (collision.gameObject.layer == 6) // ship
        {
            ApplyItem();
            CanvasManager.Instance.IncrementScore(_gameParams.CollectedItemScore);
            Destroy(gameObject);
        }
    }

    protected virtual void ApplyItem()
    {
        SoundManager.Instance.PlaySound(_pickupClip);
    }
}
